using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;
using Task2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Please update seed data and run migration before running.
builder.Services.AddDbContextFactory<ServerContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.Configure<ServiceSetting>(builder.Configuration.GetSection("ServiceSetting"));
//Load all IService type of class.
List<Type> services = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => typeof(IService).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => x).ToList();

//To keep reference of Task2.Services
builder.Services.AddSingleton<LocalService>();

//Run them as a hosted service.

foreach (var type in services)
    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), type));

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();

