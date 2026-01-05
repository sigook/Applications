using Covenant.IdentityServer.Models.ViewModels;
using IdentityServer4.EntityFramework.Entities;
using System.Threading.Tasks;

namespace Covenant.IdentityServer.Services
{
    public interface IClientService
    {
        Task<Client> GetClientDetails(int id);
        Task AddClient(ClientViewModel clientViewModel);
        Task UpdateClient(int id, ClientViewModel clientViewModel);
    }
}
