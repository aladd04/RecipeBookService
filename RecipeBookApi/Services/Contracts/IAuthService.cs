using RecipeBookApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeBookApi.Services.Contracts;

internal interface IAuthService
{
    Task<string> Authenticate(string token);
    AppUserClaimModel GetUserFromClaims(ClaimsPrincipal userClaims);
}
