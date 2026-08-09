using Covenant.Api.Validators.Company;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
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
        private readonly Mock<IIdentityServerService> _identityServerService = new();
        private readonly ISalesService _sut;
        private readonly Guid _agencyId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public SalesServiceDealTest()
        {
            _identityServerService.Setup(i => i.GetAgencyId()).Returns(_agencyId);
            _identityServerService.Setup(i => i.GetUserId()).Returns(_userId);
            _companyRepository
                .Setup(r => r.CompanyProfileBelongsToAgency(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _sut = new SalesService(
                Mock.Of<IRequestService>(),
                Mock.Of<IRequestRepository>(),
                _companyRepository.Object,
                _identityServerService.Object,
                new CreateCompanyInteractionModelValidator(),
                new UpdateCompanyInteractionModelValidator(),
                new CreateDealModelValidator(),
                new UpdateDealModelValidator());
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

        [Fact]
        public async Task CreateDealSucceedsWhenModelValid()
        {
            Result<Guid> result = await _sut.CreateDeal(ValidCreateModel());
            Assert.True(result);
            Assert.Empty(result.Errors);
            _companyRepository.Verify(r => r.Create(It.IsAny<Deal>()), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDealFailsWhenTitleEmpty()
        {
            var model = ValidCreateModel();
            model.Title = string.Empty;
            Result<Guid> result = await _sut.CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Title));
            _companyRepository.Verify(r => r.Create(It.IsAny<Deal>()), Times.Never);
        }

        [Fact]
        public async Task CreateDealFailsWhenValueNegative()
        {
            var model = ValidCreateModel();
            model.Value = -1;
            Result<Guid> result = await _sut.CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Value));
        }

        [Fact]
        public async Task CreateDealFailsWhenTypeOutOfRange()
        {
            var model = ValidCreateModel();
            model.Type = (DealType)99;
            Result<Guid> result = await _sut.CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.Type));
        }

        [Fact]
        public async Task CreateDealFailsWhenCompanyProfileMissing()
        {
            var model = ValidCreateModel();
            model.CompanyProfileId = Guid.Empty;
            Result<Guid> result = await _sut.CreateDeal(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateDealModel.CompanyProfileId));
        }

        [Fact]
        public async Task CreateDealDoesNotQueryRepositoryWhenModelInvalid()
        {
            var model = ValidCreateModel();
            model.Title = string.Empty;
            await _sut.CreateDeal(model);
            _companyRepository.Verify(r => r.CompanyProfileBelongsToAgency(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CreateDealFailsWhenCompanyDoesNotBelongToAgency()
        {
            _companyRepository
                .Setup(r => r.CompanyProfileBelongsToAgency(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            Result<Guid> result = await _sut.CreateDeal(ValidCreateModel());
            Assert.False(result);
            Assert.Equal("Company profile not found", result.Errors.First().Message);
            _companyRepository.Verify(r => r.Create(It.IsAny<Deal>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDealSucceedsWhenValidAndOwned()
        {
            var deal = OwnedDeal(_userId);
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await _sut.UpdateDeal(deal.Id, ValidUpdateModel());
            Assert.True(result);
            _companyRepository.Verify(r => r.Update(deal), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateDealFailsWhenTitleEmpty()
        {
            var model = ValidUpdateModel();
            model.Title = string.Empty;
            Result result = await _sut.UpdateDeal(Guid.NewGuid(), model);
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
            Result result = await _sut.UpdateDeal(Guid.NewGuid(), ValidUpdateModel());
            Assert.False(result);
            Assert.Equal("Deal not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateDealFailsWhenSalesUserIsNotOwner()
        {
            _identityServerService.Setup(i => i.IsSales()).Returns(true);
            var deal = OwnedDeal(Guid.NewGuid());
            _companyRepository
                .Setup(r => r.GetDeal(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(deal);
            Result result = await _sut.UpdateDeal(deal.Id, ValidUpdateModel());
            Assert.False(result);
            Assert.Equal("You can only manage your own deals", result.Errors.First().Message);
            _companyRepository.Verify(r => r.Update(It.IsAny<Deal>()), Times.Never);
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
