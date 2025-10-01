using OpenTelemetry;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

Sdk.CreateTracerProviderBuilder()
	.AddAspNetCoreInstrumentation()
	.AddHttpClientInstrumentation()
	.AddConsoleExporter()
	.Build();

var app = builder.Build();
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.MapReverseProxy();
app.Run();
