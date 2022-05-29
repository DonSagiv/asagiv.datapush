using Google.Protobuf;
using System;

namespace asagiv.pushrocket.server.common.Models
{
    public class PayloadItem
    {
        #region Properties
        public bool IsConsumed { get; private set; }
        public int BlockNumber { get; }
        public ByteString Payload { get; }
        #endregion

        #region Delegates
        public event EventHandler<string> PayloadConsumed;
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

            PayloadConsumed?.Invoke(this, string.Empty);
        }

        public void SetPayloadPushError(string errorMessage)
        {
            PayloadConsumed?.Invoke(this, errorMessage);
        }
        #endregion
    }
}
