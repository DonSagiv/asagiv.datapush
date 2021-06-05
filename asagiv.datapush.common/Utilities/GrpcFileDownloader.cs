using Grpc.Core;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Utilities
{
    public class GrpcFileDownloader
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Properties
        public string SaveDirectory { get; set; }
        #endregion

        #region Constructor
        public GrpcFileDownloader(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task OnClientDataRetrievedAsync(ResponseStreamContext<DataPullResponse> responseStream)
        {
            var fileLocation = Path.Combine(SaveDirectory, responseStream.ResponseData.Name);

            try
            {
                await DownloadFileAsync(responseStream, fileLocation);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                File.Delete(fileLocation);
            }
        }

        private static async Task<FileStream> DownloadFileAsync(ResponseStreamContext<DataPullResponse> responseStream, string fileLocation)
        {
            var fs = new FileStream(fileLocation, FileMode.Append);
            {
                while (await responseStream.ResponseStream.MoveNext())
                {
                    var byteArray = responseStream.ResponseStream.Current.Payload.ToByteArray();

                    await fs.WriteAsync(byteArray);
                }
            }

            await fs.DisposeAsync();

            return fs;
        }
        #endregion
    }
}
