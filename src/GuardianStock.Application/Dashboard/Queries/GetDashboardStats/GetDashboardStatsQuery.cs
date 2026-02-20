using GuardianStock.Application.Dashboard.Queries;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Dashboard.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<Result<DashboardDto>>
    {
    }
}
