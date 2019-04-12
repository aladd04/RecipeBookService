using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeBookApi.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        //var userIdClaimValue = User.Claims.Single(c => c.Type == "userId").Value;
        //var userId = CryptoFactory.Decrypt("GoogleAuthSecret", userIdClaimValue);
    }
}