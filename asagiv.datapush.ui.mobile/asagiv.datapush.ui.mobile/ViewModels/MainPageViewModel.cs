using asagiv.datapush.ui.mobile.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using ReactiveUI;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        public DataPushViewModel DataPushViewModel { get; }

        public MainPageViewModel()
        {
            var viewModelObject = App.ServiceProvider.GetService(typeof(DataPushViewModel));

            DataPushViewModel = viewModelObject as DataPushViewModel;
        }
    }
}
