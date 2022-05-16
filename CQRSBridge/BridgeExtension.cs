using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Text.Json;

namespace CQRSBridge
{
    public static class BridgeExtension
    {
  
        public static void UseCQRSBridge(this IApplicationBuilder app, Assembly assembly)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/rpc/{command}",[Authorize]  async (HttpContext context, string command) =>
                {
                    try
                    {

                        var targetTypes = assembly
                                               .GetTypes()
                                               .Where(t => t.GetCustomAttribute<CQRSBridge.Attribute.CommandName>()?.Command.ToLower() == command.ToLower())
                                               .ToList();

                        if (targetTypes.Count() == 0)
                        {
                            return Result<EmptyDto>.Failure(new[] { $"Unable to find command { command }" });
                        }

                        if (targetTypes.Count() > 1)
                        {
                            return Result<EmptyDto>.Failure(new[] { $"Multiple types were found with this command name.  Types : { String.Join(",", targetTypes.Select(s => s.Name)) }" });
                        }

                        var targetType = targetTypes[0];
                        object o;

                        if (int.Parse(context.Request.Headers["content-length"]) > 0)
                        {
                            o = await System.Text.Json.JsonSerializer.DeserializeAsync(context.Request.Body, targetType, serializeOptions);
                        }
                        else
                        {
                            o = Activator.CreateInstance(targetType); ;
                        }

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

            });
         
        }
    }
}
