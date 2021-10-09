using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.ui.common.ViewModels;
using System;
using System.Reactive.Linq;

namespace asagiv.datapush.ui.ViewModels
{
    public class DataPushContextViewModel : DataPushContextViewModelBase
    {
        #region Constructor
        public DataPushContextViewModel(IDataPushContext dataPushContext) : base(dataPushContext)
        {
            DataPushContext.OnPushResponseReceived
                .ObserveOnDispatcher()
                .Subscribe(OnPushResponseReceived);
        }
        #endregion
    }
}
