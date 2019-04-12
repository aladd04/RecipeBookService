using Google.Apis.Auth;
using RecipeBookApi.Models;
using System.Threading.Tasks;

namespace RecipeBookApi.Services.Contracts
{
    public interface IAppUserService
    {
        Task<string> Authenticate(GoogleJsonWebSignature.Payload payload);
        Task<AppUserViewModel> GetById(string id);
    }
}