﻿using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Status : BaseEntity
    {
        public Status(string name) : base(name) { }

        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}