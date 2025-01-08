using TodoApp.Api.Data;

namespace TodoApp.Api.Services;

public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<Todo> CreateAsync(Todo todo);
    Task<bool> UpdateAsync(int id, Todo todo);
    Task<bool> DeleteAsync(int id);
}