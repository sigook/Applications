namespace Covenant.Common.Enums;

public enum RunnerStatus
{
    SentToClient = 1,
    InterviewScheduled = 2,
    InterviewRescheduled = 3,
    NoLongerAvailable = 4,
    NoShow = 5,
    WaitingForInterviewFeedback = 6,
    WaitingForFinalDecision = 7,
    Rejected = 8,
    InOnboardingProcess = 9,
    Hired = 10
}
