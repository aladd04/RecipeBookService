using System.ComponentModel.DataAnnotations;

namespace RecipeBookApi.Models;

internal sealed class RecipePostPutModel
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    public string Ingredients { get; set; }
    [Required]
    public string Instructions { get; set; }
}
