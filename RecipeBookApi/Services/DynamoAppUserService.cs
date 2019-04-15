using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using Common.Extensions;
using Google.Apis.Auth;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeBookApi.Services
{
    public class DynamoAppUserLogic : IAppUserService
    {
        private readonly IDynamoStorageRepository<AppUser> _appUserStorage;

        public DynamoAppUserLogic(IDynamoStorageRepository<AppUser> appUserStorage)
        {
            _appUserStorage = appUserStorage;
        }

        public async Task<AppUserViewModel> Authenticate(GoogleJsonWebSignature.Payload payload)
        {
            var appUsers = await _appUserStorage.ReadAll();

            var user = appUsers.SingleOrDefault(u => u.EmailAddress.ToLower() == payload.Email.ToLower());
            if (user == null)
            {
                user = new AppUser
                {
                    EmailAddress = payload.Email,
                    OauthSubject = payload.Subject,
                    OauthIssuer = payload.Issuer,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    LastLoggedInDate = DateTime.Now.ToEasternStandardTime()
                };

                user.Id = await _appUserStorage.Create(user, null);
            }
            else
            {
                user.LastLoggedInDate = DateTime.Now.ToEasternStandardTime();

                await _appUserStorage.Update(user, user, user.Id, null);
            }

            return CreateAppUserViewModel(user);
        }

        private static AppUserViewModel CreateAppUserViewModel(AppUser appUser)
        {
            return new AppUserViewModel
            {
                Id = appUser.Id,
                EmailAddress = appUser.EmailAddress,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName
            };
        }
    }
}