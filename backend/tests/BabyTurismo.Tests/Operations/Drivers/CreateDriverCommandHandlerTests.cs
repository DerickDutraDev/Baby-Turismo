using Xunit;
using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Application.Operations.Drivers.Commands;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Common.ValueObjects;
using BabyTurismo.Domain.Core.Users;
using BabyTurismo.Domain.Operations.Drivers;
using BabyTurismo.Shared.Results;
using BabyTurismo.Tests.Common;
using FluentAssertions;
using Moq;

namespace BabyTurismo.Tests.Operations.Drivers;

public class CreateDriverCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IFleetNotificationService> _notificationServiceMock;
    private readonly CreateDriverCommandHandler _handler;

    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _buId = Guid.NewGuid();

    public CreateDriverCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantContextMock = new Mock<ITenantContext>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _notificationServiceMock = new Mock<IFleetNotificationService>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _tenantContextMock.Setup(t => t.BusinessUnitId).Returns(_buId);

        _handler = new CreateDriverCommandHandler(
            _userRepositoryMock.Object,
            _driverRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _passwordServiceMock.Object,
            _notificationServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateDriverAndReturnId()
    {
        var command = new CreateDriverCommand(
            "João Silva",
            TestData.Users.DriverEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            TestData.Cnh.ValidCnh,
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.DriverEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(_tenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordServiceMock.Setup(p => p.HashPassword(TestData.Users.TestPassword))
            .Returns(TestData.Users.PasswordHash);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _driverRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Driver>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _notificationServiceMock.Setup(n => n.NotifyDriverCreatedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _driverRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Driver>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnFailure()
    {
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var existingUser = User.CreateAdminUser(_tenantId, _orgId, _buId, "Existing User", emailResult.Value, "hash", UserRole.Driver);
        var existingDriver = Driver.Create(_tenantId, _orgId, _buId, existingUser.Id, TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.AdminEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _driverRepositoryMock.Setup(r => r.GetByUserIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDriver);

        var command = new CreateDriverCommand(
            "New Driver",
            TestData.Users.AdminEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.AnotherValidCpf,
            TestData.Cnh.AnotherValidCnh,
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Driver.EmailAlreadyExists");
    }

    [Fact]
    public async Task Handle_WithExistingCpf_ShouldReturnFailure()
    {
        var cpfHash = TestData.Cpf.CpfHash;
        var emailResult = Email.Create(TestData.Users.DriverEmail);
        var existingUser = User.CreateDriverUser(_tenantId, _orgId, _buId, "Existing Driver", emailResult.Value, "hash", cpfHash, "901");
        var existingDriver = Driver.Create(_tenantId, _orgId, _buId, existingUser.Id, TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("new@fleetos.local", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(_tenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _driverRepositoryMock.Setup(r => r.GetByUserIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDriver);

        var command = new CreateDriverCommand(
            "New Driver",
            "new@fleetos.local",
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            TestData.Cnh.AnotherValidCnh,
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Driver.CpfAlreadyExists");
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
    {
        var command = new CreateDriverCommand(
            "Driver",
            TestData.Users.InvalidEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            TestData.Cnh.ValidCnh,
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.InvalidEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(_tenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithExpiredCnh_ShouldReturnFailure()
    {
        var command = new CreateDriverCommand(
            "Driver",
            TestData.Users.DriverEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            TestData.Cnh.ValidCnh,
            "B",
            DateTime.UtcNow.AddYears(-1)
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.DriverEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(_tenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Driver.CnhExpired");
    }

    [Fact]
    public async Task Handle_WithEmptyCnhNumber_ShouldReturnFailure()
    {
        var command = new CreateDriverCommand(
            "Driver",
            TestData.Users.DriverEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            "",
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.DriverEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(_tenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Driver.CnhRequired");
    }

    [Fact]
    public async Task Handle_WithInactiveExistingDriver_ShouldAllowRecreation()
    {
        var emailResult = Email.Create(TestData.Users.AdminEmail);
        var existingUser = User.CreateDriverUser(_tenantId, _orgId, _buId, "Existing", emailResult.Value, "hash", TestData.Cpf.CpfHash, "901");
        var existingDriver = Driver.Create(_tenantId, _orgId, _buId, existingUser.Id, TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        existingDriver.UpdateStatus(DriverStatus.Inactive);

        var command = new CreateDriverCommand(
            "New Driver",
            TestData.Users.AdminEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            TestData.Cnh.AnotherValidCnh,
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(TestData.Users.AdminEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _driverRepositoryMock.Setup(r => r.GetByUserIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDriver);
        _passwordServiceMock.Setup(p => p.HashPassword(TestData.Users.TestPassword))
            .Returns(TestData.Users.PasswordHash);
        _unitOfWorkMock.Setup(u => u.CommitAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldNotifyDriverCreation()
    {
        var command = new CreateDriverCommand(
            "João Silva",
            TestData.Users.DriverEmail,
            TestData.Users.TestPassword,
            TestData.Cpf.ValidCpf,
            TestData.Cnh.ValidCnh,
            "B",
            DateTime.UtcNow.AddYears(5)
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByCpfHashAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns(TestData.Users.PasswordHash);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _notificationServiceMock.Verify(n => n.NotifyDriverCreatedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
