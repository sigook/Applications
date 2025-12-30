using Covenant.Api.Security.Models;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Security.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityServerService identityServerService;
        private readonly IUserRepository _userRepository;

        public AccountController(
            IIdentityServerService identityServerService,
            IUserRepository userRepository)
        {
            this.identityServerService = identityServerService;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            User.TryGetUserId(out Guid userId);
            Result<CvnEmail> email = CvnEmail.Create(model.NewEmail);
            if (!email) return BadRequest(ModelState.AddErrors(email.Errors));
            User user = await _userRepository.GetUserById(userId);
            if (user is null) return Unauthorized();
            if (user.Email == email.Value.Email)
            {
                ModelState.AddModelError(string.Empty, "Your new email is equal to your current email");
                return BadRequest(ModelState);
            }

            User otherUser = await _userRepository.GetUserByEmail(email.Value.Email);
            if (otherUser != null)
            {
                ModelState.AddModelError(string.Empty, ApiResources.EmailAlreadyTaken);
                return BadRequest(ModelState);
            }

            var result = await identityServerService.UpdateUserEmail(new UpdateEmailModel(userId) { NewEmail = email.Value.Email });
            if (!result)
            {
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetEmail")]
        public async Task<IActionResult> GetEmail()
        {
            User.TryGetUserId(out Guid userId);
            string email = await _userRepository.GetUserEmail(userId);
            return Ok(new { Email = email });
        }

        [HttpGet]
        [Route("Claims")]
        public IActionResult Claims()
        {
            return Ok(User.Claims.Select(s => new
            {
                s.Type,
                s.Value,
                s.Issuer
            }));
        }

        [HttpGet("HashPassword")]
        [AllowAnonymous]
        public IActionResult HashPassword([FromQuery] string password)
        {
            return Ok(identityServerService.HashPassword(password).Value);
        }
    }
}