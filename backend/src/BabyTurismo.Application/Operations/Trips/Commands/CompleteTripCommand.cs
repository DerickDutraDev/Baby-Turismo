using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Commands;

public sealed record CompleteTripCommand(
    Guid Id,
    bool ChecklistCompleted,
    string? ChecklistNotes) : IRequest<Result>;
