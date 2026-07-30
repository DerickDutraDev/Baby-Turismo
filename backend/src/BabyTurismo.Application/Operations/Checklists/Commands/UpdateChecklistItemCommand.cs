using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Commands;

public sealed record UpdateChecklistItemCommand(
    Guid Id,
    string Title,
    bool IsActive) : IRequest<Result>;
