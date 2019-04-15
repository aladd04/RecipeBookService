﻿using Common.Factories;
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
    public class AppUserController : BaseApiController
    {
        private readonly IAppUserService _appUserService;

        public AppUserController(IConfiguration configurationService, IAppUserService appUserService)
            : base(configurationService)
        {
            _appUserService = appUserService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate/google")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GoogleAuthModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTokenUsingGoogleAuthentication([FromBody]GoogleAuthModel googleAuthModel)
        {
            try
            {
                var googleAuthPayload = await GoogleJsonWebSignature.ValidateAsync(googleAuthModel.Token, new GoogleJsonWebSignature.ValidationSettings());
                var user = await _appUserService.Authenticate(googleAuthPayload);

                var googleAuthSecret = ConfigurationService.GetValue<string>("GoogleAuthSecret");
                var claims = new List<Claim>
                {
                    new Claim(nameof(user.Id), CryptoFactory.Encrypt(googleAuthSecret, user.Id)),
                    new Claim(nameof(user.EmailAddress), CryptoFactory.Encrypt(googleAuthSecret, user.EmailAddress)),
                    new Claim(nameof(user.FirstName), CryptoFactory.Encrypt(googleAuthSecret, user.FirstName)),
                    new Claim(nameof(user.LastName), CryptoFactory.Encrypt(googleAuthSecret, user.LastName))
                };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(googleAuthSecret));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(null, null, claims, null, DateTime.Now.AddHours(1), credentials);
                var validatedGoogleAuthModel = new GoogleAuthModel
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };

                return Ok(validatedGoogleAuthModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Issue authenticating with Google: ${ex.Message}");
            }
        }
    }
}