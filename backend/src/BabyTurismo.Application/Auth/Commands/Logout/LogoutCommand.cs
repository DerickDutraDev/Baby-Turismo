using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
