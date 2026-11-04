using Covenant.Common.Functionals;
using System.Text.RegularExpressions;

namespace Covenant.Common.Entities;

public partial class CvnAddress
{
    private CvnAddress(string address) => Address = address;
    public string Address { get; }
    public static Result<CvnAddress> Create(string address)
    {
        const string addressDoesNotHaveTheCorrectFormat = "Address does not have the correct format";
        if (string.IsNullOrEmpty(address)) return Result.Fail<CvnAddress>(addressDoesNotHaveTheCorrectFormat);
        bool isValidAddress = AddressRegex().IsMatch(address);
        if (!isValidAddress) return Result.Fail<CvnAddress>(addressDoesNotHaveTheCorrectFormat);
        return Result.Ok(new CvnAddress(address));
    }

    [GeneratedRegex("^[-.#, a-zA-Z0-9]+$")]
    private static partial Regex AddressRegex();
}
