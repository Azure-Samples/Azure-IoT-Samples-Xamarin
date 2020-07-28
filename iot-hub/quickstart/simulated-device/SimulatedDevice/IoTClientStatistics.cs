namespace SimulatedDevice
{
    /// <summary>
    /// This class represents the statistics of the IoT Hub Service.
    /// It's updated within IoTHubService class and used in MainPage.
    /// </summary>
    public class IoTClientStatistics
    {
        public long MessagesSent { get; set; } = 0;
        public long ReceiptsConfirmed { get; set; } = 0;
        public long SendFailures { get; set; } = 0;
        public long MessagesReceived { get; set; } = 0;
        public string LastMessageSent { get; set; }
        public string LastMessageReceived { get; set; }
    }
}
