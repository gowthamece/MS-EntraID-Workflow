using EntraID.Workflow.ApiService.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection; // Add this line
using Microsoft.OpenApi.Models; // Add this line

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                builder.Configuration.Bind("AzureAd", options);
                options.Events = new JwtBearerEvents();
            }, options => { builder.Configuration.Bind("AzureAd", options); });

builder.Services.AddControllers();
// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<AppDbContext>("sql");
// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // This line requires the Microsoft.OpenApi.Models namespace

// Add SQL services for AppDbContext


// Allowing CORS for all domains and HTTP methods for the purpose of the sample
// In production, modify this with the actual domains and HTTP methods you want to allow
builder.Services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

//string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

//app.MapGet("/weatherforecast", () => 
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; 
    });
    // app.MapOpenApi();
    using(var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
