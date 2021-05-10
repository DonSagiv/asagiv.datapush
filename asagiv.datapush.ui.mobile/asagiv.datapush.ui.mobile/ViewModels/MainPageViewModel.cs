using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        #region Commands
        public ICommand SelectFileCommand { get; set; }
        #endregion

        #region Constructor
        public MainPageViewModel()
        {
            SelectFileCommand = new DelegateCommand(async() => await SelectFileAsync());
        }
        #endregion

        #region Methods
        private async Task SelectFileAsync()
        {
            var file = await FilePicker.PickAsync();

            var fileName = file.FullPath;
        }
        #endregion
    }
}
