using Common.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RecipeBookApi.Models;

namespace RecipeBookApi.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IConfiguration ConfigurationService;

        protected AppUserViewModel CurrentUser
        {
            get
            {
                var googleAuthSecret = ConfigurationService.GetValue<string>("GoogleAuthSecret");

                return new AppUserViewModel
                {
                    Id = CryptoFactory.Decrypt(googleAuthSecret, User.FindFirst(nameof(AppUserViewModel.Id)).Value),
                    EmailAddress = CryptoFactory.Decrypt(googleAuthSecret, User.FindFirst(nameof(AppUserViewModel.EmailAddress)).Value),
                    FirstName = CryptoFactory.Decrypt(googleAuthSecret, User.FindFirst(nameof(AppUserViewModel.FirstName)).Value),
                    LastName = CryptoFactory.Decrypt(googleAuthSecret, User.FindFirst(nameof(AppUserViewModel.LastName)).Value),
                };
            }
        }

        protected bool IsLoggedIn
        {
            get
            {
                try
                {
                    return !string.IsNullOrWhiteSpace(CurrentUser?.Id);
                }
                catch
                {
                    return false;
                }
            }
        }

        protected BaseApiController(IConfiguration configurationService)
        {
            ConfigurationService = configurationService;
        }
    }
}