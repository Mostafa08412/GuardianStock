using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Dashboard.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<Result<DashboardDto>>
    {
    }
}
