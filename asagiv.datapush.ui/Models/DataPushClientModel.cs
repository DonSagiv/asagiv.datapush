using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace asagiv.datapush.ui.Models
{
    public class DataPushClientModel : BindableBase
    {
        #region Fields
        public const string serviceName = "Data-Push";
        public const string serviceExecName = "asagiv.datapush.winservice.exe";
        #endregion

        #region Methods
        public async Task InitializeClientAsync()
        {
            var services = await getServicesAsync();

            StartDataPushService(!services.Contains(serviceName));
        }

        private static async Task<IList<string>> getServicesAsync()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"cmd.exe",
                Verb = "runas",
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            processStartInfo.ArgumentList.Add(@"/C net start");

            var process = Process.Start(processStartInfo);

            var reader = process.StandardOutput;

            process.EnableRaisingEvents = true;

            var result = await reader.ReadToEndAsync();

            await process.WaitForExitAsync();

            var serviceList = result
                .Split("\r\n")
                .Where(x => x.StartsWith("   "))
                .Select(x => new string(x.Skip(3).ToArray()))
                .ToList();

            return serviceList;
        }

        public void StartDataPushService(bool installService = false)
        {
            MessageBox.Show("Warning!! Installatin of the Data-Push service requires administrate privilages. If you would to install this service, please click \"Yes\" on the upcoming dialog. Otherwise, click \"No\"",
                "Admin Privileges Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"cmd.exe",
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = false
            };

            if (installService)
            {
                var serviceBinPath = Path.Combine(Directory.GetCurrentDirectory(), serviceExecName);

                processStartInfo.ArgumentList.Add($@"/C sc create {serviceName} binpath={serviceBinPath} start=auto & sc start {serviceName}");
            }
            else
            {
                processStartInfo.ArgumentList.Add($@"/C sc start {serviceName}");
            }
            
            Process.Start(processStartInfo);
        }
        #endregion
    }
}
