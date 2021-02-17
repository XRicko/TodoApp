﻿using System;
using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class ChecklistItem : BaseEntity
    {
        public DateTime StartDate { get; private set; }
        public DateTime? DueDate { get; set; }
        public int? ParentId { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int ChecklistId { get; set; }
        public int? ImageId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Checklist Checklist { get; set; }
        public virtual Image Image { get; set; }
        public virtual Status Status { get; set; }
        public virtual ChecklistItem Parent { get; set; }
        public virtual ICollection<ChecklistItem> Children { get; set; }

        public ChecklistItem(string name, int checklistId, DateTime? dueDate = null, int? categoryId = null, int? parentId = null, int? imageId = null, int? statusId = null) : base(name)
        {
            StartDate = DateTime.Now;

            DueDate = dueDate;
            CategoryId = categoryId;
            ChecklistId = checklistId;
            ParentId = parentId;
            ImageId = imageId;
            StatusId = statusId;
        }
    }
}