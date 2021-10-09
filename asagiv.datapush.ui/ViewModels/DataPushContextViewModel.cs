using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace asagiv.datapush.ui.ViewModels
{
    public class DataPushContextViewModel : ReactiveObject, IDataPushContextViewModel
    {
        #region Fields
        private double _pushProgress;
        #endregion

        #region Properties
        public DataPushContext DataPushContext { get; }
        public double PushProgress
        {
            get => _pushProgress;
            set => this.RaiseAndSetIfChanged(ref _pushProgress, value);
        }
        #endregion

        #region Constructor
        public DataPushContextViewModel(DataPushContext dataPushContext)
        {
            DataPushContext = dataPushContext;

            DataPushContext.OnPushResponseReceived
                .ObserveOnDispatcher()
                .Subscribe(OnPushResponseReceived);
        }
        #endregion

        #region Methods
        public void OnPushResponseReceived(double progress)
        {
            PushProgress = progress;
        }
        #endregion
    }
}
