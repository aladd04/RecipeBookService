﻿using Common.Dynamo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Dynamo.Contracts
{
    public interface IDynamoStorageRepository<T> where T : DynamoDocument
    {
        Task<string> Create(T obj);
        Task Delete(string id);
        Task<T> Read(string id);
        Task<IList<T>> ReadAll(Func<T, bool> predicate = null);
        Task Update(T obj);
    }
}
