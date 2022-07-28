using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Please Update Seed Data and Run Migration Before Running.
builder.Services.AddDbContextFactory<ServerContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
//Load all IService type of Class.
List<Type> services = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => typeof(IService).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => x).ToList();
//Run them as a Hosted Service.
foreach (var type in services)
    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), type));
var app = builder.Build();

app.Run();

