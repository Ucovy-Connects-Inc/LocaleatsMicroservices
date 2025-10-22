using Microsoft.EntityFrameworkCore;
using RestaurantService.Data;
using RestaurantService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext - use local SQL Server connection string (override in env)
var connectionString = builder.Configuration.GetConnectionString("RestaurantDb") ?? "Server=localhost,1433;Database=RestaurantDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// HttpClient for cuisine validation via gateway
builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Gateway:BaseUrl"] ?? "http://localhost:5000");
});
builder.Services.AddScoped<ICuisineValidationClient, CuisineValidationClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
using Microsoft.EntityFrameworkCore;
using RestaurantService.Data;
using RestaurantService.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=RestaurantDb;Trusted_Connection=True;"));

// Http client to call gateway (configure base address via appsettings or env)
builder.Services.AddHttpClient<ICuisineClient, CuisineClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GatewayBaseUrl"] ?? "https://localhost:5000/");
});

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();

app.Run();
using Microsoft.EntityFrameworkCore;
using RestaurantService.Data;
using OpenTelemetry;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Sdk.CreateTracerProviderBuilder()
	.AddAspNetCoreInstrumentation()
	.AddHttpClientInstrumentation()
	.AddConsoleExporter()
	.Build();

// Gateway base address used for inter-service calls
var gatewayBase = builder.Configuration.GetValue<string>("GatewayBaseAddress") ?? "http://gateway:80";
builder.Services.AddHttpClient("GatewayClient", client => client.BaseAddress = new Uri(gatewayBase));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=RestaurantDb;Trusted_Connection=True;";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
