using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary.Models;

using ToDoList.SharedClientLibrary;
using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Services;
using ToDoList.BlazorClient.State;

namespace ToDoList.BlazorClient.Components.Projects
{
    public partial class Projects : IDisposable
    {
        private readonly List<Func<Task>> toRunAfterRender = new();

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [CascadingParameter]
        private IModalService Modal { get; set; }

        [Inject]
        private Notifier Notifier { get; set; }

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }

        [Inject]
        private StateContainer State { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!State.Projects.Any())
                await LoadProjects();

            Notifier.FilterChosen += OnSearch;
        }

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

            var project = State.Projects.Find(x => x.Id == 0);

            if (project is null)
            {
                project = new ProjectModel { UserId = userId };
                State.Projects.Add(project);
            }

            toRunAfterRender.Add(() => Notifier.OnItemAdded(project));
        }

        private async Task Delete(int projectId)
        {
            if (projectId > 0)
            {
                var result = await Modal.Show<Confirm>("Confirmation").Result;

                if (result.Cancelled)
                    return;

                await ApiInvoker.DeleteItemAsync($"{ApiEndpoints.Projects}/{projectId}");
            }

            State.Projects.RemoveAll(x => x.Id == projectId);
        }

        private async Task OnSearch()
        {
            await LoadProjects();

            if (!string.IsNullOrWhiteSpace(State.SearchTerm))
                State.Projects = State.Projects.FindAll(x => x.Name.Contains(State.SearchTerm, StringComparison.OrdinalIgnoreCase));

            StateHasChanged();
        }

        private async Task LoadProjects() => State.Projects = (await ApiInvoker.GetItemsAsync<ProjectModel>(ApiEndpoints.Projects)).ToList();

        public void Dispose() => Notifier.FilterChosen -= OnSearch;
    }
}
