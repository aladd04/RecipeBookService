using System;

namespace RecipeBookApi.Services.Contracts;

internal interface IDateTimeService
{
    DateTime GetEasternNow();
    DateTime GetTokenExpireTime(int hoursUntilExpire);
}
