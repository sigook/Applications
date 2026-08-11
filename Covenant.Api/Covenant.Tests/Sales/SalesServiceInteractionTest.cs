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
    public class SalesServiceInteractionTest
    {
        private readonly Mock<ICompanyRepository> _companyRepository = new();
        private readonly Mock<IIdentityServerService> _identityServerService = new();
        private readonly ISalesService _sut;
        private readonly Guid _agencyId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public SalesServiceInteractionTest()
        {
            _identityServerService.Setup(i => i.GetAgencyId()).Returns(_agencyId);
            _identityServerService.Setup(i => i.GetUserId()).Returns(_userId);
            _sut = new SalesService(
                Mock.Of<IRequestService>(),
                Mock.Of<IRequestRepository>(),
                _companyRepository.Object,
                _identityServerService.Object,
                Mock.Of<IUploadedFilesService>(),
                new CreateCompanyInteractionModelValidator(),
                new UpdateCompanyInteractionModelValidator(),
                new CreateDealModelValidator(),
                new UpdateDealModelValidator());
        }

        private static CreateCompanyInteractionModel ValidCreateModel() => new()
        {
            CompanyProfileId = Guid.NewGuid(),
            Description = "Called the client to introduce our services",
            InteractionPurpose = InteractionPurpose.Intro,
            InteractionType = InteractionType.Call,
            InteractionStatus = InteractionStatus.NotStarted,
        };

        private static UpdateCompanyInteractionModel ValidUpdateModel() => new()
        {
            Description = "Followed up by email",
            InteractionPurpose = InteractionPurpose.FollowUp,
            InteractionType = InteractionType.Mail,
            InteractionStatus = InteractionStatus.InProgress,
        };

        private CompanyInteraction OwnedInteraction(Guid ownerId) =>
            new("Existing", ownerId, Guid.NewGuid(), InteractionPurpose.Intro, InteractionType.Call, InteractionStatus.NotStarted);

        [Fact]
        public async Task CreateInteractionSucceedsWhenModelValid()
        {
            Result<Guid> result = await _sut.CreateInteraction(ValidCreateModel());
            Assert.True(result);
            Assert.Empty(result.Errors);
            _companyRepository.Verify(r => r.Create(It.IsAny<CompanyInteraction>()), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateInteractionFailsWhenDescriptionEmpty()
        {
            var model = ValidCreateModel();
            model.Description = string.Empty;
            Result<Guid> result = await _sut.CreateInteraction(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateCompanyInteractionModel.Description));
            _companyRepository.Verify(r => r.Create(It.IsAny<CompanyInteraction>()), Times.Never);
        }

        [Fact]
        public async Task CreateInteractionFailsWhenCompanyProfileMissing()
        {
            var model = ValidCreateModel();
            model.CompanyProfileId = Guid.Empty;
            Result<Guid> result = await _sut.CreateInteraction(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateCompanyInteractionModel.CompanyProfileId));
        }

        [Fact]
        public async Task CreateInteractionFailsWhenPurposeOutOfRange()
        {
            var model = ValidCreateModel();
            model.InteractionPurpose = (InteractionPurpose)99;
            Result<Guid> result = await _sut.CreateInteraction(model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(CreateCompanyInteractionModel.InteractionPurpose));
        }

        [Fact]
        public async Task UpdateInteractionSucceedsWhenValidAndOwned()
        {
            var interaction = OwnedInteraction(_userId);
            _companyRepository
                .Setup(r => r.GetInteraction(It.IsAny<Expression<Func<CompanyInteraction, bool>>>()))
                .ReturnsAsync(interaction);
            Result result = await _sut.UpdateInteraction(interaction.Id, ValidUpdateModel());
            Assert.True(result);
            _companyRepository.Verify(r => r.Update(interaction), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInteractionFailsWhenDescriptionEmpty()
        {
            var model = ValidUpdateModel();
            model.Description = string.Empty;
            Result result = await _sut.UpdateInteraction(Guid.NewGuid(), model);
            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Key == nameof(UpdateCompanyInteractionModel.Description));
            _companyRepository.Verify(r => r.GetInteraction(It.IsAny<Expression<Func<CompanyInteraction, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task UpdateInteractionFailsWhenInteractionNotFound()
        {
            _companyRepository
                .Setup(r => r.GetInteraction(It.IsAny<Expression<Func<CompanyInteraction, bool>>>()))
                .ReturnsAsync((CompanyInteraction)null);
            Result result = await _sut.UpdateInteraction(Guid.NewGuid(), ValidUpdateModel());
            Assert.False(result);
            Assert.Equal("Interaction not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateInteractionFailsWhenSalesUserIsNotOwner()
        {
            _identityServerService.Setup(i => i.IsSales()).Returns(true);
            var interaction = OwnedInteraction(Guid.NewGuid());
            _companyRepository
                .Setup(r => r.GetInteraction(It.IsAny<Expression<Func<CompanyInteraction, bool>>>()))
                .ReturnsAsync(interaction);
            Result result = await _sut.UpdateInteraction(interaction.Id, ValidUpdateModel());
            Assert.False(result);
            Assert.Equal("You can only manage your own interactions", result.Errors.First().Message);
            _companyRepository.Verify(r => r.Update(It.IsAny<CompanyInteraction>()), Times.Never);
        }

        [Fact]
        public async Task DeleteInteractionSucceedsWhenOwned()
        {
            var interaction = OwnedInteraction(_userId);
            _companyRepository
                .Setup(r => r.GetInteraction(It.IsAny<Expression<Func<CompanyInteraction, bool>>>()))
                .ReturnsAsync(interaction);
            Result result = await _sut.DeleteInteraction(interaction.Id);
            Assert.True(result);
            _companyRepository.Verify(r => r.Delete(interaction), Times.Once);
            _companyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteInteractionFailsWhenInteractionNotFound()
        {
            _companyRepository
                .Setup(r => r.GetInteraction(It.IsAny<Expression<Func<CompanyInteraction, bool>>>()))
                .ReturnsAsync((CompanyInteraction)null);
            Result result = await _sut.DeleteInteraction(Guid.NewGuid());
            Assert.False(result);
            Assert.Equal("Interaction not found", result.Errors.First().Message);
            _companyRepository.Verify(r => r.Delete(It.IsAny<CompanyInteraction>()), Times.Never);
        }
    }
}
