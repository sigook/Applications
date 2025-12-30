using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.PayStubs.Models;
using Covenant.PayStubs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.PayStub.Controllers;

[Route(RouteName)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AccountingPayStubV4Controller : AccountingBaseController
{
    public const string RouteName = "api/v4/Accounting/PayStub";

    private readonly IPayStubRepository _payStubRepository;
    private readonly IAgencyRepository _agencyRepository;
    private readonly IAccountingService accountingService;

    public AccountingPayStubV4Controller(
        IPayStubRepository payStubRepository,
        IAgencyRepository agencyRepository,
        IAccountingService accountingService)
    {
        _payStubRepository = payStubRepository;
        _agencyRepository = agencyRepository;
        this.accountingService = accountingService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromServices] CreatePayStubWithOutTimeSheet service,
        [FromBody] CreatePayStubModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await service.Create(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        var id = result.Value.Id;
        return CreatedAtAction(nameof(GetById), new { id = id }, new { });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var detail = await _payStubRepository.GetPayStubDetail(id);
        if (detail != null) return Ok(detail);
        return NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await accountingService.DeletePayStub(id);
        return Ok();
    }
}