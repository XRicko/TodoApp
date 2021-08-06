namespace ToDoList.SharedClientLibrary.Models
{
    public class StatusModel : BaseModel
    {
        public bool IsDone { get; set; }

        public static string GetBackgroundColor(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
                throw new System.ArgumentException($"'{nameof(statusName)}' cannot be null or whitespace.", nameof(statusName));

            return statusName switch
            {
                "Planned" => "bg-planned",
                "Ongoing" => "bg-ongoing",
                _ => "bg-done"
            };
        }
    }
}
