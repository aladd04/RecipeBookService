using Common.Factories;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeBookApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeBookApi.TokenValidators
{
    public class GoogleTokenValidator : JwtSecurityTokenHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public GoogleTokenValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var principal = base.ValidateToken(securityToken, validationParameters, out validatedToken);
                var token = validatedToken;

                Task.WaitAll(Task.Run(async () =>
                {
                    var payload = await GoogleJsonWebSignature.ValidateAsync(securityToken, new GoogleJsonWebSignature.ValidationSettings());

                    var configurationService = (IConfiguration)_serviceProvider.GetService(typeof(IConfiguration));
                    var appUserService = (IAppUserService)_serviceProvider.GetService(typeof(IAppUserService));
                    var googleAuthSecret = configurationService.GetValue<string>("GoogleAuthSecret");

                    var userId = CryptoFactory.Decrypt(googleAuthSecret, payload.Subject);
                    var user = appUserService.GetById(userId);

                    if (user == null)
                    {
                        principal = null;
                        token = null;
                    }
                    else
                    {
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, payload.Name),
                        new Claim(ClaimTypes.Name, payload.Name),
                        new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                        new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                        new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                        new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer)
                    };

                        principal.AddIdentity(new ClaimsIdentity(claims));
                    }
                }));

                validatedToken = token;
                return principal;
            }
            catch (Exception ex)
            {
                validatedToken = null;
                return null;
            }
        }
    }
}