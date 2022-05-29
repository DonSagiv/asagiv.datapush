using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.ui.common.Interfaces;
using ReactiveUI;
using System;

namespace asagiv.pushrocket.ui.common.ViewModels
{
    public abstract class DataPushContextViewModelBase : ReactiveObject, IDataPushContextViewModel
    {
        #region Fields
        private double _pushProgress;
        #endregion

        #region Properties
        public IDataPushContext DataPushContext { get; }
        public double PushProgress
        {
            get => _pushProgress;
            set => this.RaiseAndSetIfChanged(ref _pushProgress, value);
        }
        #endregion

        #region Constructor
        protected DataPushContextViewModelBase(IDataPushContext dataPushContext)
        {
            DataPushContext = dataPushContext;
        }
        #endregion

        #region Methods
        public void OnPushResponseReceived(int blockNumber)
        {
            PushProgress = blockNumber / Convert.ToDouble(DataPushContext.TotalNumberOfBlocks);
        }
        #endregion
    }
}
