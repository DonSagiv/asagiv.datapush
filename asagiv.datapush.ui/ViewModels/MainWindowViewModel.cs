using asagiv.datapush.ui.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private ObservableCollection<string> _destinatioNodes;
        private string _selectedDestinationNode;
        #endregion

        #region Properties
        public DataPushClientModel ClientModel { get; }
        public ObservableCollection<string> DestinationNodes
        {
            get { return _destinatioNodes; }
            set { _destinatioNodes = value; RaisePropertyChanged(nameof(DestinationNodes)); }
        }
        public string SelectedDestinationNode
        {
            get { return _selectedDestinationNode; }
            set { _selectedDestinationNode = value; RaisePropertyChanged(nameof(SelectedDestinationNode)); }
        }
        #endregion

        #region Commands
        public ICommand ConnectToServerCommand { get; }
        public ICommand SelectFileToUploadCommand { get; }
        public ICommand BrowseSaveLocationCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            ClientModel = new DataPushClientModel();

            DestinationNodes = new ObservableCollection<string>();

            ConnectToServerCommand = new DelegateCommand(async () => await ConnectToServerAsync());
        }
        #endregion

        #region Methods
        private Task ConnectToServerAsync() => ClientModel.InitializeClientAsync();
        #endregion
    }
}