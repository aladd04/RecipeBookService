using Amazon.DynamoDBv2.DataModel;
using RecipeBookApi.Dynamo.Contracts;
using RecipeBookApi.Dynamo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeBookApi.Dynamo;

internal sealed class DynamoStorageRepository<T> : IDynamoStorageRepository<T>
    where T : DynamoDocument
{
    private readonly IDynamoDBContext _context;

    public DynamoStorageRepository(IDynamoDBContext context)
    {
        _context = context;
    }

    public async Task<IList<T>> ReadAll(Func<T, bool> predicate = null)
    {
        var masterResults = new List<T>();

        var query = _context.ScanAsync<T>(null);
        while (!query.IsDone)
        {
            var currentResults = await query.GetNextSetAsync();

            if (predicate is not null)
                currentResults = currentResults.Where(predicate).ToList();

            masterResults.AddRange(currentResults);
        }

        return masterResults;
    }

    public async Task<T> Read(string id)
    {
        return await _context.LoadAsync<T>(id);
    }

    public async Task<string> Create(T obj)
    {
        obj.Id = Guid.NewGuid().ToString();

        await _context.SaveAsync(obj);

        return obj.Id;
    }

    public async Task Update(T obj)
    {
        await _context.SaveAsync(obj);
    }

    public async Task Delete(string id)
    {
        await _context.DeleteAsync<T>(id);
    }
}
