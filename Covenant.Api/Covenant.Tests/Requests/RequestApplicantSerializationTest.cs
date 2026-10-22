using System.Text.Json;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Xunit;

namespace Covenant.Tests.Requests;

public class RequestApplicantSerializationTest
{
    [Fact]
    public void RoundTripsThroughServiceBusJson()
    {
        var applicant = RequestApplicant.CreateWithCandidate(Guid.NewGuid(), Guid.NewGuid(), "Sigook", "comments", RequestApplicantStatus.Pending).Value;

        var copy = BinaryData.FromString(JsonSerializer.Serialize(applicant)).ToObjectFromJson<RequestApplicant>();

        Assert.Equal(applicant.Id, copy.Id);
        Assert.Equal(applicant.RequestId, copy.RequestId);
        Assert.Equal(applicant.CandidateId, copy.CandidateId);
        Assert.Equal(applicant.WorkerProfileId, copy.WorkerProfileId);
        Assert.Equal(applicant.CreatedBy, copy.CreatedBy);
        Assert.Equal(applicant.Comments, copy.Comments);
        Assert.Equal(applicant.Status, copy.Status);
        Assert.Equal(applicant.CreatedAt, copy.CreatedAt);
    }
}
