using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Operations.Checklists;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Queries;

internal sealed class GetChecklistItemsQueryHandler : IRequestHandler<GetChecklistItemsQuery, Result<IReadOnlyList<ChecklistItemDto>>>
{
    private readonly IRepository<ChecklistItem> _repo;

    public GetChecklistItemsQueryHandler(IRepository<ChecklistItem> repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<ChecklistItemDto>>> Handle(GetChecklistItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repo.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<ChecklistItemDto>>(
            items.OrderBy(i => i.SortOrder).Select(i => new ChecklistItemDto(
                i.Id, i.Title, i.IsActive, i.SortOrder)).ToList());
    }
}
