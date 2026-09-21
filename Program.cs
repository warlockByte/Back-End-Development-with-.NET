var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddSingleton<UserManagementAPI.Services.IUserRepository,
    UserManagementAPI.Services.InMemoryUserRepository>();
builder.Services
    .AddOptions<UserManagementAPI.Configuration.ApiTokenOptions>()
    .Bind(builder.Configuration.GetSection(
        UserManagementAPI.Configuration.ApiTokenOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.UseMiddleware<UserManagementAPI.Middleware.ExceptionHandlingMiddleware>();
app.UseMiddleware<UserManagementAPI.Middleware.TokenAuthenticationMiddleware>();
app.UseMiddleware<UserManagementAPI.Middleware.RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.MapGet("/test-error", () =>
    {
        throw new InvalidOperationException("Middleware test exception.");
    });
}
app.MapControllers();

app.Run();
