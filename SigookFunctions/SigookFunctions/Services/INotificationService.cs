using SigookFunctions.Models;

namespace SigookFunctions.Services
{
    public interface INotificationService
    {
        Task<string> SendTeamsNotificationAsync(TeamsMessage message);
    }
}
