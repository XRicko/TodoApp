﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using Microsoft.AspNetCore.Components;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Pages
{
    public partial class Checklists
    {
        private List<ChecklistModel> checklistModels = new();
        private readonly List<Func<Task>> toRunAfterRender = new();

        [CascadingParameter]
        private IModalService Modal { get; set; }

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }
        [Inject]
        private Notifier Notifier { get; set; }

        [Parameter]
        public int ProjectId { get; set; }

        protected override async Task OnParametersSetAsync() => await LoadChecklists();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            foreach (var action in toRunAfterRender)
            {
                await action();
            }

            toRunAfterRender.Clear();
        }

        private void Add()
        {
            var checklist = checklistModels.Find(x => x.Id == 0);

            if (checklist is null)
            {
                checklist = new ChecklistModel { ProjectId = ProjectId };
                checklistModels.Add(checklist);
            }

            toRunAfterRender.Add(() => Notifier.OnItemAdded(checklist));
        }

        private async Task Delete(int checklistId)
        {
            if (checklistId > 0)
            {
                var result = await Modal.Show<Confirm>("Confirmation").Result;

                if (result.Cancelled)
                    return;

                await ApiInvoker.DeleteItemAsync($"{ApiEndpoints.Checklists}/{checklistId}");
            }

            await LoadChecklists();
        }

        private async Task LoadChecklists() =>
            checklistModels = (await ApiInvoker.GetItemsAsync<ChecklistModel>($"{ApiEndpoints.ChecklistsByProjectId}/{ProjectId}")).ToList();
    }
}
