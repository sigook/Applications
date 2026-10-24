using Covenant.Common.Entities.Companies;
using CsvHelper.Configuration.Attributes;

namespace Covenant.Common.Models.Companies.Agencies;

public class CompanyCsvModel
{
    [Name("Company Name")]
    public string CompanyName { get; set; }

    [Name("Website")]
    public string Website { get; set; }

    [Name("Company HQ Phone")]
    public string CompanyHQPhone { get; set; }

    [Name("Fax")]
    public string Fax { get; set; }

    [Name("Primary Industry")]
    public string PrimaryIndustry { get; set; }

    [Name("Company City")]
    public string CompanyCity { get; set; }

    [Name("Company State")]
    public string CompanyState { get; set; }

    [Name("Company Zip Code")]
    public string CompanyZipCode { get; set; }

    [Name("Company Country")]
    public string CompanyCountry { get; set; }

    [Name("Full Address")]
    public string FullAddress { get; set; }

    [Name("Contact Name")]
    public string ContactName { get; set; }

    [Name("Contact email")]
    public string ContactEmail { get; set; }

    [Name("Title")]
    public string Title { get; set; }
}

public class BulkCompany
{
    public CompanyProfile CompanyProfile { get; set; }
    public CompanyProfileContactPerson ContactPerson { get; set; }
    public CompanyProfileLocation CompanyLocation { get; set; }
}