using System.Security.Claims;

namespace Covenant.Common.Constants;

public static class IdentityClaims
{
    public const string Subject = "sub";
    public const string Nickname = "nickname";
    public const string IdentityProvider = "idp";
    public const string LocalIdentityProvider = "local";
    public const string MicrosoftObjectId = "microsoft_oid";
    public const string MicrosoftObjectIdExternalClaim = "oid";

    public static Claim CompanyId(Guid id) => new(CovenantConstants.CompanyId, id.ToString());
    public static Claim AgencyId(Guid id) => new(CovenantConstants.AgencyId, id.ToString());
    public static Claim MicrosoftObject(string objectId) => new(MicrosoftObjectId, objectId);
}
