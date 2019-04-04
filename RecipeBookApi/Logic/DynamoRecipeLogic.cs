﻿using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using RecipeBookApi.Logic.Contracts;
using RecipeBookApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeBookApi.Logic
{
    public class DynamoRecipeLogic : IRecipeLogic
    {
        private readonly IDynamoStorageRepository<Recipe> _recipeStorage;
        private readonly IDynamoStorageRepository<AppUser> _appUserStorage;

        public DynamoRecipeLogic(IDynamoStorageRepository<Recipe> recipeStorage, IDynamoStorageRepository<AppUser> appUserStorage)
        {
            _recipeStorage = recipeStorage;
            _appUserStorage = appUserStorage;
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
            if (recipe == null)
            {
                return null;
            }

            var owner = await _appUserStorage.Read(recipe.UpdatedById);

            return CreateRecipeViewModel(recipe, owner);
        }

        public async Task<string> Create(RecipePostPutModel model)
        {
            var newRecipe = new Recipe
            {
                Description = model.Description,
                Ingredients = model.Ingredients,
                Instructions = model.Instructions,
                Name = model.Name
            };

            return await _recipeStorage.Create(newRecipe, model.ExecutedById);
        }

        public async Task Update(string id, RecipePostPutModel model)
        {
            var originalRecipe = await _recipeStorage.Read(id);
            var updatedRecipe = new Recipe
            {
                Description = model.Description,
                Ingredients = model.Ingredients,
                Instructions = model.Instructions,
                Name = model.Name
            };

            await _recipeStorage.Update(originalRecipe, updatedRecipe, id, model.ExecutedById);
        }

        public async Task Delete(string id)
        {
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
}