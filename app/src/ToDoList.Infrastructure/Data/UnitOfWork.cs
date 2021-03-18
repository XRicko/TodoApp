﻿using System.Threading.Tasks;

using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Infrastructure.Data
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly TodoListContext context;

        public IRepository Repository { get; }

        public UnitOfWork(TodoListContext toDoListContext, IRepository repository)
        {
            context = toDoListContext;
            Repository = repository;
        }

        public Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
