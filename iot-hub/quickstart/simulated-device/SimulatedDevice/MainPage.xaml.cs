using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SimulatedDevice
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly IoTHubService _iotHubService = new IoTHubService();
        private CancellationTokenSource _sendTaskCancellationTokenSource = new CancellationTokenSource();
        private DataGenerator _temperatureGenerator = new DataGenerator(0,40);
        private DataGenerator _humidityGenerator = new DataGenerator(10,90);

        public MainPage()
        {
            InitializeComponent();
            this.Appearing += MainPage_Appearing;
        }

        private async void MainPage_Appearing(object sender, EventArgs e)
        {
            this.Appearing -= MainPage_Appearing;
            try
            {
                SetButtonsState(false);
                bool initialized = await _iotHubService.Initialize();
                if (!initialized)
                {
                    await DisplayAlert("Sample not configured", "Unable to initialize IoT Hub Client. Please check IoTHubService.cs file and that you have access to Internet.", "ok");
                }

                SetButtonsState(false);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task TimerCallback()
        {
            string message = GetNextMessage();

            try
            {
                IoTClientStatistics statistics = await _iotHubService.SendMessage(message);
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.BindingContext = null;
                    this.BindingContext = statistics;
                });
            }
            catch (InvalidAsynchronousStateException e)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Exception", e?.Message, "ok"));
            }
        }

        private void OnBtnStartClicked(object sender, EventArgs e)
        {
            _sendTaskCancellationTokenSource = new CancellationTokenSource();

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                if (_sendTaskCancellationTokenSource != null &&
                    _sendTaskCancellationTokenSource.IsCancellationRequested)
                    return false;
                
                Task.Factory.StartNew(async () => await TimerCallback());
                return true;
            });

            SetButtonsState(true);
        }

        private void OnBtnStopClicked(object sender, EventArgs e)
        {
            _sendTaskCancellationTokenSource.Cancel();
            SetButtonsState(false);
        }

        private void SetButtonsState(bool isProcessRunning)
        {
            BtnStop.IsEnabled = isProcessRunning;
            BtnStart.IsEnabled = !isProcessRunning && _iotHubService.IsInitialized;
        }

        private string GetNextMessage()
        {
            DeviceReadingMessage msg = new DeviceReadingMessage()
            {
                Temperature = _temperatureGenerator.GetNextValue().ToString("N"),
                Humidity = _humidityGenerator.GetNextValue().ToString("N")
            };

            return JsonSerializer.Serialize(msg);
        }
    }
}
