using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Core.Users;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ITenantContext tenantContext)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(tenantContext.UserId, cancellationToken);
        if (user == null)
            return Result.Failure(Error.Auth.InvalidCredentials);

        user.RevokeRefreshToken(request.RefreshToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
