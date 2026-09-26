namespace Sigook.Functions.Configuration;

public class ScheduleTasksOptions
{
    public const string SectionName = "ScheduleTasks";

    public string ApiUrl { get; set; } = string.Empty;
    public string AccountsUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
