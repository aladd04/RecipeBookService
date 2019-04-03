﻿namespace RecipeBookApi.Models
{
    public class RecipePutModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Ingredients { get; set; }

        public string Instructions { get; set; }

        public string UpdatedById { get; set; }
    }
}