using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace asagiv.pushrocket.ui.common.Utilities
{
    public class DarkModeService
    {
        #region Fields
        private readonly Subject<bool> _darkModeSetSubject = new();
        private bool _darkModeEnabled;
        #endregion

        #region Properties
        public bool DarkModeEnabled 
        { 
            get => _darkModeEnabled;
            set { _darkModeEnabled = value; _darkModeSetSubject.OnNext(_darkModeEnabled); }
        }
        public IObservable<bool> DarkModeSetObservable => _darkModeSetSubject.AsObservable();
        #endregion
    }
}
