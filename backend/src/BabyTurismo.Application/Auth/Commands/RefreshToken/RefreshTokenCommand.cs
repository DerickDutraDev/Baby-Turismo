using BabyTurismo.Application.Auth.Commands.Login;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token) : IRequest<Result<LoginResponse>>;
