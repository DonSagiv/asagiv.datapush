using asagiv.datapush.ui.ViewModels;
using System.Windows;

namespace asagiv.datapush.ui.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}
