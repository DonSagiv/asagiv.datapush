using Grpc.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Utilities
{
    public class GrpcFileDownloader
    {
        #region Properties
        public string SaveDirectory { get; set; }
        #endregion

        #region Methods
        public async Task OnClientDataRetrievedAsync(ResponseStreamContext<DataPullResponse> responseStream)
        {
            var fileLocation = Path.Combine(SaveDirectory, responseStream.ResponseData.Name);

            try
            {
                using var fs = new FileStream(fileLocation, FileMode.Append);
                {
                    while (await responseStream.ResponseStream.MoveNext())
                    {
                        var byteArray = responseStream.ResponseStream.Current.Payload.ToByteArray();

                        await fs.WriteAsync(byteArray);
                    }
                }

                await fs.DisposeAsync();
            }
            catch (Exception ex)
            {
                File.Delete(fileLocation);
            }
        }
        #endregion
    }
}
