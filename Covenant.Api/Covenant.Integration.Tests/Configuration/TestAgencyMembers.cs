using System.Collections.Concurrent;
using Covenant.Common.Models.Identity;

namespace Covenant.Integration.Tests.Configuration;

public static class TestAgencyMembers
{
    private static readonly ConcurrentDictionary<AgencyMemberModel, byte> Members = new();

    public static AgencyMemberModel Add(Guid agencyId, Guid userId)
    {
        AgencyMemberModel member = new(userId, agencyId);
        Members.TryAdd(member, 0);
        return member;
    }

    public static void Remove(Guid agencyId, Guid userId) => Members.TryRemove(new AgencyMemberModel(userId, agencyId), out _);

    public static IReadOnlyList<AgencyMemberModel> OfAgencies(IEnumerable<Guid> agencyIds)
    {
        var ids = agencyIds.ToHashSet();
        return [.. Members.Keys.Where(m => ids.Contains(m.AgencyId))];
    }

    public static IReadOnlyList<AgencyMemberModel> OfUsers(IEnumerable<Guid> userIds)
    {
        var ids = userIds.ToHashSet();
        return [.. Members.Keys.Where(m => ids.Contains(m.UserId))];
    }
}
