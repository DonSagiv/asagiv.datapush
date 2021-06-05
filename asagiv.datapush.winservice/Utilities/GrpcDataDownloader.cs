using asagiv.datapush.common;
using asagiv.datapush.common.Utilities;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.datapush.winservice.Utilities
{
    public class GrpcDataDownloader
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly string _saveDirectory;
        #endregion

        #region Constructor
        public GrpcDataDownloader(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;

            _saveDirectory = configuration.GetSection("DownloadPath")?.Value;

            if(_saveDirectory == null)
            {
                _logger.Warning($"Save Diretory Not Specified.");
            }
            else
            {
                _logger?.Information($"Initializing GRPC Stream Downloader. (Save Directory: {_saveDirectory})");
            }
        }
        #endregion

        public async Task OnDataRetrievedAsync(ResponseStreamContext<DataPullResponse> responseStreamContext)
        {
            var fileLocation = Path.Combine(_saveDirectory, responseStreamContext.ResponseData.Name);

            _logger?.Information($"Streaming Pulled Data to {fileLocation}");

            try
            {
                using var fs = new FileStream(fileLocation, FileMode.Append);
                {
                    while (await responseStreamContext.ResponseStream.MoveNext())
                    {
                        await DownloadStreamBlock(responseStreamContext, fileLocation, fs);
                    }
                }

                await fs.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"File Pull Error: {ex.Message}");

                File.Delete(fileLocation);
            }
        }

        private async Task DownloadStreamBlock(ResponseStreamContext<DataPullResponse> responseStreamContext, string fileLocation, FileStream fs)
        {
            var response = responseStreamContext.ResponseStream.Current;

            var byteArray = response.Payload.ToByteArray();

            _logger?.Information($"Writing {byteArray.Length} Bytes to {fileLocation} (Block {response.BlockNumber} of {response.TotalBlocks})");

            await fs.WriteAsync(byteArray);
        }
    }
}
