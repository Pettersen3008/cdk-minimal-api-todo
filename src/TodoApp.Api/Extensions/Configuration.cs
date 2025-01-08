using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApp.Api.Data;
using TodoApp.Api.Services;

namespace TodoApp.Api.Extensions;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Host=localhost;Port=5432;Database=tododb;User ID=postgres;Password=postgres;Pooling=true;" ;
        
        builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        
        builder.Services.AddScoped<ITodoService, TodoService>();
        builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        app.UseSwagger(options => { options.RouteTemplate = "/openapi/{documentName}.json"; });
        app.MapScalarApiReference();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        app.UseHttpsRedirection();
    }
}