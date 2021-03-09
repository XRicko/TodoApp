﻿using System.Collections.Generic;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<TodoItemModel> TodoItemModels { get; set; }
        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }
    }
}
