using asagiv.datapush.common.Models;

namespace asagiv.datapush.common.Interfaces
{
    public interface IDataPushContextViewModel
    {
        #region Properties
        IDataPushContext DataPushContext { get; }
        double PushProgress { get; set; }
        #endregion
    }
}
