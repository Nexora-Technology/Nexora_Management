using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Queries.GetUserWorkspaces;

/// <summary>
/// Handler for getting all workspaces for a user
/// </summary>
public class GetUserWorkspacesQueryHandler : IRequestHandler<GetUserWorkspacesQuery, Result<List<UserWorkspaceResponse>>>
{
    private readonly AppDbContext _dbContext;

    public GetUserWorkspacesQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<UserWorkspaceResponse>>> Handle(GetUserWorkspacesQuery query, CancellationToken cancellationToken)
    {
        // Validate user exists
        var userExists = await _dbContext.Users
            .AnyAsync(u => u.Id == query.UserId, cancellationToken);

        if (!userExists)
        {
            return Result<List<UserWorkspaceResponse>>.Failure("User not found");
        }

        // Get all workspaces where user is a member OR is the owner
        var workspaces = await _dbContext.Workspaces
            .Where(w => w.OwnerId == query.UserId ||
                        w.Members.Any(m => m.UserId == query.UserId))
            .Select(w => new UserWorkspaceResponse
            {
                Id = w.Id,
                Name = w.Name,
                IsOwner = w.OwnerId == query.UserId,
                MemberCount = w.Members.Count + 1, // +1 for the owner
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            })
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);

        return Result<List<UserWorkspaceResponse>>.Success(workspaces);
    }
}
