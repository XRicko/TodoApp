using System;
using System.ComponentModel.DataAnnotations;

using ToDoList.SharedClientLibrary.Resources.Validaton;
using ToDoList.SharedKernel;

namespace ToDoList.SharedClientLibrary.Models
{
    public class TodoItemModel : BaseModel
    {
        public DateTime StartDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(AnnotationResources))]
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime? DueDate { get; set; }

        public int DaysAgo => (DateTime.Now.Date - StartDate.Date).Days;

        public GeoCoordinate GeoPoint { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public int StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int ChecklistId { get; set; }
        public int? ImageId { get; set; }

        public string StatusName { get; set; }
        public string CategoryName { get; set; }
        public string ChecklistName { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }

        public string Address { get; set; }
    }
}
