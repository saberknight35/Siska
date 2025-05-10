using Siska.Admin.Server.Endpoints.System;
using Siska.Admin.Server.Extensions;

var builder = WebApplication.CreateBuilder(args).ConfigureBuilder();

var app = builder.Build().ConfigureApplication();

_ = app.MapUsersEndPoints();
_ = app.MapRolesEndPoints();
_ = app.MapAPIEndpointEndPoints();

app.Run();
