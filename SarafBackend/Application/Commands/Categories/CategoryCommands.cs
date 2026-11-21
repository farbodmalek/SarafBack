using MediatR;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Exceptions;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Categories
{
    /// <summary>ایجاد (Id خالی) یا ویرایش (Id موجود) دسته‌بندی.</summary>
    public class SetCategoryCommand : IRequest<long>
    {
        public long? Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Slug { get; set; }
        public long? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SetCategoryCommandHandler : IRequestHandler<SetCategoryCommand, long>
    {
        private readonly ICategoryRepositoryWrite _repository;

        public SetCategoryCommandHandler(ICategoryRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<long> Handle(SetCategoryCommand request, CancellationToken cancellationToken)
        {
            var name = request.Name?.Trim() ?? string.Empty;
            if (name.Length == 0)
            {
                throw new BusinessException("نام دسته‌بندی را وارد کن.");
            }

            var slug = string.IsNullOrWhiteSpace(request.Slug) ? Slugify(name) : Slugify(request.Slug);
            if (slug.Length == 0)
            {
                throw new BusinessException("آدرس (slug) دسته‌بندی معتبر نیست.");
            }

            var isUpdate = request.Id.HasValue && request.Id.Value > 0;

            if (await _repository.SlugExistsAsync(slug, isUpdate ? request.Id : null, cancellationToken))
            {
                throw new ConflictException("دسته‌بندی دیگری با همین آدرس (slug) وجود دارد.");
            }

            if (isUpdate)
            {
                var category = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken)
                    ?? throw new KeyNotFoundException("دسته‌بندی پیدا نشد.");

                if (request.ParentId == category.Id)
                {
                    throw new BusinessException("یک دسته نمی‌تواند زیرمجموعه‌ی خودش باشد.");
                }

                category.Name = name;
                category.Slug = slug;
                category.ParentId = request.ParentId;
                category.IsActive = request.IsActive;

                await _repository.SaveChangesAsync(cancellationToken);
                return category.Id;
            }

            var created = new Category
            {
                Name = name,
                Slug = slug,
                ParentId = request.ParentId,
                IsActive = request.IsActive
            };

            await _repository.AddAsync(created, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return created.Id;
        }

        // «لباس مجلسی» → «لباس-مجلسی»؛ حروف فارسی و انگلیسی و عدد می‌مانند.
        private static string Slugify(string value)
        {
            var chars = value.Trim().ToLowerInvariant()
                .Select(c => char.IsLetterOrDigit(c) ? c : '-')
                .ToArray();

            var slug = new string(chars);
            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }
            return slug.Trim('-');
        }
    }

    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public long Id { get; set; }

        public DeleteCategoryCommand(long id)
        {
            Id = id;
        }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ICategoryRepositoryWrite _repository;

        public DeleteCategoryCommandHandler(ICategoryRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new KeyNotFoundException("دسته‌بندی پیدا نشد.");

            if (await _repository.HasProductsAsync(request.Id, cancellationToken))
            {
                throw new ConflictException("این دسته‌بندی محصول دارد؛ اول محصولاتش را جابه‌جا یا حذف کن.");
            }

            _repository.Remove(category);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
