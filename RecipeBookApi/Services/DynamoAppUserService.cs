using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using Google.Apis.Auth;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
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

        public async Task<string> Authenticate(GoogleJsonWebSignature.Payload payload)
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
                    LastName = payload.FamilyName
                };

                var createdId = await _appUserStorage.Create(user, null);

                user.Id = createdId;
            }

            return user.Id;
        }

        public async Task<AppUserViewModel> GetById(string id)
        {
            var recipe = await _appUserStorage.Read(id);
            if (recipe == null)
            {
                return null;
            }

            return CreateAppUserViewModel(recipe);
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