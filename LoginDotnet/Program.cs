using LoginDotnet.Data;
using LoginDotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<ApplicationContext>(
    options => options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddOpenApi();
builder.Services.AddIdentityCore<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
})
    .AddEntityFrameworkStores<ApplicationContext>();
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
