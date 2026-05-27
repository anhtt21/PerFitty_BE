using Hnanut.PerFitty.Api.Extensions;
using Hnanut.PerFitty.Api.Middleware;
using Hnanut.PerFitty.Application;
using Hnanut.PerFitty.Infrastructure;
using Hnanut.PerFitty.SharedKernel.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/swagger/v1/swagger.json");
    app.MapSwaggerUi();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapHealthChecks("/health");
app.MapGet("/api/health", () =>
    ApiResponse.Success(new
    {
        service = "PerFitty API",
        status = "Healthy",
        utcNow = DateTimeOffset.UtcNow
    }))
    .WithName("GetApiHealth")
    .WithTags("Health");

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
