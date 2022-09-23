using Amazon.DynamoDBv2.DataModel;
using System;

namespace RecipeBookApi.Dynamo.Models;

[DynamoDBTable("Recipe")]
internal sealed class Recipe : DynamoDocument
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Ingredients { get; set; }
    public string Instructions { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreateDate { get; set; }
    public string UpdatedById { get; set; }
    public DateTime UpdateDate { get; set; }
}
