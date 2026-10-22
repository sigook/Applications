using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Sales.Dashboard;
using Covenant.Common.Models.Sales;
using Covenant.Core.BL.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Sales;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class DashboardController(ISalesService salesService) : ControllerBase
{
    public const string RouteName = "api/agency/sales/dashboard";

    /// <summary>Gets the whole dashboard in one call: quarter/week summary, deals per status for the date range, and the recent clients, interactions and deals. Sales users only see what they own.</summary>
    /// <param name="filter">Inclusive date range for the deals-by-status block (both dates or none), optional statuses and owner.</param>
    [HttpGet]
    [ProducesResponseType(typeof(SalesDashboardModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboard([FromQuery] GetDealsByStatusFilter filter)
    {
        var result = await salesService.GetDashboard(filter);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Gets the deal count and total value per status for a date range (current month when omitted). Sales users only see the deals they own.</summary>
    /// <param name="filter">Inclusive date range (both dates or none), optional statuses and owner.</param>
    [HttpGet("deals-by-status")]
    [ProducesResponseType(typeof(DealsByStatusModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDealsByStatus([FromQuery] GetDealsByStatusFilter filter)
    {
        var result = await salesService.GetDealsByStatus(filter);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Gets the sales dashboard summary: deals per status for the current quarter and interactions per type for the current week.</summary>
    /// <param name="filter">Optional owner. Sales users only see their own data.</param>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(SalesDashboardSummaryModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary([FromQuery] GetSalesDashboardSummaryFilter filter) =>
        Ok(await salesService.GetDashboardSummary(filter));

    /// <summary>Gets the 10 clients with the most recent interactions, newest first. Sales users only count the interactions they own.</summary>
    [HttpGet("recent-clients")]
    [ProducesResponseType(typeof(List<RecentClientModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentClients() =>
        Ok(await salesService.GetRecentClients());

    /// <summary>Gets the 6 most recent interactions across all clients, newest first. Sales users only see the interactions they own.</summary>
    [HttpGet("recent-interactions")]
    [ProducesResponseType(typeof(List<CompanyInteractionListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentInteractions() =>
        Ok(await salesService.GetRecentInteractions());

    /// <summary>Gets the 6 most recent deals across all clients, newest date first. Sales users only see the deals they own.</summary>
    [HttpGet("recent-deals")]
    [ProducesResponseType(typeof(List<DealListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentDeals() =>
        Ok(await salesService.GetRecentDeals());
}
