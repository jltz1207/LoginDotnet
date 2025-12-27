using LoginDotnet.Data;
using LoginDotnet.Extensions;
using LoginDotnet.Models.Entities;
using LoginDotnet.Seed;
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await RoleSeeder.SeedRoles(app.Services);
app.Run();
