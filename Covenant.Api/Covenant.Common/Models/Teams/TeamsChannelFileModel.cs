namespace Covenant.Common.Models.Teams;

public record TeamsChannelFileModel(string Name, string WebUrl, long? Size, DateTimeOffset? LastModified, string LastModifiedBy);
