using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Services.Shared;
using Moq;
using Xunit;

namespace Covenant.Tests.Common;

public class SkillServiceTest
{
    private readonly SkillService _sut;

    public SkillServiceTest()
    {
        var catalogRepository = new Mock<ICatalogRepository>();
        catalogRepository.Setup(r => r.GetSkillCatalog()).ReturnsAsync(
        [
            new SkillCatalogModel { Value = "General Labour", Aliases = ["general labor", "gl"] },
            new SkillCatalogModel { Value = "Counter Balance", Aliases = ["counterbalance", "cb"] },
            new SkillCatalogModel { Value = "Forklift", Aliases = [] }
        ]);
        _sut = new SkillService(catalogRepository.Object);
    }

    [Theory]
    [InlineData("general labor", "General Labour")]
    [InlineData("  GL ", "General Labour")]
    [InlineData("GENERAL   LABOUR", "General Labour")]
    [InlineData("counterbalance", "Counter Balance")]
    [InlineData("forklift", "Forklift")]
    [InlineData("Montacargas   Nocturno", "Montacargas Nocturno")]
    public async Task Canonicalize_MapsValueOrAliasToCatalogValue(string skill, string expected) =>
        Assert.Equal([expected], await _sut.Canonicalize([skill]));

    [Fact]
    public async Task Canonicalize_RemovesDuplicatesAfterMapping() =>
        Assert.Equal(["General Labour", "Forklift"], await _sut.Canonicalize(["gl", "Forklift", "general labor", "FORKLIFT"]));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("w")]
    [InlineData("N/A")]
    [InlineData(null)]
    public async Task Canonicalize_DropsMeaninglessValues(string skill) =>
        Assert.Empty(await _sut.Canonicalize([skill]));

    [Fact]
    public async Task Canonicalize_NullCollection_ReturnsEmpty() =>
        Assert.Empty(await _sut.Canonicalize(null));
}
