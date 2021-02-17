﻿using MediatR;
using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Queries
{
    public class GetAllQuery<T> : IRequest<IEnumerable<T>> where T : BaseEntity
    {

    }
}