using LoginDotnet.Data;
using LoginDotnet.Extensions;
using LoginDotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
var builder = WebApplication.CreateBuilder(args);

// Add file logging
builder.Logging.AddFile("logs/{Date}.log",
    minimumLevel: LogLevel.Information);

ServicesExtension.AddApplicationService(builder.Services, builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
