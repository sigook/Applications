using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Locations;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Services.Locations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Covenant.Tests.Locations;

public class LocationServiceTest
{
    private readonly Mock<ICatalogRepository> _catalog = new();
    private readonly Mock<ILocationRepository> _locations = new();
    private readonly LocationService _sut;

    public LocationServiceTest() =>
        _sut = new LocationService(_catalog.Object, _locations.Object, NullLogger<LocationService>.Instance);

    [Fact]
    public async Task AddCity_UpperCasesTheNameAndSavesIt()
    {
        var provinceId = Guid.NewGuid();
        City created = null;
        _catalog.Setup(c => c.Create(It.IsAny<City>())).Callback<City>(c => created = c).Returns(Task.CompletedTask);

        Result<Guid> result = await _sut.AddCity(new CityModel { Value = "Kingston", Province = new ProvinceModel { Id = provinceId } });

        Assert.True(result);
        Assert.Equal("KINGSTON", created.Value);
        Assert.Equal(provinceId, created.ProvinceId);
        Assert.Equal(created.Id, result.Value);
        _catalog.Verify(c => c.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task AddCity_WithoutName_Fails(string name)
    {
        Result<Guid> result = await _sut.AddCity(new CityModel { Value = name, Province = new ProvinceModel { Id = Guid.NewGuid() } });

        Assert.False(result);
        _catalog.Verify(c => c.Create(It.IsAny<City>()), Times.Never);
    }

    [Fact]
    public async Task AddCity_NullModelOrEmptyProvince_Fails()
    {
        Assert.False(await _sut.AddCity(null));
        Assert.False(await _sut.AddCity(new CityModel { Value = "Kingston", Province = new ProvinceModel { Id = Guid.Empty } }));
    }

    [Fact]
    public async Task AddCity_StorageFailure_IsReportedAsAFailedResult()
    {
        _catalog.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("db down"));

        Result<Guid> result = await _sut.AddCity(new CityModel { Value = "Kingston", Province = new ProvinceModel { Id = Guid.NewGuid() } });

        Assert.False(result);
        Assert.Contains("An error occurred", result.StringErrors);
    }

    [Fact]
    public async Task UpsertProvinceSettings_ConvertsOvertimeHoursToTimeSpan()
    {
        var provinceId = Guid.NewGuid();
        ProvinceSetting saved = null;
        _catalog.Setup(c => c.UpsertProvinceSettings(It.IsAny<ProvinceSetting>())).Callback<ProvinceSetting>(s => saved = s).Returns(Task.CompletedTask);

        Result result = await _sut.UpsertProvinceSettings(provinceId, new ProvinceSettingsModel { PaidHolidays = true, OvertimeStartsAfter = 40.5 });

        Assert.True(result);
        Assert.Equal(provinceId, saved.ProvinceId);
        Assert.True(saved.PaidHolidays);
        Assert.Equal(TimeSpan.FromHours(40.5), saved.OvertimeStartsAfter);
    }

    [Fact]
    public async Task UpsertProvinceSettings_InvalidInput_Fails()
    {
        Assert.False(await _sut.UpsertProvinceSettings(Guid.Empty, new ProvinceSettingsModel()));
        Assert.False(await _sut.UpsertProvinceSettings(Guid.NewGuid(), null));
        _catalog.Verify(c => c.UpsertProvinceSettings(It.IsAny<ProvinceSetting>()), Times.Never);
    }

    [Fact]
    public async Task GetLocationTax_ReturnsThePercentage()
    {
        var locationId = Guid.NewGuid();
        _locations.Setup(l => l.GetLocationTax(locationId)).ReturnsAsync(new LocationTaxModel { Tax1 = 0.13m });

        LocationTaxModel tax = await _sut.GetLocationTax(locationId);

        Assert.Equal(13m, tax.Tax1);
    }

    [Fact]
    public async Task UpsertLocationTax_StoresAFraction()
    {
        var locationId = Guid.NewGuid();

        Result result = await _sut.UpsertLocationTax(locationId, new LocationTaxModel { Tax1 = 13m });

        Assert.True(result);
        _locations.Verify(l => l.UpsertLocationTax(locationId, 0.13m), Times.Once);
        _locations.Verify(l => l.SaveChangesAsync(), Times.Once);
    }
}
