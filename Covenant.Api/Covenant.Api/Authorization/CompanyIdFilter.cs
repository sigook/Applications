using Covenant.Common.Constants;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Covenant.Api.Authorization
{
    /// <summary>
    /// Add the claim companyId to users that belong to a specific company
    /// </summary>
    public class CompanyIdFilter : IAsyncActionFilter
    {
        private readonly ICompanyRepository _repository;

        public CompanyIdFilter(ICompanyRepository repository) => _repository = repository;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await AddCompanyId(context);
            await next();
        }

        private async Task AddCompanyId(ActionExecutingContext context)
        {
            if (context.Controller is ControllerBase controller)
            {
                if (controller.User.IsInRole(CovenantConstants.Role.CompanyUser))
                {
                    string sub = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(sub, out Guid userId))
                    {
                        Guid companyId = await _repository.GetCompanyIdForUser(userId);
                        controller.User.AddIdentity(new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(CovenantConstants.CompanyId, companyId.ToString())
                        }));
                    }
                }
            }
        }
    }
}