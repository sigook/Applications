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
            var request = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(Guid.NewGuid(), new Location(), now, Guid.NewGuid()).Value;

            // Verify initial status is Open
            Assert.Equal(RequestStatus.Open, request.Status);

            // Can only cancel from Open status (no workers assigned)
            Result result = request.Cancel(now);
            Assert.True(result);
            Assert.Equal(RequestStatus.Cancelled, request.Status);
            Assert.Equal(now, request.FinishAt);

            // Cannot cancel an already cancelled request
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
        public void AddWorkerRequestComplete()
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
            Assert.Equal("The Request is complete", result.Errors.Single().Message);
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
            Assert.Equal(RequestStatus.Open, request.Status);
            request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
            Assert.Equal(1, request.WorkersQuantityWorking);
            Assert.Equal(RequestStatus.Open, request.Status);
            var worker2 = Guid.NewGuid();
            request.AddWorker(worker2, new DateTime(2019, 01, 01));
            Assert.Equal(2, request.WorkersQuantityWorking);
            Assert.Equal(RequestStatus.Filled, request.Status);
            request.RejectWorker(worker2, default);
            Assert.Equal(1, request.WorkersQuantityWorking);
            Assert.Equal(RequestStatus.Open, request.Status);
        }

        [Fact]
        public void StatusTransitions()
        {
            const int workersQuantity = 1;
            var request = FakeData.FakeRequest(workersQuantity: workersQuantity);
            Assert.Equal(RequestStatus.Open, request.Status);

            var worker1 = Guid.NewGuid();
            request.AddWorker(worker1, new DateTime(2019, 01, 01));
            Assert.Equal(RequestStatus.Filled, request.Status);

            request.IncreaseWorkersQuantityByOne();
            Assert.Equal(2, request.WorkersQuantity);
            Assert.Equal(RequestStatus.Open, request.Status);

            var jen = Guid.NewGuid();
            request.AddWorker(jen, new DateTime(2019, 01, 01));
            Assert.Equal(RequestStatus.Filled, request.Status);

            request.RejectWorker(jen, default);
            Assert.Equal(RequestStatus.Open, request.Status);

            // Cannot cancel with workers assigned
            var cancelResultWithWorkers = request.Cancel(new DateTime(2019, 01, 01));
            Assert.False(cancelResultWithWorkers);
            Assert.Contains("workers assigned", cancelResultWithWorkers.Errors.Single().Message);

            // Reject all workers to allow cancellation
            request.RejectWorker(worker1, default);
            Assert.Equal(RequestStatus.Open, request.Status);

            // Now can cancel
            request.Cancel(new DateTime(2019, 01, 01));
            Assert.Equal(RequestStatus.Cancelled, request.Status);
        }

        [Fact]
        public void CannotCancelRequestsWithWorkers()
        {
            var request = FakeData.FakeRequest(workersQuantity: 2);

            // Add worker - stays Open
            var worker1 = Guid.NewGuid();
            request.AddWorker(worker1, new DateTime(2019, 01, 01));
            Assert.Equal(RequestStatus.Open, request.Status);

            // Try to cancel with workers - should fail
            var result = request.Cancel(new DateTime(2019, 01, 01));
            Assert.False(result);
            Assert.Contains("workers assigned", result.Errors.Single().Message);
            Assert.Equal(RequestStatus.Open, request.Status);

            // Fill the request
            request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
            Assert.Equal(RequestStatus.Filled, request.Status);

            // Try to cancel from Filled - should fail (wrong status)
            result = request.Cancel(new DateTime(2019, 01, 01));
            Assert.False(result);
            Assert.Equal(RequestStatus.Filled, request.Status);

            // Remove one worker to return to Open
            request.RejectWorker(worker1, default);
            Assert.Equal(RequestStatus.Open, request.Status);

            // Still has one worker, cannot cancel
            result = request.Cancel(new DateTime(2019, 01, 01));
            Assert.False(result);
            Assert.Contains("workers assigned", result.Errors.Single().Message);

            // Remove all workers
            var worker2 = request.Workers.First(w => w.WorkerRequestStatus == WorkerRequestStatus.Booked).WorkerProfileId;
            request.RejectWorker(worker2, default);

            // Now can cancel (Open status, no workers)
            result = request.Cancel(new DateTime(2019, 01, 01));
            Assert.True(result);
            Assert.Equal(RequestStatus.Cancelled, request.Status);
        }

        [Fact]
        public void DecreaseCapacityUpdatesStatus()
        {
            // Start with capacity of 3, add 2 workers (Open)
            var request = FakeData.FakeRequest(workersQuantity: 3);
            request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
            request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
            Assert.Equal(2, request.WorkersQuantityWorking);
            Assert.Equal(3, request.WorkersQuantity);
            Assert.Equal(RequestStatus.Open, request.Status);

            // Reduce capacity to 2 (should become Filled since 2/2)
            var result = request.DecreaseWorkersQuantityByOne();
            Assert.True(result);
            Assert.Equal(2, request.WorkersQuantity);
            Assert.Equal(2, request.WorkersQuantityWorking);
            Assert.Equal(RequestStatus.Filled, request.Status);

            // Increase capacity back to 3 (should become Open)
            result = request.IncreaseWorkersQuantityByOne();
            Assert.True(result);
            Assert.Equal(3, request.WorkersQuantity);
            Assert.Equal(2, request.WorkersQuantityWorking);
            Assert.Equal(RequestStatus.Open, request.Status);
        }
    }
}