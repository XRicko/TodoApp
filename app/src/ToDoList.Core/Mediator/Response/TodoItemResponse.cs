﻿using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Response
{
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, int ChecklistId, string ChecklistName, int StatusId, string StatusName, DateTime? DueDate = null, GeoCoordinate GeoPoint = null, int? CategoryId = null, string CategoryName = null, int? ImageId = null, string ImageName = null, byte[] ImageContent = null) : BaseResponse(Id, Name)
    {
        public string Address { get; init; }
    }
}
