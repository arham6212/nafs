using Nafs.Api.ExceptionHandling;
using Nafs.Api.Models;
using Nafs.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IReflectionService, ReflectionService>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        application = "Nafs.Api",
        timestamp = DateTime.UtcNow
    });
});


app.MapGet("/api/reflections/{id:int}",
    async (int id, IReflectionService reflectionService, CancellationToken cancellationToken) =>
    {
        var reflection = await reflectionService.GetByIdAsync(id, cancellationToken);
        if (reflection is null)
        {
            return
                Results.NotFound();
        }

        return Results.Ok(reflection);
    });


app.MapGet("/api/reflections",
    async (string? search,int? page, int? pageSize, IReflectionService reflectionService, CancellationToken cancellationToken) =>
    {
        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 20;
        var reflections = await reflectionService.GetAllAsync(search,currentPage, currentPageSize, cancellationToken);
        return Results.Ok(reflections);
    });

app.MapPost("/api/reflections/", async (IReflectionService reflectionService, CreateReflectionRequest request,
    CancellationToken cancellationToken) =>
{
    var reflection = await reflectionService.CreateReflectionAsync(request.Text, cancellationToken);
    return Results.Created(
        $"/api/reflections/{reflection.Id}",
        reflection);
});

app.MapGet("/api/service-status",
    (IReflectionService reflectionService) => { return Results.Ok(reflectionService.GetServiceStatus()); });
app.Run();