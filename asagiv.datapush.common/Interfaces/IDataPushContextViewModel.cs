using asagiv.datapush.common.Models;

namespace asagiv.datapush.common.Interfaces
{
    public interface IDataPushContextViewModel
    {
        #region Properties
        DataPushContext DataPushContext { get; }
        double PushProgress { get; set; }
        #endregion
    }
}
