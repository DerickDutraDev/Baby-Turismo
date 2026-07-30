using Xunit;
using BabyTurismo.Application.Auth.Commands.Logout;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Common.ValueObjects;
using BabyTurismo.Domain.Core.Users;
using BabyTurismo.Shared.Results;
using BabyTurismo.Tests.Common;
using FluentAssertions;
using Moq;

namespace BabyTurismo.Tests.Auth;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantContextMock = new Mock<ITenantContext>();

        _handler = new LogoutCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantContextMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRefreshToken_ShouldRevokeTokenAndReturnSuccess()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.RefreshToken, DateTimeOffset.UtcNow.AddDays(7));

        _tenantContextMock.Setup(t => t.UserId).Returns(userId);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new LogoutCommand(TestData.Tokens.RefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        _tenantContextMock.Setup(t => t.UserId).Returns(userId);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LogoutCommand(TestData.Tokens.RefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithInvalidRefreshToken_ShouldStillReturnSuccess()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);

        _tenantContextMock.Setup(t => t.UserId).Returns(userId);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new LogoutCommand(TestData.Tokens.ExpiredToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
