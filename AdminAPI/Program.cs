using AdminAPI.Messaging;
using AdminAPI.Middlewares.Extensions;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//ELK Serilog Configuration
ConfigureLogging();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddScoped<IAdminService, AdminService>();

//Message Consumer (RabbitMQ)
builder.Services.AddSingleton<IMessageConsumer>(options => { 
    string hostName = builder.Configuration.GetSection("MessageCosumerSettings").GetValue<string>("HostName");
    string userName = builder.Configuration.GetSection("MessageCosumerSettings").GetValue<string>("UserName");
    string password = builder.Configuration.GetSection("MessageCosumerSettings").GetValue<string>("Password");
    return new MessageConsumer(hostName, userName, password, options.GetRequiredService<IDistributedCache>());
});

//Cosmos DB Configuration
builder.Services.AddSingleton<ICosmosDBService>(options => {
    string url = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("URL");
    string primaryKey = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("PrimaryKey");
    string databaseName = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("DatabaseName");
    return new CosmosDBService(url, primaryKey, databaseName);
});

//Azure Redis Cache Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("RedisCache").GetValue<string>("ConnectionString");
});

builder.Services.AddControllers();

//Consul Configuration
builder.Services.AddConsulConfig(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Consul Configuration
app.UseConsul(app.Configuration);

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    //Call ConsumeMessage from MessageConsumer to Get Listener Registered 
    var messageConsumer = services.GetRequiredService<IMessageConsumer>();
    messageConsumer.ConsumeMessage("branch");
}
    
app.Run();

//ELK Stack Logging Configuration
void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
    };
}