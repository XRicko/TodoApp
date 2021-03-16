﻿using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services
{
    public interface IApiCallsService
    {
        Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel;
        Task<T> GetItemAsync<T>(string routeWithParameters) where T : BaseModel;

        Task PostItemAsync<T>(string route, T item) where T : BaseModel;
        Task PutItemAsync<T>(string route, T item) where T : BaseModel;
        Task DeleteItemAsync(string route, int id);
    }
}
