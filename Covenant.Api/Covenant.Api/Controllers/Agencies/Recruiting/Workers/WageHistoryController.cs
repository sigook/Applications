using Covenant.Api.Authorization;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Workers;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[Route(RouteName)]
public class WageHistoryController(IWorkerRepository workerRepository) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/workers/{workerProfileId:guid}/wagehistory";

    /// <summary>Gets the paginated pay stub history for a worker profile with running totals.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="filter">Filter, sort and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<PayStubHistoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerProfileId, [FromQuery] GetWorkerPayStubHistoryFilter filter) =>
        Ok(await workerRepository.GetWageHistory(User.GetAgencyId(), workerProfileId, filter));
}
