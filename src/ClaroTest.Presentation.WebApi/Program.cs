using ClaroTest.Core.Application.IOC;
using ClaroTest.Infrastructure.ExternalApi.IOC;
using ClaroTest.Presentation.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios de las capas Application + Infrastructure + WebApi vía extension methods.
builder.Services.AddApplicationLayer();
builder.Services.AddExternalApiInfrastructure(builder.Configuration);
builder.Services.AddCorsExtension(builder.Configuration);
builder.Services.AddApiVersioningExtension();
builder.Services.AddSwaggerExtension();

builder.Services.AddControllers();

var app = builder.Build();

// Middleware global de manejo de errores — debe ser el más externo.
app.UseErrorHandlingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerExtension();
}

app.UseRouting();

app.UseCors(CorsExtension.PolicyName);
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
