using Microsoft.EntityFrameworkCore;
using User.API.Data;
using User.API.Repository;
using User.API.Repository.Abstractions;
using ToolBox.EntityFramework.Repository;
using User.API.BusinessLogic.Abstractions;
using User.API.BusinessLogic;
using User.API.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging();

// Configure EF Core to use SQL Server
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<UserDbContext>());

builder.Services.AddGenericRepository();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IModelDtoMapper, Mapper>();

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

app.Run();
