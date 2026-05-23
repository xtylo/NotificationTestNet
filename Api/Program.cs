using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using DbSeeder = Infrastructure.Persistance.Seeders.DbSeeder;
using Infrastructure;
using Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    //makes all migrations run
    context.Database.Migrate();

    DbSeeder.Seed(context);
}

app.Run();
