using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace asagiv.datapush.ui.Models
{
    public class DataPushClientModel : BindableBase
    {
        #region Statocs
        public const string serviceName = "Data-Push";
        public const string serviceExecName = "asagiv.datapush.winservice.exe";
        #endregion

        #region Fields
        private readonly string _appDirectory;
        private readonly string _serviceAppSettingsPath;
        private readonly JObject _appSettingsJson;
        private string _nodeName;
        private string _connectionString;
        private string _downloadLocation;
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
        public string DownloadLocation
        {
            get { return _downloadLocation; }
            set { _downloadLocation = value; RaisePropertyChanged(nameof(DownloadLocation)); }
        }
        #endregion

        #region Constructor
        public DataPushClientModel()
        {
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

        public async static Task InitializeClientAsync()
        {
            StartDataPushService(await GetServiceStatus());
        }

        private async static Task<WinServiceStatus> GetServiceStatus()
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

        private static WinServiceStatus ParseServiceStatus(string serviceQueryResult)
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

        public static void StartDataPushService(WinServiceStatus status)
        {
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

            Process.Start(processStartInfo);
        }
        #endregion
    }
}
