using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkersReportDocument.Controllers
{
    [Route("api/[controller]/{requestId}/Document")]
    [ApiController]
    [Authorize]
    public class WorkersReportDocumentController : ControllerBase
    {
        private IMediator mediator;
        private readonly IRequestRepository repository;

        public WorkersReportDocumentController(IRequestRepository repository)
        {
            this.repository = repository;
        }

        protected IMediator Mediator => mediator ?? (mediator = HttpContext.RequestServices.GetService<IMediator>());

        /// <summary>Generates the workers report for a request as an Excel file.</summary>
        /// <param name="requestId">Request identifier.</param>
        [HttpGet]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetReport([FromRoute] Guid requestId)
        {
            var workers = await repository.GetWorkersRequestByRequestId(requestId, new GetWorkersRequestFilter());
            var file = await Mediator.Send(new GenerateWorkersReport(workers.Items));
            return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
        }
    }
}
