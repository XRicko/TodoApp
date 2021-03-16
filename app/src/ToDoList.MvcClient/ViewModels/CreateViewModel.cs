﻿using System.Collections.Generic;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.ViewModels
{
    public class CreateViewModel
    {
        public TodoItemModel TodoItemModel { get; set; }
        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }
        public IEnumerable<CategoryModel> CategoryModels { get; set; }
        public IEnumerable<StatusModel> StatusModels { get; set; }
    }
}
