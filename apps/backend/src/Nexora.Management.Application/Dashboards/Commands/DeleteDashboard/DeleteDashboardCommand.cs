using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Dashboards.Commands.DeleteDashboard;

public record DeleteDashboardCommand(Guid DashboardId) : IRequest<Result>;

public class DeleteDashboardCommandHandler : IRequestHandler<DeleteDashboardCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public DeleteDashboardCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async Task<Result> Handle(DeleteDashboardCommand request, CancellationToken ct)
    {
        var dashboard = await _db.Dashboards
            .FirstOrDefaultAsync(d => d.Id == request.DashboardId, ct);

        if (dashboard == null)
        {
            return Result.Failure("Dashboard not found");
        }

        // Check if user is the creator or workspace admin
        var membership = await _db.WorkspaceMembers
            .Include(wm => wm.Role)
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == dashboard.WorkspaceId
                                     && wm.UserId == _userContext.UserId, ct);

        if (membership == null)
        {
            return Result.Failure("You are not a member of this workspace");
        }

        if (dashboard.CreatedBy != _userContext.UserId && membership.Role.Name != "Admin")
        {
            return Result.Failure("You don't have permission to delete this dashboard");
        }

        _db.Dashboards.Remove(dashboard);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return Result.Failure($"Failed to delete dashboard: {ex.Message}");
        }

        return Result.Success();
    }
}
