using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Queries
{
    public class FindQuery<T> : IRequest<IEnumerable<T>> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Predicate { get; }

        public FindQuery(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
        }
    }
}
