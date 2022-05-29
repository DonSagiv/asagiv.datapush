// See https://aka.ms/new-console-template for more information
using asagiv.pushrocket.common.Models;
using Grpc.Core;
using Grpc.Net.Client;

Console.WriteLine("Hello, World!");

var connectionSettings = new ClientConnectionSettings
{
    ConnectionString = "http://localhost:5000",
    ConnectionName = "TestConnection",
    NodeName = "Computer",
    IsPullNode = false,
};

var channel = GrpcChannel.ForAddress(connectionSettings.ConnectionString);

await channel.ConnectAsync();

var connection = new GrpcClient(connectionSettings, channel, "Test");

await connection.RegisterNodeAsync(false);

Console.ReadKey();