using asagiv.datapush.common.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Utilities
{
    public class GrpcDataDownloader : IGrpcDataDownloader
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly string _saveDirectory;
        #endregion

        #region Delegates
        public event EventHandler<AcknowledgeDataPullRequest> AcknowledgeDataPush;
        #endregion

        #region Constructor
        public GrpcDataDownloader(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;

            _saveDirectory = configuration.GetSection("DownloadPath")?.Value;

            if (_saveDirectory == null)
            {
                _logger.Warning($"Save Diretory Not Specified.");
            }
            else
            {
                _logger?.Information($"Initializing GRPC Stream Downloader. (Save Directory: {_saveDirectory})");
            }
        }
        #endregion

        public async Task OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext)
        {
            var tempFilePath = Path.Combine(_saveDirectory, $"{responseStreamContext.ResponseData.SourceRequestId}.tmp");

            _logger?.Information($"Streaming Pulled Data to {tempFilePath}");

            try
            {
                if(await DownloadStreamToFileAsync(responseStreamContext, tempFilePath))
                {
                    UpdateFileName(responseStreamContext.ResponseData.Name, tempFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"File Pull Error: {ex.Message}");

                var acknowledgePullDataRequest = new AcknowledgeDataPullRequest
                {
                    RequestId = responseStreamContext.ResponseData.RequestId,
                    Name = responseStreamContext.ResponseData.Name,
                    DestinationNode = responseStreamContext.ResponseData.DestinationNode,
                    BlockNumber = responseStreamContext.ResponseData.BlockNumber,
                    IsPullSuccessful = false,
                    ErrorMessage = ex.Message
                };

                AcknowledgeDataPush?.Invoke(this, acknowledgePullDataRequest);

                File.Delete(tempFilePath);
            }
        }

        private async Task<bool> DownloadStreamToFileAsync(IResponseStreamContext<DataPullResponse> responseStreamContext, string tempFilePath)
        {
            var downloadComplete = false;

            using var fs = new FileStream(tempFilePath, FileMode.Append);
            {
                while (await responseStreamContext.ResponseStream.MoveNext())
                {
                    downloadComplete = await DownloadStreamBlockAsync(responseStreamContext, tempFilePath, fs);
                }
            }

            await fs.DisposeAsync();

            return downloadComplete;
        }

        private async Task<bool> DownloadStreamBlockAsync(IResponseStreamContext<DataPullResponse> responseStreamContext, string tempFilePath, FileStream fs)
        {
            var response = responseStreamContext.ResponseStream.Current;

            var byteArray = response.Payload.ToByteArray();

            _logger?.Information($"Writing {byteArray.Length} Bytes to {tempFilePath} (Block {response.BlockNumber} of {response.TotalBlocks})");

            await fs.WriteAsync(byteArray);

            var acknowledgePullDataRequest = new AcknowledgeDataPullRequest
            {
                RequestId = response.RequestId,
                Name = response.Name,
                DestinationNode = response.DestinationNode,
                IsPullSuccessful = true,
                BlockNumber = response.BlockNumber,
            };

            AcknowledgeDataPush?.Invoke(this, acknowledgePullDataRequest);

            return response.BlockNumber == response.TotalBlocks;
        }

        private static void UpdateFileName(string fileName, string tempFilePath)
        {
            var tempFileDirectory = Path.GetDirectoryName(tempFilePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            var currentFileName = fileName;
            var currentDownloadFilePath = Path.Combine(tempFileDirectory, currentFileName);
            var iteration = 0;

            while (File.Exists(currentDownloadFilePath))
            {
                iteration++;
                currentFileName = $"{fileNameWithoutExtension} ({iteration}){extension}";
                currentDownloadFilePath = Path.Combine(tempFileDirectory, currentFileName);
            }

            File.Move(tempFilePath, currentDownloadFilePath);
        }
    }
}
