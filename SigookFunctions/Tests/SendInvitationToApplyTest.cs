using Covenant.Common.Models.Agency;
using Microsoft.Extensions.Logging;
using SigookFunctions.Functions;
using SigookFunctions.Models;
using SigookFunctions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class SendInvitationToApplyTest
    {
        [Fact]
        public async Task Run()
        {
            var model = new InvitationToApplyModel
            {
                JobTitle = "General Labour",
                Description = "General Labour Description",
                Requirements = "General Labour Requirements",
                Rate = "$22"
            };
            var emailService = new FakeEmailService();
            var sigookApi = new FakeSigookApi();
            await new SendInvitationToApply(sigookApi, emailService).Run(model, null);
            Assert.Equal(model.JobTitle, emailService.Request.JobTitle);
            Assert.Equal(model.Description, emailService.Request.Description);
            Assert.Equal(model.Requirements, emailService.Request.Requirements);
            Assert.Equal(model.Rate, emailService.Request.Rate);
            Assert.Equal(FakeSigookApi.Workers.Count(), emailService.Workers.Count);
        }

        private class FakeSigookApi : ISigookApi
        {
            public static readonly IEnumerable<WorkerContactInfoModel> Workers = Enumerable.Range(0, 2)
                .Select(s => new WorkerContactInfoModel
                {
                    WorkerId = Guid.NewGuid().ToString(),
                    FirstName = $"Mary {s}",
                    LastName = $"Gordon {s}",
                    Email = $"mary{s}@gmail.com"
                });

            public Task<PaginatedList<WorkerContactInfoModel>> GetWorkers(int pageIndex, Guid agencyId)
            {
                return Task.FromResult(new PaginatedList<WorkerContactInfoModel>
                {
                    PageIndex = pageIndex,
                    TotalPages = Workers.Count(),
                    TotalItems = Workers.Count(),
                    Items = new List<WorkerContactInfoModel> { Workers.ElementAt(pageIndex - 1) }
                });
            }
        }

        private class FakeEmailService : IEmailService
        {
            public InvitationToApplyModel Request { get; set; }
            public List<WorkerContactInfoModel> Workers { get; } = new List<WorkerContactInfoModel>();
            public Task SendEmail(InvitationToApplyModel request, List<WorkerContactInfoModel> workers, ILogger logger)
            {
                Request = request;
                Workers.AddRange(workers);
                return Task.CompletedTask;
            }

            public Task SendEmail(EmailModel model, ILogger logger) => Task.CompletedTask;
        }
    }
}
