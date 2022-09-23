using System;

namespace RecipeBookApi.Models;

internal sealed class RecipeViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Ingredients { get; set; }
    public string Instructions { get; set; }
    public string Description { get; set; }
    public string OwnerName { get; set; }
    public DateTime UpdateDate { get; set; }
}
