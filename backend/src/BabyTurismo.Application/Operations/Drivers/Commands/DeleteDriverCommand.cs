using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Commands;

public sealed record DeleteDriverCommand(Guid Id) : IRequest<Result>;
