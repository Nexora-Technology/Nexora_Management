using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Dashboards.Commands.UpdateDashboard;

public record UpdateDashboardCommand(
    Guid DashboardId,
    string? Name,
    string? Layout
) : IRequest<Result<DashboardDto>>;

public class UpdateDashboardCommandHandler : IRequestHandler<UpdateDashboardCommand, Result<DashboardDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public UpdateDashboardCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async Task<Result<DashboardDto>> Handle(UpdateDashboardCommand request, CancellationToken ct)
    {
        var dashboard = await _db.Dashboards
            .FirstOrDefaultAsync(d => d.Id == request.DashboardId, ct);

        if (dashboard == null)
        {
            return Result<DashboardDto>.Failure("Dashboard not found");
        }

        // Check if user is the creator or workspace admin
        var membership = await _db.WorkspaceMembers
            .Include(wm => wm.Role)
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == dashboard.WorkspaceId
                                     && wm.UserId == _userContext.UserId, ct);

        if (membership == null)
        {
            return Result<DashboardDto>.Failure("You are not a member of this workspace");
        }

        if (dashboard.CreatedBy != _userContext.UserId && membership.Role.Name != "Admin")
        {
            return Result<DashboardDto>.Failure("You don't have permission to update this dashboard");
        }

        if (request.Name != null)
        {
            dashboard.Name = request.Name;
        }

        if (request.Layout != null)
        {
            dashboard.Layout = request.Layout;
        }

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return Result<DashboardDto>.Failure($"Failed to update dashboard: {ex.Message}");
        }

        var dashboardDto = new DashboardDto(
            dashboard.Id,
            dashboard.WorkspaceId,
            dashboard.Name,
            dashboard.Layout,
            dashboard.CreatedBy,
            dashboard.IsTemplate,
            dashboard.CreatedAt,
            dashboard.UpdatedAt
        );

        return Result<DashboardDto>.Success(dashboardDto);
    }
}
