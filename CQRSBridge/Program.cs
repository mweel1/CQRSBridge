using CQRSBridge;
using CQRSBridge.Commands.HelloWorld;
using MediatR;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

app.MapPost("/{command}", async (HttpContext context, string command) =>
{
    try
    {

        var targetTypes = Assembly.GetExecutingAssembly()
                               .GetTypes()
                               .Where(t => t.GetCustomAttribute<CQRSBridge.Attribute.CommandName>()?.Command.ToLower() == command.ToLower())
                               .ToList();

        if (targetTypes.Count() == 0)
        {
            return Result<EmptyDto>.Failure(new[] { $"Unable to find command { command }" });
        }

        if (targetTypes.Count() > 1)
        {
            return Result<EmptyDto>.Failure(new[] { $"Multiple types were found with this command name.  Types : { String.Join(",",targetTypes.Select(s=>s.Name)) }" });
        }

        var targetType = targetTypes[0];

        var a = Activator.CreateInstance(targetType);

        var o = await System.Text.Json.JsonSerializer.DeserializeAsync(context.Request.Body, targetType, serializeOptions);

        ISender Mediator = context.RequestServices.GetService<ISender>();

        var result = Mediator.Send(o);

        return result.Result;
    }
    catch (System.Text.Json.JsonException ex)
    {
        return Result<EmptyDto>.Failure(new[] { $"Unable to deserialize the JSON payload." });

    }
    catch (Exception ex)
    {
        return Result<EmptyDto>.Failure(new[] { ex.Message });
    }

});

app.Run();

