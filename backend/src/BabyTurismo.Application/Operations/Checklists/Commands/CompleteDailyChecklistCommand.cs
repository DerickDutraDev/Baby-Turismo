using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Commands;

public sealed record CompleteDailyChecklistCommand(
    Guid VehicleId,
    IReadOnlyList<Guid> ChecklistItemIds) : IRequest<Result>;
