using System.Globalization;
using System.Xml.Linq;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet run coverage-gate.cs <cobertura.xml> <minLineCoverage>");
    return 2;
}

var report = XDocument.Load(args[0]).Root!;
var minimum = double.Parse(args[1], CultureInfo.InvariantCulture);
var covered = double.Parse(report.Attribute("lines-covered")!.Value, CultureInfo.InvariantCulture);
var valid = double.Parse(report.Attribute("lines-valid")!.Value, CultureInfo.InvariantCulture);
var coverage = valid == 0 ? 0 : Math.Round(covered / valid * 100, 2);

Console.WriteLine($"Line coverage: {coverage.ToString(CultureInfo.InvariantCulture)}% ({covered}/{valid}), minimum {minimum.ToString(CultureInfo.InvariantCulture)}%");

if (coverage < minimum)
{
    Console.WriteLine($"##vso[task.logissue type=error]Line coverage {coverage.ToString(CultureInfo.InvariantCulture)}% is below the required {minimum.ToString(CultureInfo.InvariantCulture)}%");
    return 1;
}

return 0;
