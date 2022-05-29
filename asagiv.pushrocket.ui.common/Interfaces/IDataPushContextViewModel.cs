using asagiv.pushrocket.common.Interfaces;

namespace asagiv.pushrocket.ui.common.Interfaces
{
    public interface IDataPushContextViewModel
    {
        #region Properties
        IDataPushContext DataPushContext { get; }
        double PushProgress { get; set; }
        #endregion
    }
}
