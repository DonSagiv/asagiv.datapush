using asagiv.pushrocket.common.Interfaces;
using Grpc.Core;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Utilities
{
    public class GrpcDataDownloader : IGrpcDataDownloader
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Properties
        public string SaveDirectory { get; }
        #endregion

        #region Constructor
        public GrpcDataDownloader(ILogger logger, IPlatformServices platformServices)
        {
            _logger = logger;

            // Get the save directory from the configuration file.
            SaveDirectory = platformServices.GetDownloadDirectory();

            if (string.IsNullOrWhiteSpace(SaveDirectory) || !Directory.Exists(SaveDirectory))
            {
                _logger.Warning("Save Diretory Not Specified.");
            }
            else
            {
                _logger?.Information($"Initializing GRPC Stream Downloader. (Save Directory: {SaveDirectory})");
            }

            // Create the directory if it doesn't exist.
            if (!Directory.Exists(SaveDirectory))
            {
                _logger.Information("{saveDirectory} could not be found. Creating directory.", SaveDirectory);

                Directory.CreateDirectory(SaveDirectory);
            }
        }
        #endregion

        #region Methods
        public async Task<AcknowledgeDeliveryRequest> OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext)
        {
            var tempFilePath = Path.Combine(SaveDirectory, $"{responseStreamContext.ResponseData.SourceRequestId}.tmp");

            _logger?.Information($"Streaming Pulled Data to {tempFilePath}");

            var isDeliverySuccessful = false;
            var errorMessage = string.Empty;

            try
            {
                if (await DownloadStreamToFileAsync(responseStreamContext, tempFilePath))
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

            return new AcknowledgeDeliveryRequest
            {
                RequestId = responseStreamContext.ResponseData.RequestId,
                Name = responseStreamContext.ResponseData.Name,
                DestinationNode = responseStreamContext.ResponseData.DestinationNode,
                IsDeliverySuccessful = isDeliverySuccessful,
                ErrorMessage = errorMessage
            };
        }

        private async Task<bool> DownloadStreamToFileAsync(IResponseStreamContext<DataPullResponse> responseStreamContext, string tempFilePath)
        {
            var downloadComplete = false;

            using var fs = new FileStream(tempFilePath, FileMode.Append);

            while (await responseStreamContext.ResponseStream.MoveNext())
            {
                downloadComplete = await DownloadStreamBlockAsync(responseStreamContext, tempFilePath, fs);
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