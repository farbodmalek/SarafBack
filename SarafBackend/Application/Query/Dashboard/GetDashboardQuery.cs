using System.Globalization;
using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Dashboard
{
    public class GetDashboardQuery : IRequest<DashboardDto>
    {
    }

    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
    {
        private readonly IDashboardRepositoryRead _repository;

        public GetDashboardQueryHandler(IDashboardRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // «فروش این ماه» یعنی از اول ماه شمسی جاری
            var persian = new PersianCalendar();
            var monthStart = persian.ToDateTime(persian.GetYear(now), persian.GetMonth(now), 1, 0, 0, 0, 0);

            return _repository.GetAsync(now.Date, monthStart, cancellationToken);
        }
    }
}
