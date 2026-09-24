using Covenant.Api.Validators.Company;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Sales
{
    public class SalesServiceDealTest
    {
        private readonly Mock<ICompanyRepository> _companyRepository = new();
        private readonly Mock<ICurrentUserService> _currentUserService = new();
        private readonly Mock<IUploadedFilesService> _uploadedFilesService = new();
        private readonly Mock<IDocumentService> _documentService = new();
        private readonly ISalesService _sut;
        private readonly Guid _agencyId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public SalesServiceDealTest()
        {
            _currentUserService.Setup(i => i.GetAgencyId()).Returns(_agencyId);
            _currentUserService.Setup(i => i.GetUserId()).Returns(_userId);
            _uploadedFilesService.Setup(u => u.Validate()).Returns(Result.Ok());
            _sut = new SalesService(
                Mock.Of<IRequestService>(),
                Mock.Of<IRequestRepository>(),
                _companyRepository.Object,
                _currentUserService.Object,
                _uploadedFilesService.Object,
                _documentService.Object,
                new CreateCompanyInteractionModelValidator(),
                new UpdateCompanyInteractionModelValidator(),
                new CreateDealModelValidator(),
                new UpdateDealModelValidator(),
                Mock.Of<ITimeService>(),
                new GetDealsByStatusFilterValidator());
        }

        private static CreateDealModel ValidCreateModel() => new()
        {
            Title = "Warehouse staffing",
            CompanyProfileId = Guid.NewGuid(),
            Date = new DateTime(2026, 1, 1),
            Value = 1000m,
            Type = DealType.Temporal,
            Status = DealStatus.ToSend,
            DocumentId = null,
        };

        private static UpdateDealModel ValidUpdateModel() => new()
        {
            Title = "Updated title",
            Date = new DateTime(2026, 2, 1),
            Value = 500m,
            Type = DealType.Permanent,
            Status = DealStatus.Sent,
            DocumentId = null,
        };

        private Deal OwnedDeal(Guid ownerId) =>
            new("Existing", ownerId, Guid.NewGuid(), new DateTime(2026, 1, 1), 1, DealType.Temporal, DealStatus.ToSend, null);

        private Task<Result<Guid>> CreateDeal(CreateDealModel model)
        {
            _uploadedFilesService.Setup(u => u.GetModel<CreateDealModel>()).Returns(model);
            return _sut.CreateDeal();
        }

        private Task<Result> UpdateDeal(Guid id, UpdateDealModel model)
        {
            _uploadedFilesService.Setup(u => u.GetModel<UpdateDealModel>()).Returns(model);
            return _sut.UpdateDeal(id);
        }

        [Fact]
        public async Task CreateDealSucceedsWhenModelValid()
        {
            Result<Guid> result = await CreateDeal(ValidCreateModel());
            Assert.True(result);
            Assert.Empty(result.Errors);
            _companyRepository.Verify(r => r.Create(It.IsAny<Deal>()), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDealWithFileCreatesAndLinksDocument()
        {
            Deal created = null;
            _companyRepository
                .Setup(r => r.Create(It.IsAny<Deal>()))
                .Callback<Deal>(d => created = d)
                .Returns(Task.CompletedTask);
            var model = ValidCreateModel();
            model.FileName = "Deal_abc123.pdf";
            Result<Guid> result = await CreateDeal(model);
            Assert.True(result);
            _companyRepository.Verify(r => r.Create(It.IsAny<CovenantFile>()), Times.Once);
            Assert.NotNull(created.Document);
            Assert.Equal("Deal_abc123.pdf", created.Document.FileName);
            _uploadedFilesService.Verify(u => u.Upload(It.IsAny<IEnumerable<string>>()), Times.Once);
        }

        [Fact]
        public async Task CreateDealFailsWhenTitleEmpty()
        {
            var model = ValidCreateModel();
            model.Title = string.Empty;
            Result<Guid> result = await CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Title));
            _companyRepository.Verify(r => r.Create(It.IsAny<Deal>()), Times.Never);
        }

        [Fact]
        public async Task CreateDealFailsWhenValueNegative()
        {
            var model = ValidCreateModel();
            model.Value = -1;
            Result<Guid> result = await CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Value));
        }

        [Fact]
        public async Task CreateDealFailsWhenTypeOutOfRange()
        {
            var model = ValidCreateModel();
            model.Type = (DealType)99;
            Result<Guid> result = await CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Type));
        }

        [Fact]
        public async Task CreateDealFailsWhenStatusOutOfRange()
        {
            var model = ValidCreateModel();
            model.Status = (DealStatus)99;
            Result<Guid> result = await CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Status));
        }

        [Theory]
        [InlineData(DealStatus.UnderReview, 4)]
        [InlineData(DealStatus.Closed, 5)]
        [InlineData(DealStatus.Completed, 6)]
        public async Task CreateDealSucceedsWithNewStatus(DealStatus status, int expectedValue)
        {
            Deal created = null;
            _companyRepository
                .Setup(r => r.Create(It.IsAny<Deal>()))
                .Callback<Deal>(d => created = d)
                .Returns(Task.CompletedTask);
            var model = ValidCreateModel();
            model.Status = status;
            Result<Guid> result = await CreateDeal(model);
            Assert.True(result);
            Assert.Equal(status, created.Status);
            Assert.Equal(expectedValue, (int)created.Status);
        }

        [Fact]
        public async Task CreateDealFailsWhenCompanyProfileMissing()
        {
            var model = ValidCreateModel();
            model.CompanyProfileId = Guid.Empty;
            Result<Guid> result = await CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.CompanyProfileId));
        }

        [Fact]
        public async Task UpdateDealSucceedsWhenValidAndOwned()
        {
            var deal = OwnedDeal(_userId);
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await UpdateDeal(deal.Id, ValidUpdateModel());
            Assert.True(result);
            _companyRepository.Verify(r => r.Update(deal), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateDealWithFileReplacesDocumentAndDeletesPrevious()
        {
            var deal = OwnedDeal(_userId);
            var previousDocumentId = Guid.NewGuid();
            deal.DocumentId = previousDocumentId;
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            CovenantFile created = null;
            _companyRepository
                .Setup(r => r.Create(It.IsAny<CovenantFile>()))
                .Callback<CovenantFile>(f => created = f)
                .Returns(Task.CompletedTask);
            var model = ValidUpdateModel();
            model.DocumentId = previousDocumentId;
            model.FileName = "Deal_def456.pdf";
            Result result = await UpdateDeal(deal.Id, model);
            Assert.True(result);
            Assert.Equal("Deal_def456.pdf", created.FileName);
            Assert.Equal(created.Id, deal.DocumentId);
            _uploadedFilesService.Verify(u => u.Upload(It.IsAny<IEnumerable<string>>()), Times.Once);
            _documentService.Verify(d => d.DeleteFile(previousDocumentId), Times.Once);
        }

        [Fact]
        public async Task UpdateDealKeepsDocumentWhenNoFileUploaded()
        {
            var deal = OwnedDeal(_userId);
            var documentId = Guid.NewGuid();
            deal.DocumentId = documentId;
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            var model = ValidUpdateModel();
            model.DocumentId = documentId;
            Result result = await UpdateDeal(deal.Id, model);
            Assert.True(result);
            Assert.Equal(documentId, deal.DocumentId);
            _companyRepository.Verify(r => r.Create(It.IsAny<CovenantFile>()), Times.Never);
            _documentService.Verify(d => d.DeleteFile(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDealFailsWhenTitleEmpty()
        {
            var model = ValidUpdateModel();
            model.Title = string.Empty;
            Result result = await UpdateDeal(Guid.NewGuid(), model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(UpdateDealModel.Title));
            _companyRepository.Verify(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDealFailsWhenDealNotFound()
        {
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync((Deal)null);
            Result result = await UpdateDeal(Guid.NewGuid(), ValidUpdateModel());
            Assert.False(result);
            Assert.Equal("Deal not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateDealFailsWhenSalesUserIsNotOwner()
        {
            _currentUserService.Setup(i => i.IsSales()).Returns(true);
            var deal = OwnedDeal(Guid.NewGuid());
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await UpdateDeal(deal.Id, ValidUpdateModel());
            Assert.False(result);
            Assert.Equal("You can only manage your own deals", result.Errors.First().Message);
            _companyRepository.Verify(r => r.Update(It.IsAny<Deal>()), Times.Never);
        }

        [Fact]
        public async Task GetDealsScopesToOwnerForSalesUser()
        {
            _currentUserService.Setup(i => i.IsSales()).Returns(true);
            GetDealsFilter captured = null;
            _companyRepository
                .Setup(r => r.GetDeals(_agencyId, It.IsAny<GetDealsFilter>()))
                .Callback<Guid, GetDealsFilter>((_, f) => captured = f)
                .ReturnsAsync(new PaginatedList<DealListModel>());
            await _sut.GetDeals(new GetDealsFilter { OwnerId = Guid.NewGuid() });
            Assert.Equal(_userId, captured.OwnerId);
        }

        [Fact]
        public async Task GetDealsIsNotScopedForAdmin()
        {
            _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
            GetDealsFilter captured = null;
            _companyRepository
                .Setup(r => r.GetDeals(_agencyId, It.IsAny<GetDealsFilter>()))
                .Callback<Guid, GetDealsFilter>((_, f) => captured = f)
                .ReturnsAsync(new PaginatedList<DealListModel>());
            await _sut.GetDeals(new GetDealsFilter());
            Assert.Null(captured.OwnerId);
        }

        [Fact]
        public async Task GetDealsKeepsRequestedOwnerForAdmin()
        {
            _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
            var otherUserId = Guid.NewGuid();
            GetDealsFilter captured = null;
            _companyRepository
                .Setup(r => r.GetDeals(_agencyId, It.IsAny<GetDealsFilter>()))
                .Callback<Guid, GetDealsFilter>((_, f) => captured = f)
                .ReturnsAsync(new PaginatedList<DealListModel>());
            await _sut.GetDeals(new GetDealsFilter { OwnerId = otherUserId });
            Assert.Equal(otherUserId, captured.OwnerId);
        }

        [Fact]
        public async Task UpdateDealSucceedsWhenAdminIsNotOwner()
        {
            _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
            var deal = OwnedDeal(Guid.NewGuid());
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await UpdateDeal(deal.Id, ValidUpdateModel());
            Assert.True(result);
            _companyRepository.Verify(r => r.Update(deal), Times.Once);
        }

        [Fact]
        public async Task DeleteDealSucceedsWhenAdminIsNotOwner()
        {
            _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
            var deal = OwnedDeal(Guid.NewGuid());
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await _sut.DeleteDeal(deal.Id);
            Assert.True(result);
            _companyRepository.Verify(r => r.Delete(deal), Times.Once);
        }

        [Fact]
        public async Task DeleteDealSucceedsWhenOwned()
        {
            var deal = OwnedDeal(_userId);
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await _sut.DeleteDeal(deal.Id);
            Assert.True(result);
            _companyRepository.Verify(r => r.Delete(deal), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteDealFailsWhenDealNotFound()
        {
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync((Deal)null);
            Result result = await _sut.DeleteDeal(Guid.NewGuid());
            Assert.False(result);
            Assert.Equal("Deal not found", result.Errors.First().Message);
            _companyRepository.Verify(r => r.Delete(It.IsAny<Deal>()), Times.Never);
        }
    }
}
