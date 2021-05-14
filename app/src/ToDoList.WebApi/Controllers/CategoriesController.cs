using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Base
    {
        private readonly IDistributedCache cache;

        private readonly string recordKey = "Categories";

        public CategoriesController(IMediator mediator, IDistributedCache distributedCache) : base(mediator)
        {
            cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryResponse>> Get()
        {
            var categories = await cache.GetRecordAsync<IEnumerable<CategoryResponse>>(recordKey);

            if (categories is null)
            {
                categories = await Mediator.Send(new GetAllQuery<Category, CategoryResponse>());
                await cache.SetRecordAsync(recordKey, categories, TimeSpan.FromMinutes(10));
            }

            return categories;
        }

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<CategoryResponse> GetByName(string name) =>
            await Mediator.Send(new GetByNameQuery<Category, CategoryResponse>(name));

        [HttpPost]
        public async Task Add(CategoryCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));

            await Mediator.Send(new AddCommand<CategoryCreateRequest>(createRequest));
            await cache.RemoveAsync(recordKey);
        }
    }
}