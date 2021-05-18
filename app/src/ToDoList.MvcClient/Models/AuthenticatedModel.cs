namespace ToDoList.MvcClient.Models
{
    public class AuthenticatedModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
