using Xunit;
using BabyTurismo.Application.Auth.Commands.Login;
using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Common.ValueObjects;
using BabyTurismo.Domain.Core.Tenants;
using BabyTurismo.Domain.Core.Users;
using BabyTurismo.Shared.Results;
using BabyTurismo.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BabyTurismo.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Jwt:RefreshExpiryDays"]).Returns("7");
        _configurationMock.Setup(c => c["Jwt:AccessExpiryMinutes"]).Returns("60");

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _passwordServiceMock.Object,
            _jwtServiceMock.Object,
            _unitOfWorkMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidEmailAndPassword_ShouldReturnSuccess()
    {
        var tenantId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var buId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, orgId, buId, "Admin User", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.AdminEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(p => p.VerifyPassword(TestData.Users.TestPassword, TestData.Users.PasswordHash))
            .Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user))
            .Returns(TestData.Tokens.AccessToken);
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns(TestData.Tokens.RefreshToken);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new LoginCommand(TestData.Users.AdminEmail, TestData.Users.TestPassword, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(TestData.Tokens.AccessToken);
        result.Value.RefreshToken.Should().Be(TestData.Tokens.RefreshToken);
        result.Value.User.Email.Should().Be(TestData.Users.AdminEmail);
        result.Value.User.Name.Should().Be("Admin User");
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("invalid@babyturismo.local", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand("invalid@babyturismo.local", TestData.Users.TestPassword, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldReturnFailure()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.AdminEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(p => p.VerifyPassword(TestData.Users.WrongPassword, TestData.Users.PasswordHash))
            .Returns(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new LoginCommand(TestData.Users.AdminEmail, TestData.Users.WrongPassword, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithLockedUser_ShouldReturnUserBlocked()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create("locked@babyturismo.local");
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Locked User", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);
        user.UnlockManually();
        for (int i = 0; i < 5; i++) user.RecordFailedLogin();

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("locked@babyturismo.local", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LoginCommand("locked@babyturismo.local", TestData.Users.TestPassword, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.UserBlocked);
    }

    [Fact]
    public async Task Handle_WithCpfLogin_WithoutTenantSlug_ShouldReturnFailure()
    {
        var command = new LoginCommand(TestData.Cpf.ValidCpf, TestData.Users.TestPassword, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithCpfLogin_WithInvalidTenantSlug_ShouldReturnFailure()
    {
        _tenantRepositoryMock.Setup(t => t.GetBySlugAsync("invalid-slug", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        var command = new LoginCommand(TestData.Cpf.ValidCpf, TestData.Users.TestPassword, "invalid-slug");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithValidCpfLogin_ShouldReturnSuccess()
    {
        var tenantId = Guid.NewGuid();
        var tenant = Tenant.Create(TestData.Tenants.TenantName, TestData.Tenants.TenantSlug);
        var emailResult = Email.Create(TestData.Users.DriverEmail);
        var user = User.CreateDriverUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Driver", emailResult.Value, TestData.Users.PasswordHash, TestData.Cpf.CpfHash, TestData.Cpf.CpfLast4);

        _tenantRepositoryMock.Setup(t => t.GetBySlugAsync(TestData.Tenants.TenantSlug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(tenant.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(p => p.VerifyPassword(TestData.Users.TestPassword, TestData.Users.PasswordHash))
            .Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user))
            .Returns(TestData.Tokens.AccessToken);
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns(TestData.Tokens.RefreshToken);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new LoginCommand(TestData.Cpf.ValidCpf, TestData.Users.TestPassword, TestData.Tenants.TenantSlug);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.User.IsDriverAccount.Should().BeTrue();
        result.Value.AccessToken.Should().Be(TestData.Tokens.AccessToken);
    }

    [Fact]
    public async Task Handle_WithFiveFailedAttempts_ShouldLockUserAndReturnBlocked()
    {
        var tenantId = Guid.NewGuid();
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var user = User.CreateAdminUser(tenantId, Guid.NewGuid(), Guid.NewGuid(), "Admin", emailResult.Value, TestData.Users.PasswordHash, UserRole.TenantAdmin);

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.AdminEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(p => p.VerifyPassword(TestData.Users.WrongPassword, TestData.Users.PasswordHash))
            .Returns(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        for (int i = 0; i < 4; i++)
        {
            var cmd = new LoginCommand(TestData.Users.AdminEmail, TestData.Users.WrongPassword, null);
            await _handler.Handle(cmd, CancellationToken.None);
        }

        var finalCommand = new LoginCommand(TestData.Users.AdminEmail, TestData.Users.WrongPassword, null);
        var result = await _handler.Handle(finalCommand, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Auth.UserBlocked);
    }
}
