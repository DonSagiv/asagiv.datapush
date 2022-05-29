using asagiv.common.Logging;
using asagiv.pushrocket.server;
using asagiv.pushrocket.server.common.Interfaces;
using asagiv.pushrocket.server.common.Models;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.UseSerilog();
builder.Services.AddSingleton<INodeRepository, NodeRepository>();
builder.Services.AddSingleton<IDataRouteRepository, DataRouteRepository>();
builder.Services.AddSingleton<IRequestHandler, RequestHandler>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DataPushService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
