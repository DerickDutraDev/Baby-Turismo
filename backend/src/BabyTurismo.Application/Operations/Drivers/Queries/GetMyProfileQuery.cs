using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Queries;

public sealed record GetMyProfileQuery : IRequest<Result<DriverDto>>;
