using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.Models
{
    public class DataPushClientModel : BindableBase
    {
        #region Fields
        public const string serviceName = "Data-Push";
        public const string serviceExecName = "asagiv.datapush.winservice.exe";
        private string _nodeName;
        private string _connectionString;
        #endregion

        #region Properties
        public string NodeName
        {
            get { return _nodeName; }
            set { _nodeName = value; RaisePropertyChanged(nameof(NodeName)); }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; RaisePropertyChanged(nameof(ConnectionString)); }
        }
        #endregion

        #region Methods
        public async Task InitializeClientAsync()
        {
            StartDataPushService(await GetServiceStatus());
        }

        private async Task<WinServiceStatus> GetServiceStatus()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"cmd.exe",
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            processStartInfo.ArgumentList.Add(@$"/C sc query {serviceName}");

            var process = Process.Start(processStartInfo);

            var reader = process.StandardOutput;

            process.EnableRaisingEvents = true;

            var result = await reader.ReadToEndAsync();

            await process.WaitForExitAsync();

            return ParseServiceStatus(result);
        }

        public static WinServiceStatus ParseServiceStatus(string serviceQueryResult)
        {
            if (serviceQueryResult.Contains("FAILED 1060"))
            {
                return WinServiceStatus.NotInstalled;
            }

            var stateLine = serviceQueryResult
                .Split("\r\n")
                .FirstOrDefault(x => x.Contains("STATE"));

            if (stateLine.Contains("RUNNING"))
            {
                return WinServiceStatus.Running;
            }
            else if (stateLine.Contains("STOPPED"))
            {
                return WinServiceStatus.Stopped;
            }

            return WinServiceStatus.Error;
        }

        public void StartDataPushService(WinServiceStatus status)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"cmd.exe",
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = false
            };

            switch (status)
            {
                case WinServiceStatus.NotInstalled:
                    var serviceBinPath = Path.Combine(Directory.GetCurrentDirectory(), serviceExecName);
                    processStartInfo.ArgumentList.Add($"/C sc create {serviceName} binpath={serviceBinPath} start=auto & sc start {serviceName}");
                    // processStartInfo.ArgumentList.Add($"/C sc create {serviceName} binpath={serviceBinPath} start=auto & sc start {serviceName} [\"{NodeName}\" \"{ConnectionString}\"]");
                    break;
                case WinServiceStatus.Stopped:
                    processStartInfo.ArgumentList.Add($"/C sc start {serviceName} [\"{ NodeName}\" \"{ConnectionString}\"]");
                    break;
                case WinServiceStatus.Running:
                    processStartInfo.ArgumentList.Add($"/C sc stop {serviceName} & sc start {serviceName} [\"{NodeName}\" \"{ConnectionString}\"]");
                    break;
                case WinServiceStatus.Error:
                    throw new Exception("Invalid Query Status Detected.");
            }

            Process.Start(processStartInfo);
        }
        #endregion
    }
}
