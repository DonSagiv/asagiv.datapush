using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Serilog;

namespace asagiv.datapush.ui.Models
{
    public class WindowsServiceSettingsModel : ReactiveObject
    {
        #region Statocs
        public const string serviceName = "Data-Push";
        public const string serviceExecName = "asagiv.datapush.winservice.exe";
        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly string _appDirectory;
        private readonly string _serviceAppSettingsPath;
        private readonly JObject _appSettingsJson;
        private string _nodeName;
        private string _connectionString;
        private string _downloadLocation;
        private WinServiceStatus _status;
        #endregion

        #region Properties
        public string NodeName
        {
            get { return _nodeName; }
            set { this.RaiseAndSetIfChanged(ref _nodeName, value); }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
            set { this.RaiseAndSetIfChanged(ref _connectionString, value); }
        }
        public string DownloadLocation
        {
            get { return _downloadLocation; }
            set { this.RaiseAndSetIfChanged(ref _downloadLocation, value); }
        }
        public WinServiceStatus Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }
        #endregion

        #region Constructor
        public WindowsServiceSettingsModel(ILogger logger)
        {
            _logger = logger;

            _appDirectory = Directory.GetCurrentDirectory();
            _serviceAppSettingsPath = Path.Combine(_appDirectory, "appsettings.json");

            var appSettingsString = File.ReadAllText(_serviceAppSettingsPath);

            _appSettingsJson = JsonConvert.DeserializeObject(appSettingsString) as JObject;

            NodeName = _appSettingsJson["ClientName"].ToObject<string>();
            ConnectionString = _appSettingsJson["GrpcServerAddress"].ToObject<string>();
            DownloadLocation = _appSettingsJson["DownloadPath"].ToObject<string>();
        }
        #endregion

        #region Methods
        public async Task UpdateServiceSettingsAsync()
        {
            _appSettingsJson["ClientName"] = NodeName;
            _appSettingsJson["GrpcServerAddress"] = ConnectionString;
            _appSettingsJson["DownloadPath"] = DownloadLocation;

            var appSettingsString = _appSettingsJson.ToString();

            await File.WriteAllTextAsync(_serviceAppSettingsPath, appSettingsString);
        }

        public void InitializeService()
        {
            StartDataPushService(_status);
        }

        public void StopService()
        {
            StopDataPushService(_status);
        }

        public async Task GetServiceStatus()
        {
            _logger.Verbose("Getting Status of Windows Service");

            // Launch the Command Prompt
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"cmd.exe",
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            // Query the Service Status
            processStartInfo.ArgumentList.Add(@$"/C sc query {serviceName}");

            // Start the Process
            var process = Process.Start(processStartInfo);

            var reader = process.StandardOutput;

            process.EnableRaisingEvents = true;

            // Get results of command prompt
            var result = await reader.ReadToEndAsync();

            await process.WaitForExitAsync();

            // Status needs to be updated on the UI thread.
            await Application.Current?.Dispatcher.BeginInvoke(new Action(() => Status = ParseServiceStatus(result)), System.Windows.Threading.DispatcherPriority.Render);
        }

        private WinServiceStatus ParseServiceStatus(string serviceQueryResult)
        {
            if (serviceQueryResult.Contains("FAILED 1060"))
            {
                _logger?.Verbose("Windows Service Not Installed.");

                return WinServiceStatus.NotInstalled;
            }

            var stateLine = serviceQueryResult
                .Split("\r\n")
                .FirstOrDefault(x => x.Contains("STATE"));

            if (stateLine.Contains("RUNNING"))
            {
                _logger?.Verbose("Windows Service Running.");

                return WinServiceStatus.Running;
            }
            else if (stateLine.Contains("STOPPED"))
            {
                _logger?.Verbose("Windows Service Stopped.");

                return WinServiceStatus.Stopped;
            }

            return WinServiceStatus.Error;
        }

        public void StopDataPushService(WinServiceStatus status)
        {
            _logger?.Warning("Stopping the Windows Service.");

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
                case WinServiceStatus.Stopped:
                    return;
                case WinServiceStatus.Running:
                    processStartInfo.ArgumentList.Add($"/C sc stop {serviceName}");
                    break;
                case WinServiceStatus.Error:
                    throw new ArgumentException("Invalid Query Status Detected.");
            }

            _ = Process.Start(processStartInfo);
        }

        public void StartDataPushService(WinServiceStatus status)
        {
            _logger?.Warning("Starting the Windows Service.");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"cmd.exe",
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = false
            };

            var serviceBinPath = Path.Combine(Directory.GetCurrentDirectory(), serviceExecName);

            switch (status)
            {
                case WinServiceStatus.NotInstalled:
                    processStartInfo.ArgumentList.Add($"/C sc create {serviceName} binpath={serviceBinPath} start=auto " +
                        $"& sc failure {serviceName} reset= 120 actions= restart/120000/restart/120000//" +
                        $"& sc start {serviceName}");
                    break;
                case WinServiceStatus.Stopped:
                    processStartInfo.ArgumentList.Add($"/C sc start {serviceName}");
                    break;
                case WinServiceStatus.Running:
                    processStartInfo.ArgumentList.Add($"/C sc stop {serviceName} & sc start {serviceName}");
                    break;
                case WinServiceStatus.Error:
                    throw new ArgumentException("Invalid Query Status Detected.");
            }

            _ = Process.Start(processStartInfo);
        }
        #endregion
    }
}
