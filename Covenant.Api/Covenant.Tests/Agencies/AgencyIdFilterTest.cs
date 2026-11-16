using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using Xunit;

namespace Covenant.Tests.Agencies;

public class AgencyIdFilterTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    private sealed class FakeController : ControllerBase;

    private static ClaimsPrincipal Run(string role, params Guid[] tokenAgencyIds) =>
        Run(role, QueryString.Empty, tokenAgencyIds);

    private static ClaimsPrincipal Run(string role, QueryString query, params Guid[] tokenAgencyIds)
    {
        List<Claim> claims = [new(ClaimTypes.NameIdentifier, UserId.ToString()), new(ClaimTypes.Role, role)];
        claims.AddRange(tokenAgencyIds.Select(id => new Claim(CovenantConstants.AgencyId, id.ToString())));
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) };
        httpContext.Request.QueryString = query;
        var controller = new FakeController { ControllerContext = new ControllerContext { HttpContext = httpContext } };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object>(), controller);

        new AgencyIdFilter().OnActionExecuting(context);
        return httpContext.User;
    }

    [Fact]
    public void AgencyIds_ComeFromToken_FirstIsDefault()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var user = Run(CovenantConstants.Role.Admin, first, second, first);

        Assert.Equal(first, user.GetAgencyId());
        Assert.Equal([first, second], user.GetAgencyIds());
        Assert.Single(user.FindAll(CovenantConstants.AgencyId));
    }

    [Fact]
    public void UserWithoutAgencies_GetsEmptyAgency()
    {
        var user = Run(CovenantConstants.Role.Sales);

        Assert.Equal(Guid.Empty, user.GetAgencyId());
        Assert.Empty(user.GetAgencyIds());
    }

    [Fact]
    public void AgencyIdsQuery_KeepsOnlyAccessibleAgencies()
    {
        var first = Guid.NewGuid();
        var other = Guid.NewGuid();
        var foreign = Guid.NewGuid();
        var query = new QueryString($"?agencyIds={other}&agencyIds={foreign}");

        var user = Run(CovenantConstants.Role.Admin, query, first, other);

        Assert.Equal([other], user.GetAgencyIds());
        Assert.Equal(first, user.GetAgencyId());
    }

    [Fact]
    public void AgencyIdsQuery_WithNoAccessibleAgency_SelectsNone()
    {
        var query = QueryString.Create("agencyIds", Guid.NewGuid().ToString());

        var user = Run(CovenantConstants.Role.Admin, query, Guid.NewGuid());

        Assert.Empty(user.GetAgencyIds());
    }

    [Fact]
    public void AgencyIdQuery_SelectsTheActiveAgencyWhenAccessible()
    {
        var first = Guid.NewGuid();
        var other = Guid.NewGuid();

        var user = Run(CovenantConstants.Role.Admin, QueryString.Create("agencyId", other.ToString()), first, other);

        Assert.Equal(other, user.GetAgencyId());
    }

    [Fact]
    public void AgencyIdQuery_IgnoresForeignAgency()
    {
        var first = Guid.NewGuid();

        var user = Run(CovenantConstants.Role.Admin, QueryString.Create("agencyId", Guid.NewGuid().ToString()), first);

        Assert.Equal(first, user.GetAgencyId());
    }

    [Fact]
    public void NonAgencyUser_IsLeftUntouched()
    {
        var tokenAgency = Guid.NewGuid();
        var user = Run(CovenantConstants.Role.Worker, tokenAgency);

        Assert.Equal(tokenAgency.ToString(), user.FindFirst(CovenantConstants.AgencyId)?.Value);
        Assert.Null(user.FindFirst(CovenantConstants.AgencyIds));
    }
}
