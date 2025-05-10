using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Siska.Admin.Application.Services.System;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Errors;

namespace Siska.Admin.Server.Endpoints.System
{
    [ExcludeFromCodeCoverage]
    public static class APIEndpointEndPoints
    {
        public static WebApplication MapAPIEndpointEndPoints(this WebApplication app)
        {
            _ = app.MapPost("api/APIEndpoint/list", APIEndpointEndPointsHandler.GetAllAPIEndpointHandler)
                .WithTags("APIEndpoint")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapGet("api/APIEndpoint", APIEndpointEndPointsHandler.GetAPIEndpointHandler)
                .WithTags("APIEndpoint")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPost("api/APIEndpoint", APIEndpointEndPointsHandler.AddAPIEndpointHandler)
                .WithTags("APIEndpoint")
                .AllowAnonymous()
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPut("api/APIEndpoint", APIEndpointEndPointsHandler.UpdateAPIEndpointHandler)
                .WithTags("APIEndpoint")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapDelete("api/APIEndpoint", APIEndpointEndPointsHandler.DeleteAPIEndpointHandler)
                .WithTags("APIEndpoint")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            return app;
        }
    }

    [ExcludeFromCodeCoverage]
    public class APIEndpointEndPointsHandler
    {
        public static async Task<IResult> GetAPIEndpointHandler(int id,
            IAPIEndpointService apiEndpointService,
            ILogger<APIEndpointEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await apiEndpointService.Get(id, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> DeleteAPIEndpointHandler(int id,
            IAPIEndpointService apiEndpointService,
            ILogger<APIEndpointEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await apiEndpointService.Delete(id, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> UpdateAPIEndpointHandler(APIEndpointDTO apiEndpointDTO,
            IAPIEndpointService apiEndpointService,
            ILogger<APIEndpointEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await apiEndpointService.Update(apiEndpointDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> AddAPIEndpointHandler(APIEndpointDTO apiEndpointDTO,
            IAPIEndpointService apiEndpointService,
            ILogger<APIEndpointEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await apiEndpointService.Add(apiEndpointDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> GetAllAPIEndpointHandler(
            ListDataDTO listDataDTO,
            IAPIEndpointService apiEndpointService,
            ILogger<APIEndpointEndPointsHandler> logger,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var result = await apiEndpointService.GetList(listDataDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }
    }
}
