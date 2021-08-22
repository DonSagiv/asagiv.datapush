using asagiv.datapush.common.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class ConnectionSettingsViewModel
    {
        private ObservableCollection<IClientConnectionSettings> _prop;

        public ObservableCollection<IClientConnectionSettings> prop
        {
            get { return _prop; }
            set { _prop = value; }
        }
    }
}
