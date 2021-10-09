using asagiv.datapush.common.Interfaces;

namespace asagiv.datapush.ui.common.Interfaces
{
    public interface IDataPushContextViewModel
    {
        #region Properties
        IDataPushContext DataPushContext { get; }
        double PushProgress { get; set; }
        #endregion
    }
}
