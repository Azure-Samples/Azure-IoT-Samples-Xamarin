using System.Text.Json.Serialization;

namespace SimulatedDevice
{
    /// <summary>
    /// This class represents the message serialized and sent to IoT Hub
    /// </summary>
    class DeviceReadingMessage
    {
        [JsonPropertyName("t")]
        public string Temperature { get; set; }
        [JsonPropertyName("h")]
        public string Humidity { get; set; }
    }
}
