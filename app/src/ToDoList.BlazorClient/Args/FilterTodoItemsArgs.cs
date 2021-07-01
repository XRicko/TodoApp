namespace ToDoList.BlazorClient.Args
{
    public class FilterTodoItemsArgs
    {
        public string StatusName { get; set; }
        public string CategoryName { get; set; }

        public FilterTodoItemsArgs()
        {

        }

        public FilterTodoItemsArgs(string statusName, string categoryName)
        {
            StatusName = statusName;
            CategoryName = categoryName;
        }
    }
}
