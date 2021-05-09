using asagiv.datapush.server.common.Interfaces;
using Google.Protobuf;
using System;

namespace asagiv.datapush.server.common.Models
{
    public class PushRepositoryItem : IPushRepositoryItem
    {
        #region Properties
        public string Topic { get; }
        public DateTime PushDateTime { get; }
        public ByteString Data { get; }
        #endregion

        #region Constructor
        public PushRepositoryItem(string Topic, ByteString Data)
        {
            this.Topic = Topic;
            this.PushDateTime = DateTime.Now;
            this.Data = Data;
        }
        #endregion
    }
}
