namespace RecipeBookApi.Models;

internal sealed class AppUserClaimModel
{
    public string Id { get; set; }
    public string EmailAddress { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsAdmin { get; set; }
}
