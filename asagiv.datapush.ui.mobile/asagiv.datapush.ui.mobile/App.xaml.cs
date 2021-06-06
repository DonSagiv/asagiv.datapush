using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile
{
    public partial class App : Application
    {
        #region Fields
        private readonly ServiceProvider _serviceProvider;
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            MainPage = _serviceProvider.GetService<MainPage>();
        }
        #endregion

        #region Methods
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton(LoggerFactory.CreateLoggerXamarin);
            services.AddSingleton<MainPage>();
            services.AddSingleton<MainPageViewModel>();
        }

        protected override void OnStart()
        {
            // Do nothing for now.
        }

        protected override void OnSleep()
        {
            // Do nothing for now.
        }

        protected override void OnResume()
        {
            // Do nothing for now.
        }
        #endregion
    }
}
