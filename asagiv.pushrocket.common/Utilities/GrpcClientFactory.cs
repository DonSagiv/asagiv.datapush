using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace asagiv.pushrocket.common.Utilities
{
    public static class GrpcClientFactory
    {
        public static IGrpcClient CreateGprcClient(IServiceProvider serviceProvider = null)
        {
            var logger = serviceProvider?.GetService(typeof(ILogger)) as ILogger;
            var configuration = serviceProvider?.GetService(typeof(IConfiguration)) as IConfiguration;

            logger?.Information("Initializing Grpc Client.");

            if(configuration == null)
            {
                logger?.Warning("Unable to find configuration data.");
            }

            var nodeName = configuration?.GetSection("ClientName")?.Value;
            var connectionString = configuration?.GetSection("GrpcServerAddress")?.Value;

            var clientConnectionSettings = new ClientConnectionSettings
            {
                ConnectionString = connectionString,
                NodeName = nodeName,
            };

            logger?.Information($"Creating GRPC Client. (Node Name: {nodeName}, Connection String: {connectionString})");

            return new GrpcClient(clientConnectionSettings, GetDeviceId(), logger);
        }

        public static string GetDeviceId()
        {
            var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(appDataRoot, "asagiv_datapush");
            var deviceIdFile = Path.Combine(appDataFolder, "deviceId.txt");

            if (File.Exists(deviceIdFile))
            {
                return File.ReadAllText(deviceIdFile);
            }

            Directory.CreateDirectory(appDataFolder);

            var deviceId = Guid.NewGuid().ToString();

            File.WriteAllText(deviceIdFile, deviceId);

            return deviceId;
        }
    }
}
