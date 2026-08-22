using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Xunit;

namespace Covenant.Tests.Request;

public class RequestApplicantTest
{
    private static RequestApplicant CreateWorkerApplicant(RequestApplicantStatus status) =>
        RequestApplicant.CreateWithWorker(Guid.NewGuid(), Guid.NewGuid(), "tester", null, status).Value;

    private static RequestApplicant CreateCandidateApplicant(RequestApplicantStatus status) =>
        RequestApplicant.CreateWithCandidate(Guid.NewGuid(), Guid.NewGuid(), "tester", null, status).Value;

    [Fact]
    public void CreateWithWorkerSetsStatus()
    {
        var applicant = CreateWorkerApplicant(RequestApplicantStatus.InProgress);
        Assert.Equal(RequestApplicantStatus.InProgress, applicant.Status);
    }

    [Fact]
    public void MoveToInProgressFromPending()
    {
        var applicant = CreateWorkerApplicant(RequestApplicantStatus.Pending);
        var result = applicant.MoveToInProgress();
        Assert.True(result);
        Assert.Equal(RequestApplicantStatus.InProgress, applicant.Status);
    }

    [Fact]
    public void MoveToInProgressFromCancelledReopens()
    {
        var applicant = CreateWorkerApplicant(RequestApplicantStatus.Cancelled);
        var result = applicant.MoveToInProgress();
        Assert.True(result);
        Assert.Equal(RequestApplicantStatus.InProgress, applicant.Status);
    }

    [Fact]
    public void MoveToInProgressFailsFromConfirmed()
    {
        var applicant = CreateWorkerApplicant(RequestApplicantStatus.Confirmed);
        var result = applicant.MoveToInProgress();
        Assert.False(result);
        Assert.Equal(RequestApplicantStatus.Confirmed, applicant.Status);
    }

    [Theory]
    [InlineData(RequestApplicantStatus.Pending)]
    [InlineData(RequestApplicantStatus.InProgress)]
    public void CancelFromPendingOrInProgress(RequestApplicantStatus status)
    {
        var applicant = CreateWorkerApplicant(status);
        var result = applicant.Cancel();
        Assert.True(result);
        Assert.Equal(RequestApplicantStatus.Cancelled, applicant.Status);
    }

    [Theory]
    [InlineData(RequestApplicantStatus.Confirmed)]
    [InlineData(RequestApplicantStatus.Cancelled)]
    public void CancelFailsFromConfirmedOrCancelled(RequestApplicantStatus status)
    {
        var applicant = CreateWorkerApplicant(status);
        var result = applicant.Cancel();
        Assert.False(result);
        Assert.Equal(status, applicant.Status);
    }

    [Fact]
    public void ConfirmFromInProgress()
    {
        var applicant = CreateWorkerApplicant(RequestApplicantStatus.InProgress);
        var result = applicant.Confirm();
        Assert.True(result);
        Assert.Equal(RequestApplicantStatus.Confirmed, applicant.Status);
    }

    [Fact]
    public void ConfirmFailsFromPending()
    {
        var applicant = CreateWorkerApplicant(RequestApplicantStatus.Pending);
        var result = applicant.Confirm();
        Assert.False(result);
        Assert.Equal(RequestApplicantStatus.Pending, applicant.Status);
    }

    [Fact]
    public void ConfirmFailsForCandidate()
    {
        var applicant = CreateCandidateApplicant(RequestApplicantStatus.InProgress);
        var result = applicant.Confirm();
        Assert.False(result);
        Assert.Equal(RequestApplicantStatus.InProgress, applicant.Status);
    }
}
