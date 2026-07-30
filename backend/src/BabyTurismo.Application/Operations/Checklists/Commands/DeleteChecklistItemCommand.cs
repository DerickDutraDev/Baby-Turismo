using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Commands;

public sealed record DeleteChecklistItemCommand(Guid Id) : IRequest<Result>;
