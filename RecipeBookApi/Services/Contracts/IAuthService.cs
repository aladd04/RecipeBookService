using RecipeBookApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeBookApi.Services.Contracts
{
    public interface IAuthService
    {
        Task<string> Authenticate(string token);
        AppUserClaimModel GetUserFromClaims(ClaimsPrincipal userClaims);
    }
}