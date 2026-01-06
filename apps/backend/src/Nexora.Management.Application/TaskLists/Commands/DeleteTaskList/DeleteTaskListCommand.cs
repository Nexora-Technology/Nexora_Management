using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TaskLists.Commands.DeleteTaskList;

public record DeleteTaskListCommand(Guid Id) : IRequest<Result>;

public class DeleteTaskListCommandHandler : IRequestHandler<DeleteTaskListCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeleteTaskListCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteTaskListCommand request, CancellationToken ct)
    {
        var taskList = await _db.TaskLists.FirstOrDefaultAsync(tl => tl.Id == request.Id, ct);
        if (taskList == null)
        {
            return Result.Failure("TaskList not found");
        }

        _db.TaskLists.Remove(taskList);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
