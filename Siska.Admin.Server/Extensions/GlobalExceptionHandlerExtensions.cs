using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Siska.Admin.Application.Constants;
using Siska.Admin.Application.Exceptions;
using Siska.Admin.Model.Errors;

namespace Siska.Admin.Server.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var appEnv = env;
            var logger = app.ApplicationServices.GetService<ILogger>();

            _ = app.UseExceptionHandler(appError => appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    logger?.LogError(contextFeature.Error.Message, contextFeature.Error);

                    // Set the Http Status Code
                    var statusCode = contextFeature.Error switch
                    {
                        ValidationException ex => HttpStatusCode.BadRequest,
                        NotFoundException ex => HttpStatusCode.NotFound,
                        BaseApplicationException ex => HttpStatusCode.BadRequest,
                        ArgumentException ex => HttpStatusCode.BadRequest,
                        JsonException ex => HttpStatusCode.BadRequest,
                        InvalidOperationException ex => HttpStatusCode.BadRequest,
                        //RpcException ex => GetStatusCodeFromRpcException(ex),
                        _ => HttpStatusCode.InternalServerError
                    };

                    var message = contextFeature.Error switch
                    {
                        ValidationException ex => ex.Message,
                        NotFoundException ex => ex.Message,
                        BaseApplicationException ex => ex.Message,
                        JsonException ex => ex.Message,
                        ArgumentException ex => appEnv.IsDevelopment() ? ex.Message : ExceptionMessages.ServerError,
                        //RpcException ex => GetMessageFromRpcException(ex, appEnv),
                        InvalidOperationException ex => appEnv.IsDevelopment() ? ex.Message : ExceptionMessages.ServerError,
                        Exception ex => appEnv.IsDevelopment() ? ex.Message : ExceptionMessages.ServerError
                    };

                    var innerMessage = appEnv.IsDevelopment() ? contextFeature.Error.InnerException?.Message : null;
                    var stackTrace = appEnv.IsDevelopment() ? contextFeature.Error.StackTrace : null;

                    // Prepare Generic Error
                    var apiError = new ApiError(Activity.Current?.TraceId.ToString(), message, innerMessage, stackTrace);

                    // Set Response Details
                    context.Response.StatusCode = (int)statusCode;
                    context.Response.ContentType = "application/json";

                    // Return the Serialized Generic Error
                    await context.Response.WriteAsync(JsonSerializer.Serialize(apiError));
                }
            }));

            return app;
        }

        /*private static HttpStatusCode GetStatusCodeFromRpcException(RpcException ex)
        {
            var tralier = ex.Trailers.FirstOrDefault(e => Names.StatusCode.Equals(e.Key, StringComparison.InvariantCultureIgnoreCase));
            if (tralier == null || !Enum.TryParse(typeof(HttpStatusCode), tralier.Value, out var statudCode))
            {
                return HttpStatusCode.BadRequest;
            }
            return (HttpStatusCode)statudCode;
        }

        private static string GetMessageFromRpcException(RpcException ex, IWebHostEnvironment env)
        {
            var tralier = ex.Trailers.FirstOrDefault(e => Names.ShowMessageToClient.EqualsInvariantIgnoreCase(e.Key));
            if (tralier != null && bool.TryParse(tralier.Value, out var showToClient))
            {
                if (showToClient)
                {
                    return ex.Message;
                }
            }
            return env.IsDevelopment() ? ex.Message : ExceptionMessages.ServerError;
        }*/
    }
}
