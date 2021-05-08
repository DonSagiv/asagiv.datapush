using asagiv.datapush.server.common.Interfaces;
using Google.Protobuf;

namespace asagiv.datapush.server.common.Models
{
    public class PushRepositoryItem : IPushRepositoryItem
    {
        #region Properties
        public string Topic { get; }
        public ByteString Data { get; }
        #endregion

        #region Constructor
        public PushRepositoryItem(string Topic, ByteString Data)
        {
            this.Topic = Topic;
            this.Data = Data;
        }
        #endregion
    }
}
