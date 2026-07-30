using Xunit;
using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Application.Operations.Trips.Commands;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Finance;
using BabyTurismo.Domain.Fleet.Vehicles;
using BabyTurismo.Domain.Operations.Drivers;
using BabyTurismo.Domain.Operations.Trips;
using BabyTurismo.Shared.Results;
using BabyTurismo.Tests.Common;
using FluentAssertions;
using Moq;

namespace BabyTurismo.Tests.Operations.Trips;

public class CreateTripCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IFinancialTransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IFinancialCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IFinancialMonthRepository> _monthRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IFleetNotificationService> _notificationServiceMock;
    private readonly CreateTripCommandHandler _handler;

    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _buId = Guid.NewGuid();

    public CreateTripCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _transactionRepositoryMock = new Mock<IFinancialTransactionRepository>();
        _categoryRepositoryMock = new Mock<IFinancialCategoryRepository>();
        _monthRepositoryMock = new Mock<IFinancialMonthRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantContextMock = new Mock<ITenantContext>();
        _notificationServiceMock = new Mock<IFleetNotificationService>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _tenantContextMock.Setup(t => t.BusinessUnitId).Returns(_buId);

        _handler = new CreateTripCommandHandler(
            _tripRepositoryMock.Object,
            _driverRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _transactionRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _monthRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _notificationServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateTripAndReturnId()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _tripRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Pending, "Trip notes"
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _tripRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentDriver_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Driver?)null);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Pending, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Driver.NotFound");
    }

    [Fact]
    public async Task Handle_WithNonExistentVehicle_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Pending, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Vehicle.NotFound");
    }

    [Fact]
    public async Task Handle_WithEmptyOrigin_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var command = new CreateTripCommand(
            driverId, vehicleId, "", TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Pending, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Trip.OriginRequired");
    }

    [Fact]
    public async Task Handle_WithEmptyDestination_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, "",
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Pending, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Trip.DestinationRequired");
    }

    [Fact]
    public async Task Handle_WithInvalidDates_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(1),
            TestData.Trips.TripValue, PaymentStatus.Pending, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Trip.InvalidDates");
    }

    [Fact]
    public async Task Handle_WithNegativeValue_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            -100.00m, PaymentStatus.Pending, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Trip.InvalidValue");
    }

    [Fact]
    public async Task Handle_WithPaidTrip_ShouldCreateFinancialTransaction()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;
        var month = FinancialMonth.Open(_tenantId, _orgId, _buId, 2024, 1, 0);
        var category = FinancialCategory.Create(_tenantId, _orgId, _buId, "Viagens", TransactionType.Revenue).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _tripRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _monthRepositoryMock.Setup(r => r.GetOpenMonthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(month);
        _categoryRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinancialCategory> { category });
        _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<FinancialTransaction>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Paid, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _transactionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FinancialTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPaidTripAndNoOpenMonth_ShouldReturnFailure()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _tripRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _monthRepositoryMock.Setup(r => r.GetOpenMonthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((FinancialMonth?)null);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Paid, null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Month.NoOpenMonth");
    }

    [Fact]
    public async Task Handle_ShouldNotifyTripCreation()
    {
        var driverId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driver = Driver.Create(_tenantId, _orgId, _buId, Guid.NewGuid(), TestData.Cnh.ValidCnh, "B", DateTime.UtcNow.AddYears(5)).Value!;
        var vehicle = Vehicle.Create(_tenantId, _orgId, _buId, TestData.Vehicles.LicensePlate, null, "Vehicle", null, null, null, null, null, null, null).Value!;

        _driverRepositoryMock.Setup(r => r.GetByIdAsync(driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _tripRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateTripCommand(
            driverId, vehicleId, TestData.Trips.Origin, TestData.Trips.Destination,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
            TestData.Trips.TripValue, PaymentStatus.Pending, null
        );

        await _handler.Handle(command, CancellationToken.None);

        _notificationServiceMock.Verify(n => n.NotifyTripCreatedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
