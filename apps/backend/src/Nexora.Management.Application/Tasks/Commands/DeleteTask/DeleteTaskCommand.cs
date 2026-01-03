using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest<Result<Guid?>>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<Guid?>>
{
    private readonly IAppDbContext _db;

    public DeleteTaskCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<Guid?>> Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (task == null)
        {
            return Result<Guid?>.Failure("Task not found");
        }

        // Check if task has subtasks
        var hasSubtasks = await _db.Tasks.AnyAsync(t => t.ParentTaskId == request.Id, ct);
        if (hasSubtasks)
        {
            return Result<Guid?>.Failure("Cannot delete task with subtasks. Delete or move subtasks first.");
        }

        var projectId = task.ProjectId;
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync(ct);

        return Result<Guid?>.Success(projectId);
    }
}
