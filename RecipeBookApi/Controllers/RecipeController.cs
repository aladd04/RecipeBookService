using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookApi.Models;
using RecipeBookApi.Services.Contracts;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RecipeBookApi.Controllers
{
    [Route("api/[controller]")]
    public class RecipeController : BaseApiController
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<RecipeViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllRecipes()
        {
            var allRecipes = await _recipeService.GetAll();

            return Ok(allRecipes);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{recipeId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RecipeViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecipeById(string recipeId)
        {
            var foundRecipe = string.IsNullOrWhiteSpace(recipeId) ? null : await _recipeService.GetById(recipeId);
            if (foundRecipe == null)
            {
                return NotFound();
            }

            return Ok(foundRecipe);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Dictionary<string, string[]>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateRecipe([FromBody]RecipePostPutModel data)
        {
            if (data == null)
            {
                ModelState.AddModelError("Body", "No body provided.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdId = await _recipeService.Create(data);
            return Ok(createdId);
        }

        [HttpPut]
        [Route("{recipeId}")]
        [ProducesResponseType(typeof(Dictionary<string, string[]>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRecipe(string recipeId, [FromBody]RecipePostPutModel data)
        {
            if (string.IsNullOrWhiteSpace(recipeId))
            {
                ModelState.AddModelError(nameof(recipeId), "No ID provided to update.");
            }

            if (data == null)
            {
                ModelState.AddModelError("Body", "No body provided.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _recipeService.Update(recipeId, data);
            return Ok();
        }

        [HttpDelete]
        [Route("{recipeId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteRecipe(string recipeId)
        {
            var recipeToDelete = string.IsNullOrWhiteSpace(recipeId) ? null : await _recipeService.GetById(recipeId);
            if (recipeToDelete == null)
            {
                return NotFound();
            }

            await _recipeService.Delete(recipeId);
            return Ok();
        }
    }
}