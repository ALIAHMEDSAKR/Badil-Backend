var builder = WebApplication.CreateBuilder(args);

Badil.Api.Extensions.ApiConfigurationExtensions.AddBadilApiConfiguration(builder);


var app = builder.Build();
app.UseExceptionHandler();


if (app.Environment.IsDevelopment())
{
    // Exposes /openapi/v1.json only during local development
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
