using System;
using System.IO;

namespace asagiv.datapush.common.Utilities
{
    public static class GrpcClientFactory
    {
        public static GrpcClient CreateGprcClient(IServiceProvider serviceProvider)
        {
            return new GrpcClient("http://192.168.4.4:8082", "Windows PC", GetDeviceId());
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
