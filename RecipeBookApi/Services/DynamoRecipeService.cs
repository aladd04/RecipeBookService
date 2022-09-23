using RecipeBookApi.Dynamo.Contracts;
using RecipeBookApi.Dynamo.Models;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeBookApi.Services;

internal sealed class DynamoRecipeService : IRecipeService
{
    private readonly IDynamoStorageRepository<Recipe> _recipeStorage;
    private readonly IDynamoStorageRepository<AppUser> _appUserStorage;
    private readonly IDateTimeService _dateTimeService;

    public DynamoRecipeService(
        IDynamoStorageRepository<Recipe> recipeStorage,
        IDynamoStorageRepository<AppUser> appUserStorage,
        IDateTimeService dateTimeService)
    {
        _recipeStorage = recipeStorage;
        _appUserStorage = appUserStorage;
        _dateTimeService = dateTimeService;
    }

    public async Task<IEnumerable<RecipeViewModel>> GetAll()
    {
        var getAllRecipesTask = _recipeStorage.ReadAll();
        var getAllAppUsersTask = _appUserStorage.ReadAll();

        await Task.WhenAll(getAllRecipesTask, getAllAppUsersTask);
        var (recipes, appUsers) = (getAllRecipesTask.Result, getAllAppUsersTask.Result);

        return recipes.Select(r => CreateRecipeViewModel(r, appUsers.Single(u => u.Id == r.UpdatedById)));
    }

    public async Task<RecipeViewModel> GetById(string id)
    {
        var recipe = await _recipeStorage.Read(id);
        if (recipe is null)
            return null;

        var owner = await _appUserStorage.Read(recipe.UpdatedById);

        return CreateRecipeViewModel(recipe, owner);
    }

    public async Task<string> Create(RecipePostPutModel model, string executedById)
    {
        var newRecipe = new Recipe
        {
            Description = model.Description,
            Ingredients = model.Ingredients,
            Instructions = model.Instructions,
            Name = model.Name,
            CreateDate = _dateTimeService.GetEasternNow(),
            UpdateDate = _dateTimeService.GetEasternNow(),
            CreatedById = executedById,
            UpdatedById = executedById
        };

        return await _recipeStorage.Create(newRecipe);
    }

    public async Task Update(string id, RecipePostPutModel model, string executedById, bool isAdmin)
    {
        var originalRecipe = await _recipeStorage.Read(id);
        if (originalRecipe is null)
            throw new KeyNotFoundException();

        if (!isAdmin && originalRecipe.CreatedById != executedById)
            throw new Exception("You cannot update someone else's recipe");

        originalRecipe.Description = model.Description;
        originalRecipe.Ingredients = model.Ingredients;
        originalRecipe.Instructions = model.Instructions;
        originalRecipe.Name = model.Name;
        originalRecipe.UpdatedById = executedById;
        originalRecipe.UpdateDate = _dateTimeService.GetEasternNow();

        await _recipeStorage.Update(originalRecipe);
    }

    public async Task Delete(string id, string executedById, bool isAdmin)
    {
        var recipe = await _recipeStorage.Read(id);
        if (recipe is null)
            throw new KeyNotFoundException();

        if (!isAdmin && recipe.CreatedById != executedById)
            throw new Exception("You cannot delete someone else's recipe");

        await _recipeStorage.Delete(id);
    }

    private static RecipeViewModel CreateRecipeViewModel(Recipe recipe, AppUser owner)
    {
        return new RecipeViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            Description = recipe.Description,
            OwnerName = owner.FullName,
            UpdateDate = recipe.UpdateDate,
        };
    }
}
