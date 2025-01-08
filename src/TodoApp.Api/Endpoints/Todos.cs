using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Services;

namespace TodoApp.Api.Endpoints;

public static class Todos
{
    public static void RegisterTodoEndpoints(this IEndpointRouteBuilder routes)
    {
        var todos = routes.MapGroup("/api/v1/todos");

        todos.MapGet("/", async (ITodoService todoService) => 
            await todoService.GetAllAsync());

        todos.MapGet("/{id}", async (int id, ITodoService todoService) =>
                await todoService.GetByIdAsync(id) is Todo todo
                    ? Results.Ok(todo)
                    : Results.NotFound())
            .WithName("GetTodoById");

        todos.MapPost("/", async (Todo todo, ITodoService todoService) =>
            {
                var created = await todoService.CreateAsync(todo);
                return Results.Created($"/todos/{created.Id}", created);
            })
            .WithName("CreateTodo");

        todos.MapPut("/{id}", async (int id, Todo inputTodo, ITodoService todoService) =>
                await todoService.UpdateAsync(id, inputTodo)
                    ? Results.NoContent()
                    : Results.NotFound())
            .WithName("UpdateTodo");

        todos.MapDelete("/{id}", async (int id, ITodoService todoService) =>
                await todoService.DeleteAsync(id)
                    ? Results.Ok()
                    : Results.NotFound())
            .WithName("DeleteTodo");
    }
}