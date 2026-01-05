using Covenant.IdentityServer.Configuration;
using Covenant.IdentityServer.Models.ViewModels;
using Covenant.IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Controllers.Clients
{
    [Authorize(AuthenticationSchemes = Microsoft365OpenIdConnect.Scheme)]
    [Route("[controller]")]
    public class ClientController : Controller
    {
        private readonly ConfigurationDbContext context;
        private readonly IClientService clientService;

        public ClientController(ConfigurationDbContext context, IClientService clientService)
        {
            this.context = context;
            this.clientService = clientService;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var clients = await context.Clients
                .AsNoTracking()
                .Select(c => new Client
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    ClientName = c.ClientName,
                    Enabled = c.Enabled,
                    ClientUri = c.ClientUri,
                    RequirePkce = c.RequirePkce,
                    AllowOfflineAccess = c.AllowOfflineAccess
                })
                .ToListAsync();
            return View(clients);
        }

        [Route("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var client = await clientService.GetClientDetails(id);
            return View(client);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var viewModel = new ClientViewModel();
            CreateAllowedScopes();
            return View(viewModel);
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel client)
        {
            if (!ModelState.IsValid)
            {
                CreateAllowedScopes();
                return View(client);
            }
            else
            {
                await clientService.AddClient(client);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var clientViewModel = ClientViewModel.ViewModelFromClient(await clientService.GetClientDetails(id));
            CreateAllowedScopes(clientViewModel.AllowedScopes);
            ViewBag.Id = id;
            return View(clientViewModel);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] ClientViewModel client)
        {
            if (!ModelState.IsValid)
            {
                CreateAllowedScopes(client.AllowedScopes);
                ViewBag.Id = id;
                return View(client);
            }
            else
            {
                await clientService.UpdateClient(id, client);
                return RedirectToAction(nameof(Index));
            }
        }

        private void CreateAllowedScopes(IEnumerable<string> selecteds = null)
        {
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem("api1", "api1"),
                new SelectListItem(Config.Roles.Name, Config.Roles.Name),
                new SelectListItem(IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.OpenId),
                new SelectListItem(IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Profile),
                new SelectListItem(IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.Email),
                new SelectListItem(IdentityServerConstants.StandardScopes.Address, IdentityServerConstants.StandardScopes.Address),
                new SelectListItem(IdentityServerConstants.StandardScopes.Phone, IdentityServerConstants.StandardScopes.Phone),
                new SelectListItem(IdentityServerConstants.StandardScopes.OfflineAccess, IdentityServerConstants.StandardScopes.OfflineAccess),
            };
            if (selecteds?.Any() == true)
            {
                selectListItems = selectListItems
                    .GroupJoin(selecteds, sli => sli.Value, s => s, (selectListItem, selected) => new { selectListItem, selected })
                    .SelectMany(lj => lj.selected.DefaultIfEmpty(), (lj, selected) => new SelectListItem(
                        lj.selectListItem.Text, lj.selectListItem.Value, selected != null)).ToList();
            }
            ViewBag.AllowedScopes = selectListItems;
        }
    }
}
