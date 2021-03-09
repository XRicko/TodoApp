using System;

namespace ToDoList.MvcClient.Models
{
    public class TodoItemModel
    {
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }

        public string Address { get; set; }

        public int? ParentId { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int ChecklistId { get; set; }
        public int? ImageId { get; set; }

    }
}
