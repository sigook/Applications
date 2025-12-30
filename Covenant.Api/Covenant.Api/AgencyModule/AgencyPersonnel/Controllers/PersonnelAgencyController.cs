using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyPersonnel.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class PersonnelAgencyController : ControllerBase
    {
        public const string RouteName = "api/PersonnelAgency";

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IAgencyRepository repository) =>
            Ok(await repository.GetPersonnelAgency(User.GetUserId()));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromServices] IAgencyRepository repository)
        {
            foreach (var personnel in await repository.GetPersonnelByUserId(User.GetUserId()))
            {
                if (personnel.IsPrimary)
                {
                    personnel.SetToNotPrimary();
                    repository.Update(personnel);
                }

                if (personnel.Id != id) continue;
                personnel.SetToPrimary();
                repository.Update(personnel);
            }

            await repository.SaveChangesAsync();
            return Ok();
        }
    }
}