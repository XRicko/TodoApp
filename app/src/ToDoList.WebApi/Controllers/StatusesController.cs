using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;

namespace ToDoList.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusesController : Base
    {
        private readonly IDistributedCache cache;

        private readonly string recordKey = "Statuses";

        public StatusesController(IMediator mediator, IDistributedCache distributedCache) : base(mediator)
        {
            cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        [HttpGet]
        public async Task<IEnumerable<StatusResponse>> Get()
        {
            var statuses = await cache.GetRecordAsync<IEnumerable<StatusResponse>>(recordKey);

            if (statuses is null)
            {
                statuses = await Mediator.Send(new GetAllQuery<Status, StatusResponse>());
                await cache.SetRecordAsync(recordKey, statuses, TimeSpan.FromHours(12));
            }

            return statuses;
        }

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<ActionResult<StatusResponse>> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            return await Mediator.Send(new GetByNameQuery<Status, StatusResponse>(name));
        }
    }
}
