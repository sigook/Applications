using Covenant.Common.Constants;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Common.Repositories.Identity;
using Covenant.Core.BL.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Covenant.Tests.Identity;

public class UserAdministrationServiceTest
{
    private const string Email = "new.user@sigook.com";

    private readonly Mock<UserManager<CovenantUser>> _userManager = new(Mock.Of<IUserStore<CovenantUser>>(), null, null, null, null, null, null, null, null);
    private readonly Mock<RoleManager<CovenantRole>> _roleManager = new(Mock.Of<IRoleStore<CovenantRole>>(), null, null, null, null);
    private readonly Mock<IIdentityRepository> _repository = new();
    private readonly Mock<IAccountNotificationService> _notifications = new();
    private readonly UserAdministrationService _sut;
    private readonly CovenantUser _existing = new() { Id = Guid.NewGuid(), Email = "existing@sigook.com" };
    private CovenantUser _created;
    private readonly List<Claim> _addedClaims = [];

    public UserAdministrationServiceTest()
    {
        _roleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userManager.Setup(u => u.CreateAsync(It.IsAny<CovenantUser>(), It.IsAny<string>()))
            .Callback<CovenantUser, string>((user, _) => _created = user)
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(u => u.AddClaimAsync(It.IsAny<CovenantUser>(), It.IsAny<Claim>()))
            .Callback<CovenantUser, Claim>((_, claim) => _addedClaims.Add(claim))
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(u => u.AddToRolesAsync(It.IsAny<CovenantUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(u => u.FindByIdAsync(_existing.Id.ToString())).ReturnsAsync(_existing);
        _sut = new UserAdministrationService(_userManager.Object, _roleManager.Object, _repository.Object, _notifications.Object);
    }

    private static CreateUserModel Model(UserType type, string confirmPassword = null, Guid? agencyId = null, Guid? companyId = null) => new()
    {
        Email = Email,
        Password = "Secret123!",
        ConfirmPassword = confirmPassword,
        UserType = type,
        Role = CovenantConstants.Role.Worker,
        AgencyId = agencyId,
        CompanyId = companyId
    };

    private static IdentityResult Failed(string code, string description) =>
        IdentityResult.Failed(new IdentityError { Code = code, Description = description });

    private void Claims(params Claim[] claims) =>
        _userManager.Setup(u => u.GetClaimsAsync(_existing)).ReturnsAsync([.. claims]);

    [Fact]
    public async Task CreateUser_UnknownRole_FailsWithoutCreatingTheUser()
    {
        _roleManager.Setup(r => r.RoleExistsAsync("ghost")).ReturnsAsync(false);
        CreateUserModel model = Model(UserType.Worker);
        model.Role = "ghost";

        Result<Guid> result = await _sut.CreateUser(model);

        Assert.False(result);
        Assert.Contains(result.Errors, e => e.Key == nameof(CreateUserModel.Role));
        _userManager.Verify(u => u.CreateAsync(It.IsAny<CovenantUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_EmailAlreadyRegistered_Fails()
    {
        _userManager.Setup(u => u.FindByEmailAsync(Email)).ReturnsAsync(_existing);

        Result<Guid> result = await _sut.CreateUser(Model(UserType.Worker));

        Assert.False(result);
        _userManager.Verify(u => u.CreateAsync(It.IsAny<CovenantUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_Agency_IsConfirmedWithAgencyClaimAndNoEmail()
    {
        var agencyId = Guid.NewGuid();

        Result<Guid> result = await _sut.CreateUser(Model(UserType.Agency, agencyId: agencyId));

        Assert.True(result);
        Assert.Equal(_created.Id, result.Value);
        Assert.True(_created.EmailConfirmed);
        Assert.Equal(Email, _created.UserName);
        Claim claim = Assert.Single(_addedClaims);
        Assert.Equal(CovenantConstants.AgencyId, claim.Type);
        Assert.Equal(agencyId.ToString(), claim.Value);
        _notifications.VerifyNoOtherCalls();
        _userManager.Verify(u => u.AddToRolesAsync(_created, It.Is<IEnumerable<string>>(r => r.Single() == CovenantConstants.Role.Worker)), Times.Once);
    }

    [Fact]
    public async Task CreateUser_AgencyPersonnel_IsConfirmedAndReceivesTheCreatePasswordLink()
    {
        Result<Guid> result = await _sut.CreateUser(Model(UserType.AgencyPersonnel, agencyId: Guid.NewGuid()));

        Assert.True(result);
        Assert.True(_created.EmailConfirmed);
        Assert.Equal(CovenantConstants.AgencyId, Assert.Single(_addedClaims).Type);
        _notifications.Verify(n => n.SendConfirmAndSetPassword(_created), Times.Once);
    }

    [Fact]
    public async Task CreateUser_CompanyUser_IsUnconfirmedWithCompanyClaimAndCreatePasswordLink()
    {
        var companyId = Guid.NewGuid();

        Result<Guid> result = await _sut.CreateUser(Model(UserType.CompanyUser, companyId: companyId));

        Assert.True(result);
        Assert.False(_created.EmailConfirmed);
        Claim claim = Assert.Single(_addedClaims);
        Assert.Equal(CovenantConstants.CompanyId, claim.Type);
        Assert.Equal(companyId.ToString(), claim.Value);
        _notifications.Verify(n => n.SendConfirmAndSetPassword(_created), Times.Once);
    }

    [Fact]
    public async Task CreateUser_CompanyCreatedByAgency_IsConfirmedWithoutEmail()
    {
        Result<Guid> result = await _sut.CreateUser(Model(UserType.Company));

        Assert.True(result);
        Assert.True(_created.EmailConfirmed);
        _notifications.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateUser_SelfRegisteredCompany_MustConfirmItsEmail()
    {
        Result<Guid> result = await _sut.CreateUser(Model(UserType.Company, confirmPassword: "Secret123!"));

        Assert.True(result);
        Assert.False(_created.EmailConfirmed);
        _notifications.Verify(n => n.SendConfirmAccount(_created, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_SelfRegisteredWorker_ReceivesTheConfirmAccountLink()
    {
        Result<Guid> result = await _sut.CreateUser(Model(UserType.Worker, confirmPassword: "Secret123!"));

        Assert.True(result);
        Assert.False(_created.EmailConfirmed);
        _notifications.Verify(n => n.SendConfirmAccount(_created, It.IsAny<string>()), Times.Once);
        _notifications.Verify(n => n.SendConfirmAndSetPassword(It.IsAny<CovenantUser>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_WorkerCreatedByAgency_ReceivesTheCreatePasswordLink()
    {
        Result<Guid> result = await _sut.CreateUser(Model(UserType.Worker));

        Assert.True(result);
        _notifications.Verify(n => n.SendConfirmAndSetPassword(_created), Times.Once);
        _notifications.Verify(n => n.SendConfirmAccount(It.IsAny<CovenantUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_IdentityRejection_IsMappedToResultErrors()
    {
        _userManager.Setup(u => u.CreateAsync(It.IsAny<CovenantUser>(), It.IsAny<string>()))
            .ReturnsAsync(Failed("PasswordTooShort", "Passwords must be at least 8 characters."));

        Result<Guid> result = await _sut.CreateUser(Model(UserType.Worker));

        Assert.False(result);
        ResultError error = Assert.Single(result.Errors);
        Assert.Equal("PasswordTooShort", error.Key);
        Assert.Equal("Passwords must be at least 8 characters.", error.Message);
        _userManager.Verify(u => u.AddToRolesAsync(It.IsAny<CovenantUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_RoleAssignmentFailure_DeletesTheUserWithoutSendingEmails()
    {
        _userManager.Setup(u => u.AddToRolesAsync(It.IsAny<CovenantUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(Failed("RoleError", "Could not add role"));

        Result<Guid> result = await _sut.CreateUser(Model(UserType.Worker));

        Assert.False(result);
        Assert.Equal("RoleError", Assert.Single(result.Errors).Key);
        _userManager.Verify(u => u.DeleteAsync(_created), Times.Once);
        _notifications.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(UserType.AgencyPersonnel)]
    [InlineData(UserType.CompanyUser)]
    public async Task CreateUser_ClaimFailure_DeletesTheUserWithoutRoleNorEmail(UserType type)
    {
        _userManager.Setup(u => u.AddClaimAsync(It.IsAny<CovenantUser>(), It.IsAny<Claim>()))
            .ReturnsAsync(Failed("ClaimError", "Could not add claim"));

        Result<Guid> result = await _sut.CreateUser(Model(type, agencyId: Guid.NewGuid(), companyId: Guid.NewGuid()));

        Assert.False(result);
        Assert.Equal("ClaimError", Assert.Single(result.Errors).Key);
        _userManager.Verify(u => u.DeleteAsync(_created), Times.Once);
        _userManager.Verify(u => u.AddToRolesAsync(It.IsAny<CovenantUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        _notifications.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateUser_SendsTheWelcomeEmailOnlyAfterTheRoleIsAssigned()
    {
        var calls = new List<string>();
        _userManager.Setup(u => u.AddToRolesAsync(It.IsAny<CovenantUser>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => calls.Add("role"))
            .ReturnsAsync(IdentityResult.Success);
        _notifications.Setup(n => n.SendConfirmAndSetPassword(It.IsAny<CovenantUser>()))
            .Callback(() => calls.Add("email"))
            .Returns(Task.CompletedTask);

        Result<Guid> result = await _sut.CreateUser(Model(UserType.CompanyUser, companyId: Guid.NewGuid()));

        Assert.True(result);
        Assert.Equal(["role", "email"], calls);
        _userManager.Verify(u => u.DeleteAsync(It.IsAny<CovenantUser>()), Times.Never);
    }

    [Fact]
    public async Task AddAgencyClaim_AddsTheClaim()
    {
        var agencyId = Guid.NewGuid();
        Claims(IdentityClaims.AgencyId(Guid.NewGuid()));

        Result result = await _sut.AddAgencyClaim(_existing.Id, agencyId);

        Assert.True(result);
        Assert.Equal(agencyId.ToString(), Assert.Single(_addedClaims).Value);
    }

    [Fact]
    public async Task AddAgencyClaim_AlreadyInTheAgency_Fails()
    {
        var agencyId = Guid.NewGuid();
        Claims(IdentityClaims.AgencyId(agencyId));

        Result result = await _sut.AddAgencyClaim(_existing.Id, agencyId);

        Assert.False(result);
        Assert.Empty(_addedClaims);
    }

    [Fact]
    public async Task UserNotFound_Fails_ForEveryUserOperation()
    {
        var unknown = Guid.NewGuid();

        Assert.False(await _sut.AddAgencyClaim(unknown, Guid.NewGuid()));
        Assert.False(await _sut.RemoveClaimOrDeleteUser(unknown, Guid.NewGuid()));
        Assert.False(await _sut.Deactivate(unknown));
        Assert.False(await _sut.ConfirmEmail(unknown, "token"));
        Assert.False(await _sut.UpdateEmail(new UpdateEmailModel(unknown) { NewEmail = "x@sigook.com" }));
        Assert.False(await _sut.UpdateRole(new UpdateRoleModel { Id = unknown, Role = CovenantConstants.Role.Admin }));
    }

    [Fact]
    public async Task RemoveClaimOrDeleteUser_WithSeveralClaims_RemovesOnlyThatClaim()
    {
        var agencyId = Guid.NewGuid();
        Claim claim = IdentityClaims.AgencyId(agencyId);
        Claims(IdentityClaims.AgencyId(Guid.NewGuid()), claim);
        _userManager.Setup(u => u.RemoveClaimAsync(_existing, claim)).ReturnsAsync(IdentityResult.Success);

        Result<bool> result = await _sut.RemoveClaimOrDeleteUser(_existing.Id, agencyId);

        Assert.True(result);
        Assert.False(result.Value);
        _userManager.Verify(u => u.DeleteAsync(It.IsAny<CovenantUser>()), Times.Never);
    }

    [Fact]
    public async Task RemoveClaimOrDeleteUser_WithSeveralClaimsButNotThatOne_Fails()
    {
        Claims(IdentityClaims.AgencyId(Guid.NewGuid()), IdentityClaims.AgencyId(Guid.NewGuid()));

        Result<bool> result = await _sut.RemoveClaimOrDeleteUser(_existing.Id, Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveClaimOrDeleteUser_WithLastClaim_DeletesTheUser()
    {
        Claims(IdentityClaims.AgencyId(Guid.NewGuid()));
        _userManager.Setup(u => u.DeleteAsync(_existing)).ReturnsAsync(IdentityResult.Success);

        Result<bool> result = await _sut.RemoveClaimOrDeleteUser(_existing.Id, Guid.NewGuid());

        Assert.True(result);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Deactivate_MarksTheUserInactiveAndRevokesItsTokens()
    {
        Result result = await _sut.Deactivate(_existing.Id);

        Assert.True(result);
        _repository.Verify(r => r.Create(It.Is<InactiveUser>(i => i.UserId == _existing.Id)), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _repository.Verify(r => r.RevokeTokens(_existing.Id), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_AlreadyConfirmed_SucceedsWithoutUsingTheToken()
    {
        _existing.EmailConfirmed = true;

        Result result = await _sut.ConfirmEmail(_existing.Id, "stale-token");

        Assert.True(result);
        _userManager.Verify(u => u.ConfirmEmailAsync(It.IsAny<CovenantUser>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConfirmEmail_ValidatesTheToken(bool valid)
    {
        _userManager.Setup(u => u.ConfirmEmailAsync(_existing, "token"))
            .ReturnsAsync(valid ? IdentityResult.Success : Failed("InvalidToken", "Invalid token."));

        Result result = await _sut.ConfirmEmail(_existing.Id, "token");

        Assert.Equal(valid, result.IsSuccess);
    }

    [Fact]
    public async Task UpdateEmail_ChangesEmailAndUserName()
    {
        _userManager.Setup(u => u.UpdateAsync(_existing)).ReturnsAsync(IdentityResult.Success);

        Result result = await _sut.UpdateEmail(new UpdateEmailModel(_existing.Id) { NewEmail = "changed@sigook.com" });

        Assert.True(result);
        Assert.Equal("changed@sigook.com", _existing.Email);
        Assert.Equal("changed@sigook.com", _existing.UserName);
    }

    [Fact]
    public async Task UpdateRole_UnknownRole_Fails()
    {
        _roleManager.Setup(r => r.RoleExistsAsync("ghost")).ReturnsAsync(false);

        Result result = await _sut.UpdateRole(new UpdateRoleModel { Id = _existing.Id, Role = "ghost" });

        Assert.False(result);
        _userManager.Verify(u => u.FindByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRole_SameSingleRole_DoesNothing()
    {
        _userManager.Setup(u => u.GetRolesAsync(_existing)).ReturnsAsync([CovenantConstants.Role.Sales]);

        Result result = await _sut.UpdateRole(new UpdateRoleModel { Id = _existing.Id, Role = CovenantConstants.Role.Sales });

        Assert.True(result);
        _userManager.Verify(u => u.RemoveFromRolesAsync(It.IsAny<CovenantUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        _userManager.Verify(u => u.AddToRoleAsync(It.IsAny<CovenantUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRole_ReplacesEveryCurrentRole()
    {
        IList<string> current = [CovenantConstants.Role.Sales, CovenantConstants.Role.Recruiting];
        _userManager.Setup(u => u.GetRolesAsync(_existing)).ReturnsAsync(current);
        _userManager.Setup(u => u.RemoveFromRolesAsync(_existing, current)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(u => u.AddToRoleAsync(_existing, CovenantConstants.Role.Admin)).ReturnsAsync(IdentityResult.Success);

        Result result = await _sut.UpdateRole(new UpdateRoleModel { Id = _existing.Id, Role = CovenantConstants.Role.Admin });

        Assert.True(result);
        _userManager.Verify(u => u.RemoveFromRolesAsync(_existing, current), Times.Once);
        _userManager.Verify(u => u.AddToRoleAsync(_existing, CovenantConstants.Role.Admin), Times.Once);
    }

    [Fact]
    public async Task GetUsersRoles_QueriesEachUserOnce()
    {
        var id = Guid.NewGuid();
        IReadOnlyList<UserRoleModel> roles = [new UserRoleModel(id, CovenantConstants.Role.Admin)];
        _repository.Setup(r => r.GetUsersRoles(It.Is<IEnumerable<Guid>>(ids => ids.SequenceEqual(new[] { id })))).ReturnsAsync(roles);

        var result = await _sut.GetUsersRoles([id, id]);

        Assert.Same(roles, result);
    }
}
