using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Dashboards.Commands.CreateDashboard;

public record CreateDashboardCommand(
    Guid WorkspaceId,
    string Name,
    string? Layout,
    bool IsTemplate = false
) : IRequest<Result<DashboardDto>>;

public class CreateDashboardCommandHandler : IRequestHandler<CreateDashboardCommand, Result<DashboardDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreateDashboardCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async Task<Result<DashboardDto>> Handle(CreateDashboardCommand request, CancellationToken ct)
    {
        // Validate workspace exists and user is a member
        var workspace = await _db.Workspaces
            .Include(w => w.Members)
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, ct);

        if (workspace == null)
        {
            return Result<DashboardDto>.Failure("Workspace not found");
        }

        var isMember = workspace.Members.Any(m => m.UserId == _userContext.UserId);
        if (!isMember)
        {
            return Result<DashboardDto>.Failure("You are not a member of this workspace");
        }

        var dashboard = new Dashboard
        {
            WorkspaceId = request.WorkspaceId,
            Name = request.Name,
            Layout = request.Layout,
            CreatedBy = _userContext.UserId,
            IsTemplate = request.IsTemplate
        };

        _db.Dashboards.Add(dashboard);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return Result<DashboardDto>.Failure($"Failed to create dashboard: {ex.Message}");
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
