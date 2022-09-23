using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeBookApi.Dynamo.Contracts;
using RecipeBookApi.Dynamo.Models;
using RecipeBookApi.Factories;
using RecipeBookApi.Models;
using RecipeBookApi.Options;
using RecipeBookApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookApi.Services;

internal sealed class GoogleAuthService : IAuthService
{
    private readonly AppOptions _appOptions;
    private readonly IDateTimeService _dateTimeService;
    private readonly IDynamoStorageRepository<AppUser> _appUserStorage;

    public GoogleAuthService(
        IOptions<AppOptions> appOptions,
        IDateTimeService dateTimeService,
        IDynamoStorageRepository<AppUser> appUserStorage)
    {
        _appOptions = appOptions.Value;
        _dateTimeService = dateTimeService;
        _appUserStorage = appUserStorage;
    }

    public async Task<string> Authenticate(string token)
    {
        var googleAuthPayload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings());
        var user = await UpdateStorageWithUserPayload(googleAuthPayload);
        var userToken = CreateUserToken(user);

        return userToken;
    }

    public AppUserClaimModel GetUserFromClaims(ClaimsPrincipal userClaims)
    {
        return new AppUserClaimModel
        {
            Id = CryptoFactory.Decrypt(_appOptions.GoogleClientSecret, userClaims.FindFirst(nameof(AppUserClaimModel.Id)).Value),
            EmailAddress = userClaims.FindFirst(nameof(AppUserClaimModel.EmailAddress)).Value,
            FirstName = userClaims.FindFirst(nameof(AppUserClaimModel.FirstName)).Value,
            LastName = userClaims.FindFirst(nameof(AppUserClaimModel.LastName)).Value,
            IsAdmin = Convert.ToBoolean(userClaims.FindFirst(nameof(AppUserClaimModel.IsAdmin)).Value)
        };
    }

    private async Task<AppUser> UpdateStorageWithUserPayload(GoogleJsonWebSignature.Payload googleAuthPayload)
    {
        var user = (await _appUserStorage.ReadAll(u => u.EmailAddress.ToLower() == googleAuthPayload.Email.ToLower())).FirstOrDefault();            
        if (user is null)
        {
            user = new AppUser
            {
                EmailAddress = googleAuthPayload.Email,
                FirstName = googleAuthPayload.GivenName,
                LastName = googleAuthPayload.FamilyName,
                LastLoggedInDate = _dateTimeService.GetEasternNow(),
                CreateDate = _dateTimeService.GetEasternNow(),
                UpdateDate = _dateTimeService.GetEasternNow()
            };

            user.Id = await _appUserStorage.Create(user);
        }
        else
        {
            user.LastLoggedInDate = _dateTimeService.GetEasternNow();

            await _appUserStorage.Update(user);
        }

        return user;
    }

    private string CreateUserToken(AppUser appUser)
    {           
        var claims = new List<Claim>
        {
            new Claim(nameof(AppUserClaimModel.Id), CryptoFactory.Encrypt(_appOptions.GoogleClientSecret, appUser.Id)),
            new Claim(nameof(AppUserClaimModel.EmailAddress), appUser.EmailAddress),
            new Claim(nameof(AppUserClaimModel.FirstName), appUser.FirstName),
            new Claim(nameof(AppUserClaimModel.LastName), appUser.LastName),
            new Claim(nameof(AppUserClaimModel.IsAdmin), appUser.IsAdmin.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appOptions.GoogleClientSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(null, null, claims, null, _dateTimeService.GetTokenExpireTime(1), credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
