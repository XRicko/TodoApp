namespace ToDoList.SharedClientLibrary
{
    public class ApiEndpoints
    {
        private const string GetByName = "GetByName";
        private const string Authentication = "Authentication";

        public const string Statuses = "Statuses";
        public const string StatusByName = $"{Statuses}/{GetByName}";

        public const string StatusPlanned = $"{Statuses}/{GetByName}/Planned";
        public const string StatusOngoing = $"{Statuses}/{GetByName}/Ongoing";
        public const string StatusDone = $"{Statuses}/{GetByName}/Done";

        public const string Categories = "Categories";
        public const string CategoryByName = $"{Categories}/{GetByName}";

        public const string Projects = "Projects";
        public const string ProjectByName = $"{Projects}/{GetByName}";

        public const string Checklists = "Checklists";
        public const string ChecklistsByProjectId = $"{Checklists}/GetByProjectId";
        public const string ChecklistByProjectIdAndName = $"{ChecklistsByProjectId}AndName";

        public const string Images = "Images";
        public const string ImageByName = $"{Images}/{GetByName}";

        public const string TodoItems = "TodoItems";
        public const string TodoItemsByChecklistId = $"{TodoItems}/GetByChecklistId";

        public const string Register = $"{Authentication}/Register";
        public const string Login = $"{Authentication}/Login";

        public const string Logout = $"{Authentication}/Logout";
        public const string LogoutEverywhere = $"{Authentication}/LogoutEverywhere";

        public const string RefreshToken = $"{Authentication}/Refresh";
    }
}
