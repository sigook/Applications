using Covenant.Common.Configuration;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Worker.Agency;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Extensions;
using Covenant.Infrastructure.Repositories.Candidates;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace Covenant.Integration.Tests.Infrastructure;

public class SearchExtensionsTest
{
    private static CovenantContext CreateContext(string connectionString) =>
        new(new DbContextOptionsBuilder<CovenantContext>().UseNpgsql(connectionString).Options);

    private static async Task<(CovenantContext Context, Guid AgencyId, Dictionary<string, Guid> Workers)> Seed()
    {
        var context = CreateContext(PostgresTestDatabase.CreateDatabase());
        var agency = FakeData.FakeAgency();
        var workers = new Dictionary<string, Guid>();
        WorkerProfile AddWorker(string key, string firstName, string middleName, string lastName, string email)
        {
            var worker = FakeData.FakeWorkerProfile(agency, email);
            worker.FirstName = firstName;
            worker.MiddleName = middleName;
            worker.LastName = lastName;
            workers[key] = worker.Id;
            return worker;
        }
        context.AddRange(
            AddWorker("juan", "Juan", "Carlos", "Pérez", "jcp@test.com"),
            AddWorker("helene", "Hélène", null, "Tremblay", "ht@test.com"),
            AddWorker("jonathan", "Jonathan", null, "Smith", "john.smith@mail.com"),
            AddWorker("underscore", "Ana", null, "Lopez", "ana_lopez@test.com"));
        await context.SaveChangesAsync();
        return (context, agency.Id, workers);
    }

    private static async Task<List<Guid>> SearchDropdown(CovenantContext context, Guid agencyId, string term)
    {
        var repository = new WorkerRepository(context, Options.Create(new FilesConfiguration()));
        var result = await repository.GetWorkerProfilesDropdown([agencyId], term);
        return result.Select(r => r.WorkerProfileId).ToList();
    }

    [Theory]
    [InlineData("perez juan")]
    [InlineData("JUAN PÉREZ")]
    [InlineData("  juan   carlos  ")]
    [InlineData("jcp@test")]
    public async Task Search_MatchesEveryWordInAnyField_IgnoringOrderCaseAndAccents(string term)
    {
        var (context, agencyId, workers) = await Seed();
        await using var _ = context;

        var result = await SearchDropdown(context, agencyId, term);

        Assert.Equal([workers["juan"]], result);
    }

    [Fact]
    public async Task Search_UnaccentedTerm_MatchesAccentedName()
    {
        var (context, agencyId, workers) = await Seed();
        await using var _ = context;

        var result = await SearchDropdown(context, agencyId, "helene");

        Assert.Equal([workers["helene"]], result);
    }

    [Fact]
    public async Task Search_WordMissingFromEveryField_ReturnsNothing()
    {
        var (context, agencyId, _) = await Seed();
        await using var __ = context;

        var result = await SearchDropdown(context, agencyId, "juan smith");

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("%")]
    [InlineData("a_l")]
    public async Task Search_LikeWildcards_AreMatchedLiterally(string term)
    {
        var (context, agencyId, workers) = await Seed();
        await using var _ = context;

        var result = await SearchDropdown(context, agencyId, term);

        Assert.Equal(term == "%" ? [] : [workers["underscore"]], result);
    }

    [Theory]
    [InlineData("cb")]
    [InlineData("COUNTERBALANCE")]
    [InlineData("counter bal")]
    public async Task SkillFilter_MatchesCatalogAliasOrText(string term)
    {
        var (context, agencyId, workers) = await Seed();
        await using var _ = context;
        var worker = await context.WorkerProfiles.Include(wp => wp.Skills).SingleAsync(wp => wp.Id == workers["juan"]);
        worker.PatchSkills(["Counter Balance"]);
        var candidate = new Candidate(agencyId, "Maria Lopez");
        candidate.AddSkill("Counter Balance");
        context.Add(candidate);
        await context.SaveChangesAsync();

        var workerResult = await new WorkerRepository(context, Options.Create(new FilesConfiguration()))
            .GetAllWorkersProfile(agencyId, new GetWorkerProfileFilter { Skills = term })
            .Select(wp => wp.Id)
            .ToListAsync();
        var candidateResult = await new CandidateRepository(context, Options.Create(new FilesConfiguration()))
            .GetAllCandidates(agencyId, new GetCandidatesFilter { Skills = term })
            .Select(c => c.Id)
            .ToListAsync();

        Assert.Equal([workers["juan"]], workerResult);
        Assert.Equal([candidate.Id], candidateResult);
    }

    [Fact]
    public async Task SimilarityPredicate_ToleratesTypos()
    {
        var (context, agencyId, workers) = await Seed();
        await using var _ = context;

        var result = await context.WorkerProfiles
            .Where(wp => wp.AgencyId == agencyId)
            .Where(SearchExtensions.SimilarityPredicate<WorkerProfile>("jonathon smth", 0.4, wp => wp.FirstName + " " + wp.LastName))
            .Select(wp => wp.Id)
            .ToListAsync();

        Assert.Equal([workers["jonathan"]], result);
    }
}
