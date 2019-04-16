using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using Common.Extensions;
using Common.Factories;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookApi.Services
{
    public class GoogleAuthService : IAuthService
    {
        private readonly IConfiguration _configurationService;
        private readonly IDynamoStorageRepository<AppUser> _appUserStorage;

        public GoogleAuthService(IConfiguration configurationService, IDynamoStorageRepository<AppUser> appUserStorage)
        {
            _configurationService = configurationService;
            _appUserStorage = appUserStorage;
        }

        public async Task<string> Authenticate(string token)
        {
            var googleAuthPayload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings());

            var user = await UpdateStorageWithUserPayload(googleAuthPayload);

            var userToken = CreateUserToken(user);

            return userToken;
        }

        public AppUserClaimModel GetUserFromClaims(ClaimsPrincipal userClaims)
        {
            var googleAuthSecret = _configurationService.GetValue<string>("GoogleAuthSecret");

            return new AppUserClaimModel
            {
                Id = CryptoFactory.Decrypt(googleAuthSecret, userClaims.FindFirst(nameof(AppUserClaimModel.Id)).Value),
                EmailAddress = userClaims.FindFirst(nameof(AppUserClaimModel.EmailAddress)).Value,
                FirstName = userClaims.FindFirst(nameof(AppUserClaimModel.FirstName)).Value,
                LastName = userClaims.FindFirst(nameof(AppUserClaimModel.LastName)).Value,
            };
        }

        private async Task<AppUser> UpdateStorageWithUserPayload(GoogleJsonWebSignature.Payload googleAuthPayload)
        {
            var appUsers = await _appUserStorage.ReadAll();
            var user = appUsers.SingleOrDefault(u => u.EmailAddress.ToLower() == googleAuthPayload.Email.ToLower());

            if (user == null)
            {
                user = new AppUser
                {
                    EmailAddress = googleAuthPayload.Email,
                    FirstName = googleAuthPayload.GivenName,
                    LastName = googleAuthPayload.FamilyName,
                    LastLoggedInDate = DateTime.Now.ToEasternStandardTime()
                };

                user.Id = await _appUserStorage.Create(user, null);
            }
            else
            {
                user.LastLoggedInDate = DateTime.Now.ToEasternStandardTime();

                await _appUserStorage.Update(user, user, user.Id, null);
            }

            return user;
        }

        private string CreateUserToken(AppUser appUser)
        {
            var googleAuthSecret = _configurationService.GetValue<string>("GoogleAuthSecret");
            var claims = new List<Claim>
            {
                new Claim(nameof(appUser.Id), CryptoFactory.Encrypt(googleAuthSecret, appUser.Id)),
                new Claim(nameof(appUser.EmailAddress), googleAuthSecret, appUser.EmailAddress),
                new Claim(nameof(appUser.FirstName), googleAuthSecret, appUser.FirstName),
                new Claim(nameof(appUser.LastName), googleAuthSecret, appUser.LastName)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(googleAuthSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(null, null, claims, null, DateTime.Now.AddHours(1), credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}