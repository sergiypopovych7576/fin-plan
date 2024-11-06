using FP.Api.Extensions;
using FP.Api.Filters;
using FP.Application.Interfaces;
using FP.Infrastructure;
using FP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

builder.Services.AddDbContext<BudgetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var domainAssembly = Assembly.Load("FP.Application");
builder.Services.AddAutoMapper(domainAssembly);
builder.RegisterRepositories();
builder.RegisterServices();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => {
	var configuration = builder.Configuration.GetConnectionString("Redis");
	return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddTransient<ICacheService, RedisCacheService>();

builder.Host.UseSerilog();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
