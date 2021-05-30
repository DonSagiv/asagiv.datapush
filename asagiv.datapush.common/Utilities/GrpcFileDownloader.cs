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
            _logger?.Information($"Starting Retrieval: {responseStream.ResponseData.Name} from {responseStream.ResponseData.SourceNode}.");

            var fileLocation = Path.Combine(SaveDirectory, responseStream.ResponseData.Name);

            try
            {
                using var fs = new FileStream(fileLocation, FileMode.Append);
                {
                    while (await responseStream.ResponseStream.MoveNext())
                    {
                        var byteArray = responseStream.ResponseStream.Current.Payload.ToByteArray();

                        await fs.WriteAsync(byteArray);

                        _logger?.Information($"Data Retrieved: {byteArray.Length} bytes.");
                    }
                }

                _logger?.Information($"Disposing File Stream.");

                await fs.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, ex.Message);

                File.Delete(fileLocation);
            }
        }
        #endregion
    }
}
