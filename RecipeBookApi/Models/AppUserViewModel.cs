using System;

namespace RecipeBookApi.Models
{
    public class AppUserViewModel
    {
        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}