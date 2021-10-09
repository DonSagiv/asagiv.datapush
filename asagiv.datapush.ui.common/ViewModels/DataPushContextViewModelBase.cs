using asagiv.datapush.common.Interfaces;
using asagiv.datapush.ui.common.Interfaces;
using ReactiveUI;

namespace asagiv.datapush.ui.common.ViewModels
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
        public void OnPushResponseReceived(double progress)
        {
            PushProgress = progress;
        }
        #endregion
    }
}
