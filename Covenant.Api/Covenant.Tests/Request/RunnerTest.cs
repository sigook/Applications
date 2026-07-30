using System;
using System.Linq;
using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Enums;
using Xunit;

namespace Covenant.Tests.Request
{
    public class RunnerTest
    {
        private static readonly Guid Actor = Guid.NewGuid();

        private static Runner NewRunner() =>
            Runner.CreateFromWorker(Guid.NewGuid(), Guid.NewGuid(), RunnerType.Active, Actor).Value;

        [Fact]
        public void Create_StartsAtSentToClient_WithInitialHistory()
        {
            var runner = NewRunner();
            Assert.Equal(RunnerStatus.SentToClient, runner.Status);
            var history = Assert.Single(runner.StatusHistory);
            Assert.Null(history.PreviousStatus);
            Assert.Equal(RunnerStatus.SentToClient, history.NewStatus);
        }

        [Theory]
        [InlineData(RunnerStatus.InterviewScheduled, true)]
        [InlineData(RunnerStatus.InterviewRescheduled, true)]
        [InlineData(RunnerStatus.SentToClient, false)]
        [InlineData(RunnerStatus.WaitingForFinalDecision, false)]
        [InlineData(RunnerStatus.Hired, false)]
        public void CanAddInterview_OnlyForScheduledAndRescheduled(RunnerStatus status, bool expected)
        {
            Assert.Equal(expected, Runner.CanAddInterview(status));
        }

        [Fact]
        public void ChangeStatus_AppendsHistory_WithPreviousAndNew()
        {
            var runner = NewRunner();
            var result = runner.ChangeStatus(RunnerStatus.InterviewScheduled, Actor, "scheduled it");
            Assert.True(result);
            Assert.Equal(RunnerStatus.InterviewScheduled, runner.Status);
            Assert.Equal(2, runner.StatusHistory.Count());
            var last = runner.StatusHistory.Last();
            Assert.Equal(RunnerStatus.SentToClient, last.PreviousStatus);
            Assert.Equal(RunnerStatus.InterviewScheduled, last.NewStatus);
            Assert.Equal("scheduled it", last.Comments);
        }

        [Fact]
        public void ChangeStatus_ToHired_SetsStartDate()
        {
            var runner = NewRunner();
            var startDate = new DateTime(2026, 7, 1);
            var result = runner.ChangeStatus(RunnerStatus.Hired, Actor, startDate: startDate);
            Assert.True(result);
            Assert.Equal(RunnerStatus.Hired, runner.Status);
            Assert.Equal(startDate, runner.StartDate);
        }

        [Fact]
        public void ChangeStatus_ToHired_WithoutStartDate_IsBlocked()
        {
            var runner = NewRunner();
            var result = runner.ChangeStatus(RunnerStatus.Hired, Actor);
            Assert.False(result);
            Assert.NotEqual(RunnerStatus.Hired, runner.Status);
            Assert.Null(runner.StartDate);
        }

        [Fact]
        public void ChangeStatus_WhenAlreadyHired_IsBlocked()
        {
            var runner = NewRunner();
            runner.ChangeStatus(RunnerStatus.Hired, Actor, startDate: DateTime.Now);
            var result = runner.ChangeStatus(RunnerStatus.Rejected, Actor);
            Assert.False(result);
            Assert.Equal(RunnerStatus.Hired, runner.Status);
        }

        [Fact]
        public void AddInterview_BlockedWhenStatusDoesNotAllow()
        {
            var runner = NewRunner();
            var result = runner.AddInterview(DateTime.Now, InterviewType.Phone, "Jane", "notes", Actor);
            Assert.False(result);
            Assert.Empty(runner.Interviews);
        }

        [Fact]
        public void AddInterview_AllowedWhenScheduled()
        {
            var runner = NewRunner();
            runner.ChangeStatus(RunnerStatus.InterviewScheduled, Actor);
            var result = runner.AddInterview(DateTime.Now, InterviewType.Onsite, "Jane", "notes", Actor);
            Assert.True(result);
            Assert.Single(runner.Interviews);
            Assert.Equal(InterviewStatus.Scheduled, result.Value.Status);
        }

        [Fact]
        public void RescheduleInterview_TransitionsToRescheduled()
        {
            var runner = NewRunner();
            runner.ChangeStatus(RunnerStatus.InterviewScheduled, Actor);
            var interview = runner.AddInterview(DateTime.Now, InterviewType.Video, "Jane", null, Actor).Value;
            var result = runner.RescheduleInterview(interview.Id, DateTime.Now.AddDays(2), Actor);
            Assert.True(result);
            Assert.Equal(RunnerStatus.InterviewRescheduled, runner.Status);
            Assert.Equal(InterviewStatus.Rescheduled, interview.Status);
        }

        [Fact]
        public void RescheduleInterview_BlockedWhenStatusDoesNotAllow()
        {
            var runner = NewRunner();
            runner.ChangeStatus(RunnerStatus.InterviewScheduled, Actor);
            var interview = runner.AddInterview(DateTime.Now, InterviewType.Phone, "Jane", null, Actor).Value;
            runner.ChangeStatus(RunnerStatus.Rejected, Actor);
            var result = runner.RescheduleInterview(interview.Id, DateTime.Now.AddDays(1), Actor);
            Assert.False(result);
            Assert.Equal(RunnerStatus.Rejected, runner.Status);
        }
    }
}
