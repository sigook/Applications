using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Tests.Utils;
using Xunit;

namespace Covenant.Tests.Request
{
    public class RequestTest
    {
        [Fact]
        public void CancelRequest()
        {
            var now = new DateTime(2019, 01, 01);
            var request = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(Guid.NewGuid(), Guid.NewGuid(), new Location(), now, Guid.NewGuid()).Value;
            request.AddWorker(Guid.NewGuid(), now);
            Assert.Equal(RequestStatus.Requested, request.Status);
            Assert.True(request.Workers.All(w => w.WorkerRequestStatus == WorkerRequestStatus.Booked));


            Result result = request.Cancel(now);
            Assert.True(result);
            Assert.Equal(RequestStatus.Cancelled, request.Status);
            Assert.Equal(now, request.FinishAt);
            Assert.True(request.Workers.All(w => w.WorkerRequestStatus == WorkerRequestStatus.Rejected));


            result = request.Cancel(now);
            Assert.False(result);
        }

        [Fact]
        public void AddWorker()
        {
            var startWorking = new DateTime(2019, 01, 01);
            var request = FakeData.FakeRequest();
            var jane = Guid.NewGuid();
            Result result = request.AddWorker(jane, startWorking);
            Assert.True(result);
            Assert.Single(request.Workers);
            result = request.AddWorker(jane, startWorking);
            Assert.False(result);
        }

        [Fact]
        public void AddWorkerOrderComplete()
        {
            const int workersQuantity = 1;
            var request = FakeData.FakeRequest(workersQuantity: workersQuantity);
            var mary = Guid.NewGuid();
            var startWorking = new DateTime(2019, 01, 01);
            Result result = request.AddWorker(mary, startWorking);
            Assert.True(result);
            var adrian = Guid.NewGuid();
            result = request.AddWorker(adrian, startWorking);
            Assert.False(result);
            Assert.Equal("The Order is complete", result.Errors.Single().Message);
            request.RejectWorker(mary, default);
            result = request.AddWorker(adrian, startWorking);
            Assert.True(result);
        }

        [Fact]
        public void AddWorkerIsNotPossibleIfRequestHasCancelled()
        {
            var request = FakeData.FakeRequest();
            request.Cancel(new DateTime(2019, 01, 01));
            var jhon = Guid.NewGuid();
            Result result = request.AddWorker(jhon, new DateTime(2019, 01, 01));
            Assert.False(result);
            Assert.Empty(request.Workers);
        }

        [Fact]
        public void WorkersQuantityWorking()
        {
            const int workersQuantity = 2;
            var request = FakeData.FakeRequest(workersQuantity: workersQuantity);
            Assert.Equal(0, request.WorkersQuantityWorking);
            Assert.True(request.IsOpen);
            request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
            Assert.Equal(1, request.WorkersQuantityWorking);
            Assert.True(request.IsOpen);
            var worker2 = Guid.NewGuid();
            request.AddWorker(worker2, new DateTime(2019, 01, 01));
            Assert.Equal(2, request.WorkersQuantityWorking);
            Assert.False(request.IsOpen);
            request.RejectWorker(worker2, default);
            Assert.Equal(1, request.WorkersQuantityWorking);
            Assert.True(request.IsOpen);
        }

        [Fact]
        public void IsOpen()
        {
            const int workersQuantity = 1;
            var request = FakeData.FakeRequest(workersQuantity: workersQuantity);
            Assert.True(request.IsOpen);
            request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
            Assert.False(request.IsOpen);
            request.IncreaseWorkersQuantityByOne();
            Assert.Equal(2, request.WorkersQuantity);
            Assert.True(request.IsOpen);
            var jen = Guid.NewGuid();
            request.AddWorker(jen, new DateTime(2019, 01, 01));
            Assert.False(request.IsOpen);
            request.RejectWorker(jen, default);
            Assert.True(request.IsOpen);

            request.Cancel(new DateTime(2019, 01, 01));
            Assert.False(request.IsOpen);
        }
    }
}