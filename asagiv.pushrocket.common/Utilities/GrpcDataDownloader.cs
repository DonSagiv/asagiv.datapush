using asagiv.pushrocket.common.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Utilities
{
    public class GrpcDataDownloader : IGrpcDataDownloader
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly string _saveDirectory;
        #endregion

        #region Delegates
        public event EventHandler<AcknowledgeDeliveryRequest> AcknowledgeDelivery;
        #endregion

        #region Constructor
        public GrpcDataDownloader(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;

            // Get the save directory from the configuration file.
            _saveDirectory = configuration.GetSection("DownloadPath")?.Value;

            if (_saveDirectory == null)
            {
                _logger.Warning("Save Diretory Not Specified.");
            }
            else
            {
                _logger?.Information($"Initializing GRPC Stream Downloader. (Save Directory: {_saveDirectory})");
            }

            // Create the directory if it doesn't exist.
            if (!Directory.Exists(_saveDirectory))
            {
                _logger.Information("{saveDirectory} could not be found. Creating directory.", _saveDirectory);

                Directory.CreateDirectory(_saveDirectory);
            }
        }
        #endregion

        #region Methods
        public async Task<Unit> OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext)
        {
            var tempFilePath = Path.Combine(_saveDirectory, $"{responseStreamContext.ResponseData.SourceRequestId}.tmp");

            _logger?.Information($"Streaming Pulled Data to {tempFilePath}");

            var isDeliverySuccessful = false;
            var errorMessage = string.Empty;

            try
            {
                if(await DownloadStreamToFileAsync(responseStreamContext, tempFilePath))
                {
                    UpdateFileName(responseStreamContext.ResponseData.Name, tempFilePath);
                }

                isDeliverySuccessful = true;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"File Pull Error: {ex.Message}");

                errorMessage = ex.Message;

                File.Delete(tempFilePath);
            }

            _logger.Information($"Sending Delivery Acknowledgement (Name: {responseStreamContext.ResponseData.Name}, Is Successful: {isDeliverySuccessful})");

            var acknowledgePullDataRequest = new AcknowledgeDeliveryRequest
            {
                RequestId = responseStreamContext.ResponseData.RequestId,
                Name = responseStreamContext.ResponseData.Name,
                DestinationNode = responseStreamContext.ResponseData.DestinationNode,
                IsDeliverySuccessful = isDeliverySuccessful,
                ErrorMessage = errorMessage
            };

            AcknowledgeDelivery?.Invoke(this, acknowledgePullDataRequest);

            return Unit.Default;
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

            return response.BlockNumber == response.TotalBlocks;
        }

        private void UpdateFileName(string fileName, string tempFilePath)
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

            _logger?.Information($"Renamed {tempFilePath} Bytes to {currentDownloadFilePath}.");
        }
        #endregion
    }
}