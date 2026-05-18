using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.PayStub.Controllers;

[Route(RouteName)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AccountingPayStubV4Controller : AccountingBaseController
{
    public const string RouteName = "api/v4/Accounting/PayStub";

    private readonly IPayStubRepository _payStubRepository;
    private readonly IAccountingService accountingService;
    private readonly IPayStubService payStubService;

    public AccountingPayStubV4Controller(
        IPayStubRepository payStubRepository,
        IAccountingService accountingService,
        IPayStubService payStubService)
    {
        _payStubRepository = payStubRepository;
        this.accountingService = accountingService;
        this.payStubService = payStubService;
    }

    /// <summary>
    /// Creates a manual pay stub. Obsolete.
    /// </summary>
    /// <param name="model">Manual pay stub creation data.</param>
    [Obsolete]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreatePayStubModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await payStubService.CreateManualPayStub(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var id = result.Value.Id;
        return CreatedAtAction(nameof(GetById), new { id }, new { });
    }

    /// <summary>
    /// Gets the detail of a pay stub by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the pay stub.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PayStubDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var detail = await _payStubRepository.GetPayStubDetail(id);
        if (detail != null) return Ok(detail);
        return NotFound();
    }

    /// <summary>
    /// Deletes a pay stub by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the pay stub to delete.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await accountingService.DeletePayStub(id);
        return Ok();
    }
}