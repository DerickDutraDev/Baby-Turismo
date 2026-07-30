using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Commands;

public sealed record CreateChecklistItemCommand(
    string Title) : IRequest<Result<Guid>>;
