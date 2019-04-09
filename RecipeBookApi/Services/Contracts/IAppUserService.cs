using Google.Apis.Auth;
using RecipeBookApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeBookApi.Services.Contracts
{
    public interface IAppUserService
    {
        Task<string> Authenticate(GoogleJsonWebSignature.Payload payload);
        Task<IEnumerable<AppUserViewModel>> GetAll();
    }
}