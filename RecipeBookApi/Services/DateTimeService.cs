using Microsoft.Extensions.Options;
using RecipeBookApi.Options;
using RecipeBookApi.Services.Contracts;
using System;

namespace RecipeBookApi.Services;

internal sealed class DateTimeService : IDateTimeService
{
    private readonly AppOptions _appOptions;

    public DateTimeService(IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions.Value;
    }

    public DateTime GetEasternNow()
    {
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById(_appOptions.EasternTimeZoneId));
    }

    public DateTime GetTokenExpireTime(int hoursUntilExpire)
    {
        var now = DateTime.UtcNow;
        if (!_appOptions.UseUtcForTokenExpire)
            now = GetEasternNow();

        return now.AddHours(hoursUntilExpire);
    }
}
