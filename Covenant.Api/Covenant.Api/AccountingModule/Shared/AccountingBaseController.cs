using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Shared
{
	[Authorize]
	[ApiController]
	public abstract class AccountingBaseController : ControllerBase
	{
		private IMediator mediator;

		protected IMediator Mediator => mediator ?? (mediator = HttpContext.RequestServices.GetService<IMediator>());
	}
}