using Xunit;
using BabyTurismo.Application.Auth.Commands.RefreshToken;
using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Common.ValueObjects;
using BabyTurismo.Domain.Core.Users;
using BabyTurismo.Shared.Results;
using BabyTurismo.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BabyTurismo.Tests.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Jwt:RefreshExpiryDays"]).Returns("7");
        _configurationMock.Setup(c => c["Jwt:AccessExpiryMinutes"]).Returns("60");

        _handler = new RefreshTokenCommandHandler(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _unitOfWorkMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.RefreshToken, DateTimeOffset.UtcNow.AddDays(7));

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(TestData.Tokens.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user))
            .Returns(TestData.Tokens.AccessToken);
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns("NEW_TEST_TOKEN");
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RefreshTokenCommand(TestData.Tokens.RefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(TestData.Tokens.AccessToken);
        result.Value.RefreshToken.Should().Be("NEW_TEST_TOKEN");
    }

    [Fact]
    public async Task Handle_WithInvalidRefreshToken_ShouldReturnFailure()
    {
        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync("invalid_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new RefreshTokenCommand("invalid_token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithExpiredRefreshToken_ShouldReturnFailure()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.ExpiredToken, DateTimeOffset.UtcNow.AddDays(-1));

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(TestData.Tokens.ExpiredToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new RefreshTokenCommand(TestData.Tokens.ExpiredToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithRevokedRefreshToken_ShouldReturnFailure()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.RevokedToken, DateTimeOffset.UtcNow.AddDays(7));
        user.RevokeRefreshToken(TestData.Tokens.RevokedToken);

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(TestData.Tokens.RevokedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new RefreshTokenCommand(TestData.Tokens.RevokedToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithLockedUser_ShouldReturnUserBlocked()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.RefreshToken, DateTimeOffset.UtcNow.AddDays(7));
        user.UnlockManually();
        for (int i = 0; i < 5; i++) user.RecordFailedLogin();

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(TestData.Tokens.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new RefreshTokenCommand(TestData.Tokens.RefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.UserBlocked);
    }

    [Fact]
    public async Task Handle_WithDisabledUser_ShouldReturnUserBlocked()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.RefreshToken, DateTimeOffset.UtcNow.AddDays(7));
        user.Disable();

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(TestData.Tokens.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new RefreshTokenCommand(TestData.Tokens.RefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.UserBlocked);
    }

    [Fact]
    public async Task Handle_WithValidToken_ShouldRevokeOldTokenAndAddNewOne()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.AddRefreshToken(TestData.Tokens.RefreshToken, DateTimeOffset.UtcNow.AddDays(7));

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(TestData.Tokens.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user))
            .Returns(TestData.Tokens.AccessToken);
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns("NEW_TEST_TOKEN");
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RefreshTokenCommand(TestData.Tokens.RefreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.RefreshToken.Should().Be("NEW_TEST_TOKEN");
        result.Value.RefreshToken.Should().NotBe(TestData.Tokens.RefreshToken);
    }
}
