using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Siska.Admin.Application.Services.System;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Errors;

namespace Siska.Admin.Server.Endpoints.System
{
    [ExcludeFromCodeCoverage]
    public static class RolesEndPoints
    {
        public static WebApplication MapRolesEndPoints(this WebApplication app)
        {

            _ = app.MapPost("api/Roles/list", RolesEndPointsHandler.GetAllRoleHandler)
                .WithTags("Roles")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapGet("api/Roles", RolesEndPointsHandler.GetRoleHandler)
                .WithTags("Roles")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPost("api/Roles", RolesEndPointsHandler.AddRoleHandler)
                .WithTags("Roles")
                .AllowAnonymous()
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPut("api/Roles", RolesEndPointsHandler.UpdateRoleHandler)
                .WithTags("Roles")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapDelete("api/Roles", RolesEndPointsHandler.DeleteRoleHandler)
                .WithTags("Roles")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            return app;
        }
    }

    [ExcludeFromCodeCoverage]
    public class RolesEndPointsHandler
    {
        public static async Task<IResult> GetRoleHandler(Guid id,
            IRoleService roleService,
            ILogger<RolesEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await roleService.Get(id, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> DeleteRoleHandler(Guid id,
            IRoleService roleService,
            ILogger<RolesEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await roleService.Delete(id, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> UpdateRoleHandler(RoleDTO RoleDTO,
            IRoleService roleService,
            ILogger<RolesEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await roleService.Update(RoleDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> AddRoleHandler(RoleDTO RoleDTO,
            IRoleService RoleService,
            ILogger<RolesEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await RoleService.Add(RoleDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> GetAllRoleHandler(
            ListDataDTO listDataDTO,
            IRoleService RoleService,
            ILogger<RolesEndPointsHandler> logger,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var result = await RoleService.GetList(listDataDTO, cancellationToken);

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
