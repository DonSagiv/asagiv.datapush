using Serilog;
using System;
using System.IO;

namespace asagiv.datapush.common.Utilities
{
    public static class GrpcClientFactory
    {
        public static GrpcClient CreateGprcClient(IServiceProvider serviceProvider = null)
        {
            var logger = serviceProvider?.GetService(typeof(ILogger)) as ILogger;

            var nodeName = "Windows PC";
            var connectionString = "http://localhost:80";

            logger?.Information($"Creating GRPC Client. (Node Name: {nodeName}, Connection String: {connectionString})");

            return new GrpcClient(connectionString, nodeName, GetDeviceId(), logger);
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
