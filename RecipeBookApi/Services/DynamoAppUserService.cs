using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using Google.Apis.Auth;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
            //var appUsers = await _appUserStorage.ReadAll();

            //var user = appUsers.SingleOrDefault(u => u.EmailAddress == payload.Email);
            //if (user == null)
            //{
            //    user = new AppUser
            //    {
            //        EmailAddress = payload.Email,
            //        OauthSubject = payload.Subject,
            //        OauthIssuer = payload.Issuer,
            //        FirstName = payload.GivenName,
            //        LastName = payload.FamilyName
            //    };

            //    var createdId = _appUserStorage.Create(user, null);
            //}
        }

        public async Task<IEnumerable<AppUserViewModel>> GetAll()
        {
            var appUsers = await _appUserStorage.ReadAll();

            return appUsers.Select(u => CreateAppUserViewModel(u));
        }

        private static AppUserViewModel CreateAppUserViewModel(AppUser appUser)
        {
            return new AppUserViewModel
            {
                Id = appUser.Id,
                EmailAddress = appUser.EmailAddress,
                FirstName = appUser.FirstName,
                LastLoggedInDate = appUser.LastLoggedInDate,
                LastName = appUser.LastName
            };
        }
    }
}