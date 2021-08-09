using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using Microsoft.AspNetCore.Components;

using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.Checklist
{
    public partial class Checklists
    {
        [CascadingParameter]
        private IModalService Modal { get; set; }

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }

        private IEnumerable<ChecklistModel> checklistModels = Array.Empty<ChecklistModel>();

        protected override async Task OnInitializedAsync() =>
            checklistModels = await ApiInvoker.GetItemsAsync<ChecklistModel>(ApiEndpoints.Checklists);

        private async Task Delete(int checklistId)
        {
            var result = await Modal.Show<Confirm>("Confirmation").Result;

            if (result.Cancelled)
                return;

            await ApiInvoker.DeleteItemAsync($"{ApiEndpoints.Checklists}/{checklistId}");
            checklistModels = await ApiInvoker.GetItemsAsync<ChecklistModel>(ApiEndpoints.Checklists);
        }
    }
}
