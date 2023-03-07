using BranchAPI.Messaging;
using BranchAPI.Middlewares.Extensions;
using BranchAPI.Services;
using BranchAPI.Services.Interfaces;
using FluentValidation.AspNetCore;
using System.Reflection;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

//ELK Serilog Configuration
ConfigureLogging();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IMessageProducer, MessageProducer>();

builder.Services.AddSingleton<ICosmosDBService>(options => {
    string url = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("URL");
    string primaryKey = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("PrimaryKey");
    string databaseName= builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("DatabaseName");
    return new CosmosDBService(url, primaryKey, databaseName);
});

builder.Services.AddControllers().AddFluentValidation(options =>
{
    // Validate child properties and root collection elements
    options.ImplicitlyValidateChildProperties = true;
    options.ImplicitlyValidateRootCollectionElements = true;

    // Automatic registration of validators in assembly
    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddConsulConfig(builder.Configuration);

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


app.UseConsul(app.Configuration);
app.MapControllers();
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

