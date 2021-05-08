﻿using asagiv.datapush.common;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive.Linq;
using System;
using System.Reactive.Concurrency;

namespace asagiv.datapush.ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private string _filePath;
        private readonly GrpcChannel _channel;
        private readonly DataPush.DataPushClient _client;
        private string _reply;
        private IObservable<long> _pullObservable;
        private IDisposable _pullSubscribe;
        #endregion

        #region Properties
        public string Reply
        {
            get { return _reply; }
            set { _reply = value; RaisePropertyChanged(nameof(Reply)); }
        }
        #endregion

        #region Commands
        public ICommand SelectFileToUploadCommand { get; }
        public ICommand UploadFileCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            _channel = GrpcChannel.ForAddress("http://localhost:80");
            _client = new DataPush.DataPushClient(_channel);

            SelectFileToUploadCommand = new DelegateCommand(SelectFileToUpload);
            UploadFileCommand = new DelegateCommand(async () => await UploadFileAsync());

            _pullObservable = Observable.Interval(TimeSpan.FromSeconds(1));

            _pullSubscribe = _pullObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(async x => await pollDataAsync(x));
        }
        #endregion

        #region Methods
        private void SelectFileToUpload()
        {
            _filePath = null;

            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select File to Upload.",
                CheckPathExists = true,
            };

            var result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            _filePath = openFileDialog.FileName;
        }

        private async Task UploadFileAsync()
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                return;
            }

            var data = await File.ReadAllBytesAsync(_filePath);

            var request = new DataPushRequest
            {
                Topic = "Test Topic",
                Data = ByteString.CopyFrom(data)
            };

            await _client.PushDataAsync(request);
        }

        private async Task pollDataAsync(long obj)
        {
            var request = new DataPullRequest
            {
                Topic = Path.GetFileName("Test Topic"),
            };

            var pushReply = await _client.PullDataAsync(request);

            if(pushReply.Data.Length == 0)
            {
                Reply = $"{pushReply.Topic} (Attempt {obj})";
            }
            else
            {
                Reply = $"Data Found for {pushReply.Topic}";
                _pullSubscribe.Dispose();
            }
        }
        #endregion
    }
}
