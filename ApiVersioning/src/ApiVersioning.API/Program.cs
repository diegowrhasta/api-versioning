using ApiVersioning.API;
using ApiVersioning.API.OpenApi;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

var apiVersionSet = app
    .NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();
var groupBuilder = app.MapGroup("api/v{apiVersion:apiVersion}").WithApiVersionSet(apiVersionSet);

groupBuilder.MapWeatherForecastEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // Generates OpenAPI JSON
    app.UseSwaggerUI(options =>
    {
        IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

        foreach (ApiVersionDescription description in descriptions)
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();
            
            options.SwaggerEndpoint(url, name);
        }
    }); // Enables Swagger UI
}

app.UseHttpsRedirection();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record WeatherForecastTwo(DateOnly Date, int TemperatureC, string? Summary, string? ExtraSummary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}