using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Operations.Checklists;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Commands;

internal sealed class DeleteChecklistItemCommandHandler : IRequestHandler<DeleteChecklistItemCommand, Result>
{
    private readonly IRepository<ChecklistItem> _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenant;

    public DeleteChecklistItemCommandHandler(
        IRepository<ChecklistItem> repo,
        IUnitOfWork unitOfWork,
        ITenantContext tenant)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _tenant = tenant;
    }

    public async Task<Result> Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return Result.Failure(Error.NotFound("ChecklistItem", request.Id));

        item.Delete();
        _repo.Update(item);
        await _unitOfWork.CommitAsync(_tenant.TenantId, cancellationToken);

        return Result.Success();
    }
}
