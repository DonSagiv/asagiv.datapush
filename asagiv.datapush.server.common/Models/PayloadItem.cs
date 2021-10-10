using Google.Protobuf;
using System;

namespace asagiv.datapush.server.common.Models
{
    public class PayloadItem
    {
        #region Properties
        public bool IsConsumed { get; private set; }
        public int BlockNumber { get; }
        public ByteString Payload { get; }
        #endregion

        #region Delegates
        public event EventHandler PayloadConsumed;
        #endregion

        #region Constructor
        public PayloadItem(int blockNumber, ByteString payload)
        {
            BlockNumber = blockNumber;
            Payload = payload;
        }
        #endregion

        #region Methods
        public void SetPayloadConsumed()
        {
            IsConsumed = true;

            PayloadConsumed?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
