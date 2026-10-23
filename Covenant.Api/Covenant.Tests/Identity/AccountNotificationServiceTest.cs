using Covenant.Common.Entities.Identity;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Identity;
using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Services.Identity;
using Covenant.Core.BL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Covenant.Tests.Identity;

public class AccountNotificationServiceTest
{
    private const string WebClientUrl = "https://app.sigook.test/";

    private readonly Mock<UserManager<CovenantUser>> _userManager = new(Mock.Of<IUserStore<CovenantUser>>(), null, null, null, null, null, null, null, null);
    private readonly Mock<IRazorViewToStringRenderer> _renderer = new();
    private readonly Mock<IEmailService> _email = new();
    private readonly AccountNotificationService _sut;
    private readonly CovenantUser _user = new() { Id = Guid.NewGuid(), Email = "worker@sigook.com" };
    private object _viewModel;
    private string _view;
    private EmailParams _sent;

    public AccountNotificationServiceTest()
    {
        _renderer.Setup(r => r.RenderViewToStringAsync(It.IsAny<string>(), It.IsAny<ConfirmAccountViewModel>()))
            .Callback<string, ConfirmAccountViewModel>((view, model) => { _view = view; _viewModel = model; })
            .ReturnsAsync("<html>confirm</html>");
        _renderer.Setup(r => r.RenderViewToStringAsync(It.IsAny<string>(), It.IsAny<PasswordResetCodeViewModel>()))
            .Callback<string, PasswordResetCodeViewModel>((view, model) => { _view = view; _viewModel = model; })
            .ReturnsAsync("<html>code</html>");
        _email.Setup(e => e.SendEmail(It.IsAny<EmailParams>())).Callback<EmailParams>(p => _sent = p).ReturnsAsync(true);
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["WebClientUrl"] = WebClientUrl })
            .Build();
        _sut = new AccountNotificationService(_userManager.Object, _renderer.Object, _email.Object, configuration, NullLogger<AccountNotificationService>.Instance);
    }

    [Fact]
    public async Task SendConfirmAccount_LinksToTheWebConfirmEmailPage()
    {
        _userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(_user)).ReturnsAsync("a+b/c=");

        await _sut.SendConfirmAccount(_user, "Welcome!");

        var model = Assert.IsType<ConfirmAccountViewModel>(_viewModel);
        Assert.Equal($"https://app.sigook.test/confirm-email?token=a%2Bb%2Fc%3D&id={_user.Id}", model.Url);
        Assert.Equal("Welcome!", model.Message);
        Assert.EndsWith("ConfirmAccount.cshtml", _view);
        Assert.Equal(_user.Email, _sent.Email);
        Assert.Equal("Confirm your account", _sent.Subject);
        Assert.Equal("<html>confirm</html>", _sent.Message);
        Assert.Equal(EmailSettingName.SigookNotification, _sent.EmailSettingName);
    }

    [Fact]
    public async Task SendConfirmAndSetPassword_LinksToTheWebCreatePasswordPage()
    {
        _userManager.Setup(u => u.GeneratePasswordResetTokenAsync(_user)).ReturnsAsync("reset-token");

        await _sut.SendConfirmAndSetPassword(_user);

        var model = Assert.IsType<ConfirmAccountViewModel>(_viewModel);
        Assert.Equal($"https://app.sigook.test/create-password?token=reset-token&id={_user.Id}", model.Url);
        Assert.Contains("create your password", model.Message);
    }

    [Fact]
    public async Task ResendConfirmAccount_UnconfirmedUser_SendsANewLink()
    {
        _userManager.Setup(u => u.FindByEmailAsync(_user.Email)).ReturnsAsync(_user);
        _userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(_user)).ReturnsAsync("token");

        await _sut.ResendConfirmAccount(_user.Email);

        Assert.NotNull(_sent);
        Assert.Contains("confirm-email", Assert.IsType<ConfirmAccountViewModel>(_viewModel).Url);
    }

    [Fact]
    public async Task ResendConfirmAccount_ConfirmedOrUnknownUser_SendsNothing()
    {
        _user.EmailConfirmed = true;
        _userManager.Setup(u => u.FindByEmailAsync(_user.Email)).ReturnsAsync(_user);

        await _sut.ResendConfirmAccount(_user.Email);
        await _sut.ResendConfirmAccount("nobody@sigook.com");

        _email.Verify(e => e.SendEmail(It.IsAny<EmailParams>()), Times.Never);
    }

    [Fact]
    public async Task SendPasswordResetCode_RendersTheCodeAndExpiration()
    {
        await _sut.SendPasswordResetCode(_user, "123456", 15);

        var model = Assert.IsType<PasswordResetCodeViewModel>(_viewModel);
        Assert.Equal("123456", model.Code);
        Assert.Equal(15, model.ExpiresMinutes);
        Assert.EndsWith("PasswordResetCode.cshtml", _view);
        Assert.Equal("Your password reset code", _sent.Subject);
    }

    [Fact]
    public async Task RenderingFailure_IsSwallowed()
    {
        _renderer.Setup(r => r.RenderViewToStringAsync(It.IsAny<string>(), It.IsAny<PasswordResetCodeViewModel>()))
            .ThrowsAsync(new InvalidOperationException("view not found"));

        await _sut.SendPasswordResetCode(_user, "123456", 15);

        _email.Verify(e => e.SendEmail(It.IsAny<EmailParams>()), Times.Never);
    }
}
