using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest<Result>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeleteTaskCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (task == null)
        {
            return Result.Failure("Task not found");
        }

        // Check if task has subtasks
        var hasSubtasks = await _db.Tasks.AnyAsync(t => t.ParentTaskId == request.Id, ct);
        if (hasSubtasks)
        {
            return Result.Failure("Cannot delete task with subtasks. Delete or move subtasks first.");
        }

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
