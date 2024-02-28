namespace AspireTodo.Web;

public class TodoApiClient(HttpClient httpClient)
{
    public async Task<TodoItem[]> GetAllTodoItems()
    {
        return await httpClient.GetFromJsonAsync<TodoItem[]>("/todos") ?? [];
    }
}

public record TodoItem(string Description, bool IsCompleted) { }
