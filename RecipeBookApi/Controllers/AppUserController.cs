using Common.Factories;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAppUserService _appUserService;

        public AppUserController(IConfiguration configuration, IAppUserService appUserService)
        {
            _configuration = configuration;
            _appUserService = appUserService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate/google")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTokenUsingGoogleAuthentication([FromBody]string userToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(userToken, new GoogleJsonWebSignature.ValidationSettings());
                var user = await _appUserService.Authenticate(payload);

                var emailEncryptionKey = _configuration.GetValue<string>("EmailEncryptionKey");
                var jwtSecret = _configuration.GetValue<string>("JwtSecret");

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, CryptoFactory.Encrypt(emailEncryptionKey, user.EmailAddress)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var returnToken = new JwtSecurityToken(null, null, claims, null, DateTime.Now.AddHours(1), credentials);
                var returnTokenString = new JwtSecurityTokenHandler().WriteToken(returnToken);

                return Ok(returnTokenString);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<AppUserViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAppUsers()
        {
            var allAppUsers = await _appUserService.GetAll();

            return Ok(allAppUsers);
        }
    }
}