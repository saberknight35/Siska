using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Siska.Admin.Application.Services.System;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Errors;

namespace Siska.Admin.Server.Endpoints.System
{
    [ExcludeFromCodeCoverage]
    public static class UsersEndPoints
    {
        public static WebApplication MapUsersEndPoints(this WebApplication app)
        {
            _ = app.MapPost("api/signin", UsersEndPointsHandler.SignIn)
                .WithTags("Auth")
                .AllowAnonymous()
                .WithName("SignIn")
                .WithOpenApi();

            _ = app.MapPost("api/resetPassword", UsersEndPointsHandler.resetPassword)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPost("api/changePassword", UsersEndPointsHandler.changePassword)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPost("api/Users/list", UsersEndPointsHandler.GetAllUserHandler)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapGet("api/Users", UsersEndPointsHandler.GetUserHandler)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPost("api/Users", UsersEndPointsHandler.AddUserHandler)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPut("api/Users", UsersEndPointsHandler.UpdateUserHandler)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapDelete("api/Users", UsersEndPointsHandler.DeleteUserHandler)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapPost("api/Users/image", UsersEndPointsHandler.AddUserImageHandler)
                .DisableAntiforgery()
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            _ = app.MapGet("api/Users/image", UsersEndPointsHandler.GetUserImageHandler)
                .WithTags("Users")
                .RequireAuthorization("apiRole")
                .WithOpenApi();

            return app;
        }
    }

    [ExcludeFromCodeCoverage]
    public class UsersEndPointsHandler
    {
        public static async Task<IResult> SignIn(UserLoginDTO loginDTO,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.SignInAsync(loginDTO);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);


                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> resetPassword(ResetPasswordDTO resetPasswordDTO,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.ResetPasswordAsync(resetPasswordDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> changePassword(ChangePasswordDTO changePasswordDTO,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.ChangePasswordAsync(changePasswordDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> GetUserHandler(Guid id,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.Get(id, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> DeleteUserHandler(Guid id,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.Delete(id, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> UpdateUserHandler(UserDTO UserDTO,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.Update(UserDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> AddUserHandler(UserDTO UserDTO,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.Add(UserDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> GetAllUserHandler(
            ListDataDTO listDataDTO,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var result = await userService.GetList(listDataDTO, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> AddUserImageHandler(
            Guid id,
            [FromForm] IFormFile formFile,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                var stream = new MemoryStream();
                formFile.CopyTo(stream);

                var fileName = formFile.FileName;

                var result = await userService.UploadImage(id, stream, fileName, cancellationToken);

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }

        public static async Task<IResult> GetUserImageHandler(Guid id,
            IUserService userService,
            ILogger<UsersEndPointsHandler> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                (var result, var fileName) = await userService.GetImage(id, cancellationToken);

                if (result == null)
                    return Results.Empty;

                var ext = Path.GetExtension(fileName).Replace(".", "");
                if (ext == "jpg")
                    ext = "jpeg";

                var mimeType = "image/" + ext; // Replace with the appropriate MIME type for your file

                return Results.File(result, mimeType, fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Results.BadRequest(new ApiError(Activity.Current?.TraceId.ToString(), ex.Message, ex.InnerException?.Message, ex.StackTrace));
            }
        }
    }
}
