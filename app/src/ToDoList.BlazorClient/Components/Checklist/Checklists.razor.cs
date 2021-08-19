using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.Checklist
{
    public partial class Checklists
    {
        private List<ChecklistModel> checklistModels = new();
        private readonly List<Func<Task>> toRunAfterRender = new();

        [CascadingParameter]
        private IModalService Modal { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }
        [Inject]
        private Notifier Notifier { get; set; }

        protected override async Task OnInitializedAsync() =>
            checklistModels = (await ApiInvoker.GetItemsAsync<ChecklistModel>(ApiEndpoints.Checklists)).ToList();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            foreach (var action in toRunAfterRender)
            {
                await action();
            }

            toRunAfterRender.Clear();
        }

        private async Task Add()
        {
            var authState = await AuthenticationStateTask;

            var user = authState.User;
            int userId = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier).Value);

            var checklist = new ChecklistModel { UserId = userId };
            checklistModels.Add(checklist);

            toRunAfterRender.Add(() => Notifier.OnItemAdded(checklist));
        }

        private async Task Delete(int checklistId)
        {
            var result = await Modal.Show<Confirm>("Confirmation").Result;

            if (result.Cancelled)
                return;

            await ApiInvoker.DeleteItemAsync($"{ApiEndpoints.Checklists}/{checklistId}");
            checklistModels.RemoveAll(x => x.Id == checklistId);
        }
    }
}
