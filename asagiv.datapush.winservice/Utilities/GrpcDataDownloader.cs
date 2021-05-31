using asagiv.datapush.common;
using asagiv.datapush.common.Utilities;
using Grpc.Core;
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
        private const string _saveDirectory = @"C:\Users\DonSa\Desktop\Rob Photos";
        #endregion

        #region Constructor
        public GrpcDataDownloader(ILogger logger)
        {
            _logger = logger;

            _logger?.Information("Initializing GRPC Stream Downloader.");
        }
        #endregion

        public async Task OnDataRetrievedAsync(ResponseStreamContext<DataPullResponse> e)
        {
            var fileLocation = Path.Combine(_saveDirectory, e.ResponseData.Name);

            _logger?.Information($"Streaming Pulled Data to {fileLocation}");

            try
            {
                using var fs = new FileStream(fileLocation, FileMode.Append);
                {
                    while (await e.ResponseStream.MoveNext())
                    {
                        var byteArray = e.ResponseStream.Current.Payload.ToByteArray();

                        _logger?.Information($"Writing {byteArray.Length} Bytes to {fileLocation}");

                        await fs.WriteAsync(byteArray);
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
    }
}
