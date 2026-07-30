namespace BabyTurismo.Tests.Common;

public static class TestData
{
    public static class Users
    {
        public const string AdminEmail = "test.admin@babyturismo.local";
        public const string ManagerEmail = "test.manager@babyturismo.local";
        public const string DriverEmail = "test.driver@babyturismo.local";
        public const string InvalidEmail = "invalid-email-format";
        public const string PasswordHash = "TEST_HASHED_PASSWORD_12345";
        public const string TestPassword = "Test@123456";
        public const string WrongPassword = "Wrong@Password789";
    }

    public static class Cpf
    {
        public const string ValidCpf = "00000000000";
        public const string AnotherValidCpf = "11111111111";
        public const string CpfLast4 = "0000";
        public const string CpfHash = "TEST_CPF_HASH_VALUE";
    }

    public static class Cnh
    {
        public const string ValidCnh = "00000000000";
        public const string AnotherValidCnh = "11111111111";
        public const string CnhCategory = "B";
    }

    public static class Vehicles
    {
        public const string LicensePlate = "TEST001";
        public const string AnotherLicensePlate = "TEST002";
        public const string Chassi = "TESTCHASSI00001";
        public const string Nickname = "Test Vehicle";
        public const string Brand = "TestBrand";
        public const string Model = "TestModel";
    }

    public static class Trips
    {
        public const string Origin = "Test City A";
        public const string Destination = "Test City B";
        public const decimal TripValue = 1000.00m;
    }

    public static class Tenants
    {
        public const string TenantName = "Test Company";
        public const string TenantSlug = "test-company";
    }

    public static class Tokens
    {
        public const string AccessToken = "TEST_ACCESS_TOKEN";
        public const string RefreshToken = "TEST_REFRESH_TOKEN";
        public const string ExpiredToken = "EXPIRED_TEST_TOKEN";
        public const string RevokedToken = "REVOKED_TEST_TOKEN";
    }
}
