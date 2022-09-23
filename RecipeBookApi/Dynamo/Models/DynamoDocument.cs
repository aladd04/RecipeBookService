using Amazon.DynamoDBv2.DataModel;

namespace RecipeBookApi.Dynamo.Models;

internal abstract class DynamoDocument
{
    [DynamoDBHashKey]
    public string Id { get; set; }
}
