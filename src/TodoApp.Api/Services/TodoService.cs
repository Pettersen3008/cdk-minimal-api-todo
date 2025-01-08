using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;

namespace TodoApp.Api.Services;

public class TodoService(AppDbContext db) : ITodoService
{
    public async Task<IEnumerable<Todo>> GetAllAsync()
        => await db.Todos.ToListAsync();

    public async Task<Todo?> GetByIdAsync(int id)
        => await db.Todos.FindAsync(id);

    public async Task<Todo> CreateAsync(Todo todo)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return todo;
    }

    public async Task<bool> UpdateAsync(int id, Todo inputTodo)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null) return false;

        todo.Title = inputTodo.Title;
        todo.IsComplete = inputTodo.IsComplete;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null) return false;

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return true;
    }
}