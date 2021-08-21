using asagiv.datapush.ui.mobile.Utilities;
using System.Linq;
using Prism.Mvvm;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        public DataPushViewModel DataPushViewModel { get; }

        public MainPageViewModel()
        {
            var viewModelObject = App.ServiceProvider.GetService(typeof(DataPushViewModel));

            DataPushViewModel = viewModelObject as DataPushViewModel;
        }

        public async Task GetConnectionSettingsAsync()
        {
            try
            {
                var connectionSettings = await XFormsDataPusDbContext.Instance.ConnectionSettingsSet
                    .OrderBy(x => x.ConnectionName)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                var message = e.Message;
            }

            var a = 1;
        }
    }
}
