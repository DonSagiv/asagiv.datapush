namespace asagiv.pushrocket.ui.Utilities
{
    public sealed class MauiAppServices
    {
        #region Statics
        private static readonly Lazy<MauiAppServices> _lazyInstance = new(() => new MauiAppServices());
        public static MauiAppServices Instance => _lazyInstance.Value;
        #endregion

        #region Properties
        public IServiceProvider ServiceProvider { get; private set; }
        #endregion

        #region Constructor
        private MauiAppServices() { }
        #endregion

        #region Methods
        public void InjectServiceProvider(IServiceProvider serviceProviderInput)
        {
            this.ServiceProvider = serviceProviderInput;
        }

        public TService GetService<TService>() => ServiceProvider.GetService<TService>();
        #endregion
    }
}
