using Covenant.Common.Entities.Request;
using CsvHelper.Configuration.Attributes;

namespace Covenant.Common.Models.Candidate;

public class CandidateCsvModel
{
    [Name("Full Name")]
    public string FullName { get; set; }
    [Name("Email")]
    public string Email { get; set; }
    [Name("Phone")]
    public string Phone { get; set; }
    [Name("Address")]
    public string Address { get; set; }
    [Name("Skills")]
    public string Skills { get; set; }
    [Name("Status")]
    public string Status { get; set; }
    [Name("Gender")]
    public string Gender { get; set; }
    [Name("Has Transportation?")]
    public string HasTransportation { get; set; }
    [Name("Order ID")]
    public string OrderID { get; set; }
    [Name("Source")]
    public string Source { get; set; }
    [Name("URL Resume")]
    public string UrlResume { get; set; }
}

public class BulkCandidate
{
    public Entities.Candidate.Candidate Candidate { get; set; }
    public Entities.Candidate.CandidatePhone CandidatesPhone { get; set; }
    public List<Entities.Candidate.CandidateSkill> CandidatesSkills { get; set; } = new List<Entities.Candidate.CandidateSkill>();
    public Entities.Candidate.CandidateDocument CandidatesDocument { get; set; }
    public RequestApplicant RequestApplicant { get; set; }
}
