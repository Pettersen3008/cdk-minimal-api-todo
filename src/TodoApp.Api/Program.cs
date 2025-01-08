using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

var builder = WebApplication.CreateBuilder(args);

// boilerplate code, can we clean this up?
builder.Host.ConfigureServices((_, services) =>
{
    if (!builder.Environment.IsDevelopment())
    {
        services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set.")
        ));
    }
    else
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql("Host=localhost;Port=5432;Database=tododb;User ID=postgres;Password=postgres;Pooling=true;")
        );
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});
app.MapScalarApiReference();

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok("Hello from Todo API!")).WithName("HealthCheck");

// Refactor this to different folder
app.MapGet("/todos", async (AppDbContext db) => await db.Todos.ToListAsync());

app.MapGet("/todos/{id}", async (int id, AppDbContext db) =>
    await db.Todos.FindAsync(id) is Todo todo
        ? Results.Ok(todo)
        : Results.NotFound())
.WithName("GetTodoById");

app.MapPost("/todos", async (Todo todo, AppDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
})
.WithName("CreateTodo");

app.MapPut("/todos/{id}", async (int id, Todo inputTodo, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = inputTodo.Title;
    todo.IsComplete = inputTodo.IsComplete;
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("UpdateTodo");

app.MapDelete("/todos/{id}", async (int id, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.Ok();
})
.WithName("DeleteTodo");

app.Run();

public class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsComplete { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Todo> Todos => Set<Todo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>()
            .ToTable("Todos")
            .HasKey(t => t.Id);
            
        modelBuilder.Entity<Todo>()
            .Property(t => t.Id)
            .UseIdentityColumn();
    }
}