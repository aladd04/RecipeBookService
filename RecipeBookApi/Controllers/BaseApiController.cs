using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeBookApi.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    { }
}