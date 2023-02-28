using BranchAPI.Services;
using BranchAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddSingleton<ICosmosDBService>(options => {
    string url = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("URL");
    string primaryKey = builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("PrimaryKey");
    string databaseName= builder.Configuration.GetSection("AzureCosmosDbSettings").GetValue<string>("DatabaseName");
    return new CosmosDBService(url, primaryKey, databaseName);
});
builder.Services.AddControllers();
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

app.MapControllers();

app.Run();
