using backend;
using System;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(options =>
{
    string directory = Directory.GetCurrentDirectory();
    IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(directory)
    .AddJsonFile("appsettings.json")
    .Build();
    var connectionString = configuration.GetConnectionString(@"ConnectionStrings");
    Console.WriteLine(connectionString);    
    options.UseSqlServer(connectionString);
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
