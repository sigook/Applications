using Covenant.Common.Enums;
using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Accounting.Deductions;

public class ImportCraTableFromBlobModel
{
    public string BlobName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PayPeriod PayPeriod { get; set; }

    public int Year { get; set; }
}
