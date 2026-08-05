using System.Text.Json.Serialization;
using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Deductions;

public class ImportCraTableFromBlobModel
{
    public string BlobName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PayPeriod PayPeriod { get; set; }

    public int Year { get; set; }
}
