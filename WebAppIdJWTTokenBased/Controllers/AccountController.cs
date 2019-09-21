using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAppIdJWTTokenBased.Models.Identity;

namespace WebAppIdJWTTokenBased.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel registration)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser { Email = registration.Email, UserName = registration.Email };

                IdentityResult result = await _userManager.CreateAsync(user, registration.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }

            }

            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
                if (result.Succeeded)
                {
                    IdentityUser user = await _userManager.FindByEmailAsync(login.Email);
                    JwtSecurityToken token = await GenerateTokenAsync(user);

                    string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(new SuccessfullLoginResult() { Token = serializedToken });
                }
                
            }
            return Unauthorized();
        }

        [HttpGet("email")]
        public ActionResult<string> GetEmail()
        {
            return Ok(User.Identity.Name);//UserName is set to the user´s email
        }

        private async Task<JwtSecurityToken> GenerateTokenAsync(IdentityUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            int expirationDays = _configuration.GetValue<int>("JWTConfiguration:TokenExpirationDays");
            var singingKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTConfiguration:SingingKey"));
            var token = new JwtSecurityToken
            (
                issuer: _configuration.GetValue<string>("JWTConfiguration:Issuer"),
                audience: _configuration.GetValue<string>("JWTConfiguration:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(expirationDays)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(singingKey), SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}