using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities.Requests;
using CsvHelper.Configuration.Attributes;

namespace Covenant.Common.Models.Candidates;

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
    [Name("Request ID")]
    public string RequestID { get; set; }
    [Name("Source")]
    public string Source { get; set; }
    [Name("URL Resume")]
    public string UrlResume { get; set; }
}

public class BulkCandidate
{
    public Candidate Candidate { get; set; }
    public CandidatePhone CandidatesPhone { get; set; }
    public List<CandidateSkill> CandidatesSkills { get; set; } = new List<CandidateSkill>();
    public CandidateDocument CandidatesDocument { get; set; }
    public RequestApplicant RequestApplicant { get; set; }
}
