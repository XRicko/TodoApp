using System;
using System.ComponentModel.DataAnnotations;

using ToDoList.SharedClientLibrary.Resources;
using ToDoList.SharedKernel;

namespace ToDoList.SharedClientLibrary.Models
{
    public class TodoItemModel : BaseModel
    {
        public DateTime StartDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(Annotations))]
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime? DueDate { get; set; }

        public int DaysAgo => (DateTime.Now.Date - StartDate.Date).Days;

        public GeoCoordinate GeoPoint { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Annotations))]
        public int StatusId { get; set; }

        [Display(Name = "Category", ResourceType = typeof(Annotations))]
        public int? CategoryId { get; set; }

        [Display(Name = "Checklist", ResourceType = typeof(Annotations))]
        public int ChecklistId { get; set; }

        [Display(Name = "Image", ResourceType = typeof(Annotations))]
        public int? ImageId { get; set; }

        public string StatusName { get; set; }
        public string CategoryName { get; set; }
        public string ChecklistName { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageContent { get; set; }

        public string Address { get; set; }
    }
}
