using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;

namespace RecipeBookApi.Controllers;

[Authorize]
[ApiController]
internal abstract class BaseApiController : ControllerBase
{
    private AppUserClaimModel _currentUser;

    protected IAuthService AuthService { get; }

    protected AppUserClaimModel CurrentUser
    {
        get
        {
            _currentUser ??= AuthService.GetUserFromClaims(User);
            return _currentUser;
        }
    }

    protected BaseApiController(IAuthService authService)
    {
        AuthService = authService;
    }
}
