using System;
using System.Collections.Generic;

using NetTopologySuite.Geometries;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class TodoItem : BaseEntity
    {
        public DateTime StartDate { get; private set; }
        public DateTime? DueDate { get; set; }
        public Point GeoPoint { get; set; }

        public int? ParentId { get; set; }
        public int StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int ChecklistId { get; set; }
        public int? ImageId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Checklist Checklist { get; set; }
        public virtual Image Image { get; set; }
        public virtual Status Status { get; set; }
        public virtual TodoItem Parent { get; set; }

        public virtual ICollection<TodoItem> Children { get; set; }

        public TodoItem() : base()
        {
            StartDate = DateTime.Now;
            Children = new HashSet<TodoItem>();
        }
    }
}
