using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Queries;

public sealed record GetChecklistItemsQuery : IRequest<Result<IReadOnlyList<ChecklistItemDto>>>;
