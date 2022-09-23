namespace RecipeBookApi.Options;

internal sealed class AppOptions
{
    public string AllowedOrigins { get; set; }
    public bool UseUtcForTokenExpire { get; set; }
    public string EasternTimeZoneId { get; set; }
    public string GoogleClientId { get; set; }
    public string GoogleClientSecret { get; set; }
}
