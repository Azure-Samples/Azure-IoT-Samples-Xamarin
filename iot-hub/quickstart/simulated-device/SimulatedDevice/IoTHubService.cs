using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Xamarin.Forms;

namespace SimulatedDevice
{
    public class IoTHubService
    {
        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id myXamarinDevice --output table
        private static readonly string _iotHubConnectionString = "{Your device connection string here}";

        private static DeviceClient _deviceClient;
        private IoTClientStatistics _statistics = new IoTClientStatistics();

        public bool IsInitialized { get; private set; }

        public async Task<bool> Initialize()
        {
            // Catch developers that don't update the connection string at the top of this code file.
            if (string.IsNullOrWhiteSpace(_iotHubConnectionString) || _iotHubConnectionString.StartsWith("{Your"))
                return false;

            _deviceClient = DeviceClient.CreateFromConnectionString(_iotHubConnectionString, TransportType.Mqtt);

            // This operation will take a few seconds
            await _deviceClient.OpenAsync();
            IsInitialized = true;
            return true;
        }

        public async Task<IoTClientStatistics> SendMessage(string messageContents)
        {
            if (_deviceClient == null || IsInitialized == false)
                throw new InvalidAsynchronousStateException("You must call Initialize() before sending a message");

            // IoT Hub message is a specific object that takes a byte array. 
            // You can then do many things...
            Message message = new Message(Encoding.UTF8.GetBytes(messageContents));
            
            // ...For example: add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            // Here we add the current runtime platform (Android, iOS, ...)
            message.Properties.Add("platform", Device.RuntimePlatform);

            try
            {
                // Sending the event. If the event fails to be sent (no network, etc...)
                // an exception will be raised.
                await _deviceClient.SendEventAsync(message);
                
                // Updating statistics
                _statistics.MessagesSent += 1;
                _statistics.LastMessageSent = messageContents;
            }
            catch (Exception e)
            {
                _statistics.LastMessageSent = "Error: " + e;
                _statistics.SendFailures += 1;
            }

            return _statistics;
        }
    }
}
