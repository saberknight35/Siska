using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Siska.Admin.Application;
using Siska.Admin.Application.Services.System;
using Siska.Admin.Cache;
using Siska.Admin.Database;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Server.Extensions
{
    public static class ProgramExtensions
    {
        static IServiceProvider serviceProvider;
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            #region Serialisation

            _ = builder.Services.Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                opt.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                opt.SerializerOptions.PropertyNameCaseInsensitive = true;
                opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

            #endregion Serialisation

            #region Swagger

            var ti = CultureInfo.CurrentCulture.TextInfo;

            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    }
                );

                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = $"Siska Admin API - {ti.ToTitleCase(builder.Environment.EnvironmentName)} ",
                        Description = "Siska Admin Service",
                        Contact = new OpenApiContact
                        {
                            Name = "Siska Admin API",
                            Email = "siska.admin@jktbks.dev",
                            Url = new Uri("https://github.com/jktbks/Siska.Admin")
                        },
                        License = new OpenApiLicense()
                        {
                            Name = "Siska Admin API - License - MIT",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        },
                        TermsOfService = new Uri("https://github.com/jktbks/Siska.Admin")
                    });

                options.DocInclusionPredicate((name, api) => true);
            });

            #endregion Swagger

            #region Authentication
            var accessTokenSecret = builder.Configuration["Jwt:AccessTokenSecret"];

            _ = builder.Services
                .AddIdentityCore<Users>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 5;
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
                })
                .AddSignInManager()
                .AddRoles<Roles>()
                .AddEntityFrameworkStores<SiskaDbContext>();

            _ = builder.Services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret)),
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.SaveToken = true;
                });

            _ = builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "apiRole",
                    policy => policy.RequireAuthenticatedUser().RequireAssertion(APIRoleAssertAsync)
                );
            });

            #endregion Authentication

            #region Cache

            _ = builder.Services.AddCaching(builder.Configuration);

            #endregion Cache

            #region Project Dependencies

            _ = builder.Services.AddInfraDb(builder.Configuration);
            _ = builder.Services.AddApplication();

            #endregion Project Dependencies

            _ = builder.Services.AddHttpContextAccessor();

            return builder;
        }

        private static async Task<bool> APIRoleAssertAsync(AuthorizationHandlerContext ctx)
        {
            List<string> userRoles = ctx.User.Claims.Where(q => q.Type == ClaimsIdentity.DefaultRoleClaimType).Select(s => s.Value).ToList();

            HttpContext httpContext = (HttpContext)ctx.Resource;

            var apiPath = httpContext.Request.Path.Value;

            var apiMethod = httpContext.Request.Method;

            using (var scope = serviceProvider.CreateScope())
            {
                var apiEndpoint = scope.ServiceProvider.GetRequiredService<IAPIEndpointService>();

                return await apiEndpoint.IsMatch(apiPath, apiMethod, userRoles);
            }
        }

        public static WebApplication ConfigureApplication(this WebApplication app)
        {
            #region Exceptions

            _ = app.UseGlobalExceptionHandler(app.Environment);

            #endregion Exceptions

            #region Swagger

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
            
            #endregion Swagger

            #region Security

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.UseHttpsRedirection();

            #endregion Security

            #region API Configuration

            _ = app.UseDefaultFiles().UseStaticFiles();

            _ = app.UseHttpsRedirection();

            #endregion API Configuration

            #region run startup task

            _ = app.MapFallbackToFile("/index.html");

            serviceProvider = app.Services;

            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SiskaDbContext>();
                if (db.Database.EnsureCreated())
                {
                    #region Inject Data

                    #region System
                    db.roles.AddRange(new List<Roles> {
                        new() { Id = Guid.Parse("5ecd3fda-ddb4-46e7-bec7-3672053f41ad"), Description = "Administrator for all access", Name = "Admin", NormalizedName = "ADMIN" },
                        new() { Id = Guid.Parse("034a2b79-b19f-46d9-bef2-60ebaa79df1f"), Description = "User access level", Name = "User", NormalizedName = "USER" }
                    });

                    db.SaveChanges();

                    db.users.AddRange(new List<Users> {
                        new() {
                            Id = Guid.Parse("0319176e-1a10-4243-8e06-08113b2ca494"),
                            FullName = "Application Admin",
                            Age = 0,
                            Address = "alamat",
                            DataStatus = 1,
                            UserName = "Admin",
                            NormalizedUserName = "ADMIN",
                            Email = "admin@siska.com",
                            NormalizedEmail = "ADMIN@TRAVEL.COM",
                            EmailConfirmed = false,
                            PasswordHash = "AQAAAAIAAYagAAAAEJNOKCjrDU3XagcMZ3NmFas+x3JvJmCElMBLXZPdd65D+0p7xs6DgOh+COwKqvM6jg==",
                            SecurityStamp = "BEMT7GOPXXFDZL7L6T2VELXTVQOAVJIX",
                            ConcurrencyStamp = "4a1487f5-2651-4e10-8971-e027d77e127b",
                            PhoneNumber = null,
                            PhoneNumberConfirmed = false,
                            TwoFactorEnabled = false,
                            LockoutEnd = null,
                            LockoutEnabled = true,
                            AccessFailedCount = 0,
                            Roles = [
                                db.roles.First(p => p.Name == "Admin"),
                                db.roles.First(p => p.Name == "User")
                            ]
                        }
                    });

                    db.SaveChanges();

                    db.apiEndpoints.AddRange(new List<APIEndpoint> {
                        #region System
                        #region users
                        new() { ApiPath = "/api/users", ApiMethod = "GET", ApiDescription = "Get user", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        new() { ApiPath = "/api/users", ApiMethod = "POST", ApiDescription = "Add user", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/users", ApiMethod = "PUT", ApiDescription = "Edit user", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/users", ApiMethod = "DELETE", ApiDescription = "Delete user", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/users/list", ApiMethod = "POST", ApiDescription = "Get list of user", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        new() { ApiPath = "/api/users/image", ApiMethod = "POST", ApiDescription = "upload user image", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        new() { ApiPath = "/api/users/image", ApiMethod = "GET", ApiDescription = "get user image", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        new() { ApiPath = "/api/signin", ApiMethod = "POST", ApiDescription = "User Sign In", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow },
                        new() { ApiPath = "/api/resetpassword", ApiMethod = "POST", ApiDescription = "Reset user password", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/changepassword", ApiMethod = "POST", ApiDescription = "Change password", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "User")
                        ] },
                        #endregion users
                        #region roles
                        new() { ApiPath = "/api/roles", ApiMethod = "GET", ApiDescription = "Get roles", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        new() { ApiPath = "/api/roles", ApiMethod = "POST", ApiDescription = "Add roles", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/roles", ApiMethod = "PUT", ApiDescription = "Edit roles", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/roles", ApiMethod = "DELETE", ApiDescription = "Delete roles", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/roles/list", ApiMethod = "POST", ApiDescription = "Get list of roles", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        #endregion roles
                        #region APIEndpoint
                        new() { ApiPath = "/api/apiendpoint", ApiMethod = "GET", ApiDescription = "Get APIEndpoint", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        new() { ApiPath = "/api/apiendpoint", ApiMethod = "POST", ApiDescription = "Add APIEndpoint", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/apiendpoint", ApiMethod = "PUT", ApiDescription = "Edit APIEndpoint", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/apiendpoint", ApiMethod = "DELETE", ApiDescription = "Delete APIEndpoint", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin")
                        ] },
                        new() { ApiPath = "/api/apiendpoint/list", ApiMethod = "POST", ApiDescription = "Get list of APIEndpoint", CreatedBy = "System", CreatedDate = DateTime.UtcNow, ModifiedBy = "System", ModifiedDate = DateTime.UtcNow, Roles = [
                            db.roles.First(p => p.Name == "Admin"),
                            db.roles.First(p => p.Name == "User")
                        ] },
                        #endregion APIEndpoint
                        #endregion System
                    });

                    db.SaveChanges();
                    #endregion System

                    #endregion Inject Data
                }
            }

            #endregion run startup task

            return app;
        }
    }
}
