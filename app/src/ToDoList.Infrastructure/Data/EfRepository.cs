﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Infrastructure.Data
{
    class EfRepository : IRepository
    {
        private readonly ToDoListContext context;

        public EfRepository(ToDoListContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<T> GetAsync<T>(int id) where T : BaseEntity =>
            context.Set<T>().SingleOrDefaultAsync(x => x.Id == id);

        public Task<T> GetAsync<T>(string name) where T : BaseEntity =>
            context.Set<T>().SingleOrDefaultAsync(x => x.Name == name);

        public Task<T> GetAsync<T>(T entity) where T : BaseEntity =>
            context.Set<T>().SingleOrDefaultAsync(x => x.Name == entity.Name);

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : BaseEntity =>
            await context.Set<T>().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity =>
            await context.Set<T>().Where(predicate).ToListAsync();

        public async Task AddAsync<T>(T entity) where T : BaseEntity =>
            await context.Set<T>().AddAsync(entity);

        public void Update<T>(T entity) where T : BaseEntity
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove<T>(T entity) where T : BaseEntity =>
            context.Set<T>().Remove(entity);
    }

}