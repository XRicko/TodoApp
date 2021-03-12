using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using ToDoList.SharedKernel;

namespace ToDoList.MvcClient.Models
{
    public class TodoItemModel : BaseModel
    {
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime? DueDate { get; set; }
        public GeoCoordinate GeoPoint { get; set; }

        public int? ParentId { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int ChecklistId { get; set; }
        public int? ImageId { get; set; }

        public string StatusName { get; set; }
        public string CategoryName { get; set; }
        public string ChecklistName { get; set; }
        public string ImagePath { get; set; }

        public IFormFile Image { get; set; }

        public string Address { get; set; }
    }
}
