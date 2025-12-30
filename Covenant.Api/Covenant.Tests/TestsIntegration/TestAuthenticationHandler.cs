using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Covenant.Tests.TestsIntegration
{
    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        public const string Agency = "agency";
        public const string Company = "company";
        public const string CovenantCompany = "CovenantCompany";
        public const string Worker = "worker";
        public const string CovenantWorker = "CovenatWorker";
        public static readonly FakeUser UserAgency = new FakeUser("3BA9C4DE-FF05-4EDE-8F4C-88F8BBAB1698", "agency@gmail.com");
        public static readonly FakeUser UserCompany = new FakeUser("1D44CFE3-3E54-4B88-8CF5-6AF24FE45EDF", "company@gmail.com");
        public static readonly FakeUser UserCovenantCompany = new FakeUser("AA17C6C7-80FB-4D57-8B26-16E49C4B95C5", "covenant_company@gmail.com");
        public static readonly FakeUser UserWorker = new FakeUser("B518D5BA-CA48-4085-9384-7B029BF5D29E", "worker@gmail.com");
        public static readonly FakeUser UserCovenantWorker = new FakeUser("A97742DF-A348-4E01-8484-7D07CDCDA6F4", "covenant_worker@gmail.com");

        public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Context.Request.Headers.TryGetValue("Authorization", out StringValues user);
            if (user.Count > 0)
            {
                List<Claim> claims = Options.Identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier ||
                                                                        c.Type == Agency ||
                                                                        c.Type == Company ||
                                                                        c.Type == Worker).ToList();
                foreach (Claim claim in claims)
                {
                    Options.Identity.TryRemoveClaim(claim);
                }

                string typeUser = user.First();
                if (typeUser.Contains(Agency))
                {
                    Options.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserAgency.UserId));
                    Options.Identity.AddClaim(new Claim(Agency, Agency));
                }
                else if (typeUser.Contains(Company))
                {
                    Options.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserCompany.UserId));
                    Options.Identity.AddClaim(new Claim(Company, Company));
                }
                else if (typeUser.Contains(CovenantCompany))
                {
                    Options.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserCovenantCompany.UserId));
                    Options.Identity.AddClaim(new Claim(Company, Company));
                }
                else if (typeUser.Contains(Worker))
                {
                    Options.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserWorker.UserId));
                    Options.Identity.AddClaim(new Claim(Worker, Worker));
                }
                else if (typeUser.Contains(CovenantWorker))
                {
                    Options.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserCovenantWorker.UserId));
                    Options.Identity.AddClaim(new Claim(Worker, Worker));
                }
            }
            var authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(Options.Identity), new AuthenticationProperties(), "Test scheme");
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }

        public class FakeUser
        {
            public FakeUser(string userId, string email)
            {
                UserId = userId;
                Email = email;
            }

            public string UserId { get; private set; }
            public string Email { get; private set; }
        }
    }
}