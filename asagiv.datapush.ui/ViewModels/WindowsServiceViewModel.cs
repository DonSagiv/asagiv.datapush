﻿using asagiv.datapush.ui.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace asagiv.datapush.ui.ViewModels
{
    public class WindowsServiceViewModel : BindableBase
    {
        #region Fields
        private WinServiceStatus _status;
        #endregion

        #region Properties
        public WindowsServiceSettingsModel ServiceModel { get; }
        public WinServiceStatus Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(nameof(Status)); }
        }
        #endregion

        #region Commands
        public ICommand StartServiceCommand { get; }
        public ICommand StopServiceCommand { get; }
        public ICommand UpdateSettingsCommand { get; }
        #endregion

        #region Constructor
        public WindowsServiceViewModel()
        {
            ServiceModel = new WindowsServiceSettingsModel();

            StartServiceCommand = new DelegateCommand(async () => await StartServiceAsync());
            StopServiceCommand = new DelegateCommand(async () => await StopServiceAsync());
            UpdateSettingsCommand = new DelegateCommand(async () => await ServiceModel.UpdateServiceSettingsAsync());

            // Check the status of the service every second.
            _ = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(async _ => await GetServiceStatusAsync());
        }
        #endregion

        #region methods
        private async Task StopServiceAsync()
        {
            await WindowsServiceSettingsModel.StopClientAsync();

            await GetServiceStatusAsync();
        }

        private async Task StartServiceAsync()
        {
            await WindowsServiceSettingsModel.InitializeClientAsync();

            await GetServiceStatusAsync();
        }

        private async Task GetServiceStatusAsync()
        {
            Status = await WindowsServiceSettingsModel.GetServiceStatus();
        }
        #endregion
    }
}