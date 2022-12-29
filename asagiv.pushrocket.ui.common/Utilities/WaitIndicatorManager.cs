using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace asagiv.pushrocket.ui.common.Utilities
{
    public class WaitIndicatorService
    {
        #region Fields
        private readonly Subject<bool> _waitIndicatorSetSubject = new();
        #endregion

        #region Properties
        public bool WaitIndicatorVisible { get; private set; }
        public IObservable<bool> WaitIndicatorSetObservable => _waitIndicatorSetSubject.AsObservable();
        #endregion

        #region Methods
        public void ShowWaitIndicator()
        {
            WaitIndicatorVisible = true;

            _waitIndicatorSetSubject?.OnNext(WaitIndicatorVisible);
        }

        public void HideWaitIndicator()
        {
            WaitIndicatorVisible = false;

            _waitIndicatorSetSubject?.OnNext(WaitIndicatorVisible);
        }
        #endregion
    }
}
