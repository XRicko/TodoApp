using System;

using NetTopologySuite.Geometries;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class TodoItem : BaseEntity
    {
        public DateTime StartDate { get; init; }
        public DateTime? DueDate { get; set; }

        public Point GeoPoint { get; set; }

        public int StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int ChecklistId { get; set; }
        public int? ImageId { get; set; }

        public Category Category { get; set; }
        public Checklist Checklist { get; set; }
        public Image Image { get; set; }
        public Status Status { get; set; }

        public TodoItem() : base()
        {
            StartDate = DateTime.Now;
        }
    }
}
