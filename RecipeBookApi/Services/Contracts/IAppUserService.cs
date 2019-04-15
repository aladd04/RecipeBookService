using Google.Apis.Auth;
using RecipeBookApi.Models;
using System.Threading.Tasks;

namespace RecipeBookApi.Services.Contracts
{
    public interface IAppUserService
    {
        Task<AppUserViewModel> Authenticate(GoogleJsonWebSignature.Payload payload);
    }
}