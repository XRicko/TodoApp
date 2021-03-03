﻿using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Queries;
using ToDoList.Core.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public StatusesController(IMediator m, IMapper map)
        {
            mediator = m;
            mapper = map;
        }

        [HttpGet]
        public async Task<IEnumerable<StatusResponse>> Get()
        {
            IEnumerable<Status> statuses = await mediator.Send(new GetAllQuery<Status>());
            return mapper.Map<IEnumerable<StatusResponse>>(statuses);
        }

        [HttpGet("{id}")]
        public async Task<StatusResponse> Get(int id)
        {
            Status status = await mediator.Send(new GetByIdQuery<Status>(id));
            return mapper.Map<StatusResponse>(status);
        }
    }
}
