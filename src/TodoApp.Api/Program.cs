using Amazon.Lambda.Core;
using TodoApp.Api.Endpoints;
using TodoApp.Api.Extensions;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

var builder = WebApplication.CreateSlimBuilder(args);

builder.RegisterServices();

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterTodoEndpoints();

app.Run();