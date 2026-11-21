using MediatR;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Exceptions;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Addresses
{
    /// <summary>ایجاد (Id خالی) یا ویرایش (Id موجود) آدرس یک کاربر.</summary>
    public class SetAddressCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public int? Id { get; set; }
        public string Title { get; set; } = default!;
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
    }

    public class SetAddressCommandHandler : IRequestHandler<SetAddressCommand, int>
    {
        private const int MaxAddressesPerUser = 10;
        private readonly IAddressRepositoryWrite _repository;

        public SetAddressCommandHandler(IAddressRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(SetAddressCommand request, CancellationToken cancellationToken)
        {
            var title = request.Title?.Trim() ?? string.Empty;
            var detail = request.Detail?.Trim() ?? string.Empty;
            if (title.Length == 0 || detail.Length == 0)
            {
                throw new BusinessException("عنوان و آدرس کامل را وارد کن.");
            }

            var isUpdate = request.Id.HasValue && request.Id.Value > 0;
            Address address;

            if (isUpdate)
            {
                address = await _repository.GetTrackedAsync(request.UserId, request.Id!.Value, cancellationToken)
                    ?? throw new KeyNotFoundException("آدرس پیدا نشد.");
            }
            else
            {
                var count = await _repository.CountAsync(request.UserId, cancellationToken);
                if (count >= MaxAddressesPerUser)
                {
                    throw new BusinessException($"حداکثر {MaxAddressesPerUser} آدرس می‌توانی ذخیره کنی.");
                }

                address = new Address { UserId = request.UserId };
                // اولین آدرس کاربر خودکار پیش‌فرض می‌شود
                address.IsDefault = count == 0;
                await _repository.AddAsync(address, cancellationToken);
            }

            address.Title = title;
            address.RecipientName = request.RecipientName?.Trim();
            address.PhoneNumber = request.PhoneNumber?.Trim();
            address.City = request.City?.Trim();
            address.PostalCode = request.PostalCode?.Trim();
            address.Detail = detail;

            if (request.IsDefault)
            {
                address.IsDefault = true;
            }

            await _repository.SaveChangesAsync(cancellationToken);

            if (address.IsDefault)
            {
                // بقیه‌ی آدرس‌ها دیگر پیش‌فرض نیستند
                await _repository.ClearDefaultAsync(request.UserId, address.Id, cancellationToken);
            }

            return address.Id;
        }
    }

    public class DeleteAddressCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
    }

    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Unit>
    {
        private readonly IAddressRepositoryWrite _repository;

        public DeleteAddressCommandHandler(IAddressRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _repository.GetTrackedAsync(request.UserId, request.AddressId, cancellationToken)
                ?? throw new KeyNotFoundException("آدرس پیدا نشد.");

            var wasDefault = address.IsDefault;

            _repository.Remove(address);
            await _repository.SaveChangesAsync(cancellationToken);

            if (wasDefault)
            {
                await _repository.PromoteOldestAsDefaultAsync(request.UserId, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
