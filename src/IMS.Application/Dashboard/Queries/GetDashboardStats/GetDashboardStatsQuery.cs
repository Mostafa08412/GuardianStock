using IMS.Application.Dashboard.Queries;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Dashboard.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<Result<DashboardDto>>
    {
    }
}
