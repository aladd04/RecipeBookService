using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RecipeBookApi.Controllers;

[Route("api/[controller]")]
internal sealed class AuthController : BaseApiController
{
    public AuthController(IAuthService authService)
        : base(authService)
    { }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AuthModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAuthToken([FromBody]AuthModel authModel)
    {
        try
        {
            var validatedAuthModel = new AuthModel
            {
                Token = await AuthService.Authenticate(authModel.Token)
            };

            return Ok(validatedAuthModel);
        }
        catch (Exception ex)
        {
            return BadRequest($"Issue authenticating: ${ex.Message}");
        }
    }
}
