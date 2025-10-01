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
