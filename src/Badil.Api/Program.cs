using Badil.Application;
using Badil.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Layer-specific Dependency Injection extensions 
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    // Exposes /openapi/v1.json only during local development
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
