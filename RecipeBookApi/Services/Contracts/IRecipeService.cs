using RecipeBookApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeBookApi.Services.Contracts;

internal interface IRecipeService
{
    Task<string> Create(RecipePostPutModel model, string executedById);
    Task Delete(string id, string executedById, bool isAdmin);
    Task<IEnumerable<RecipeViewModel>> GetAll();
    Task<RecipeViewModel> GetById(string id);
    Task Update(string id, RecipePostPutModel model, string executedById, bool isAdmin);
}
