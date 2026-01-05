using System;
using Microsoft.AspNetCore.Authentication;

namespace Covenant.Tests.TestsIntegration
{
    public static class TestAuthenticationMiddleware
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder,Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>("Test Scheme", "Test Auth",configureOptions);
        }
    }
}