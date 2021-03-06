using System;
using System.Diagnostics.CodeAnalysis;

namespace ToDoList.SharedKernel
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseEntity
    {
        private string name;

        public int Id { get; set; }
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(name), "Name cannot be null");

                name = value;
            }
        }

        protected BaseEntity(string name)
        {
            Name = name;
        }

        protected BaseEntity() { }
    }
}
