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
using Microsoft.AspNetCore.Http;
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

        /// <summary>Changes the email address of the current authenticated user.</summary>
        /// <param name="model">New email data.</param>
        [HttpPost]
        [Route("ChangeEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>Gets the email address of the current authenticated user.</summary>
        [HttpGet]
        [Route("GetEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmail()
        {
            User.TryGetUserId(out Guid userId);
            string email = await _userRepository.GetUserEmail(userId);
            return Ok(new { Email = email });
        }

        /// <summary>Gets the claims of the current authenticated user.</summary>
        [HttpGet]
        [Route("Claims")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Claims()
        {
            return Ok(User.Claims.Select(s => new
            {
                s.Type,
                s.Value,
                s.Issuer
            }));
        }

        /// <summary>Computes the hash of the given password.</summary>
        /// <param name="password">Plain text password to hash.</param>
        [HttpGet("HashPassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult HashPassword([FromQuery] string password)
        {
            return Ok(identityServerService.HashPassword(password).Value);
        }
    }
}