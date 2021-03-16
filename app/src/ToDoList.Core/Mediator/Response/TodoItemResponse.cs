﻿using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Response
{
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int StatusId, string StatusName, int CategoryId, string CategoryName, string ChecklistName, string ImagePath, string Address = null) : BaseResponse(Id, Name);
}
