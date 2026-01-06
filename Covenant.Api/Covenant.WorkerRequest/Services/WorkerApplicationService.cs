using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Covenant.WorkerRequest.Services
{
    public class WorkerApplicationService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IWorkerRequestRepository _workerRequestRepository;
        private readonly ITimeService _timeService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<WorkerApplicationService> _logger;

        public WorkerApplicationService(
            IRequestRepository requestRepository,
            IWorkerRepository workerRepository,
            IWorkerRequestRepository workerRequestRepository,
            ITimeService timeService,
            IConfiguration configuration,
            IHttpClientFactory httpFactory,
            ILogger<WorkerApplicationService> logger)
        {
            _requestRepository = requestRepository;
            _workerRepository = workerRepository;
            _workerRequestRepository = workerRequestRepository;
            _timeService = timeService;
            _configuration = configuration;
            _httpFactory = httpFactory;
            _logger = logger;
        }

        public async Task<Result<RequestApplicantDetailModel>> Apply(Guid workerId, Guid requestId, WorkerRequestApplyModel model)
        {
            
        }
    }
}