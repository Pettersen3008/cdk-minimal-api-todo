using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApp.Api.Data;
using TodoApp.Api.Services;

namespace TodoApp.Api.Extensions;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
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
        builder.Services.AddScoped<ITodoService, TodoService>();
        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options => { options.RouteTemplate = "/openapi/{documentName}.json"; }).UseSwaggerUI();
            app.UseDeveloperExceptionPage();
            app.MapScalarApiReference();
        }

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        app.UseHttpsRedirection();
    }
}