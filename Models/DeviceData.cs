using System.Text.Json.Serialization;

namespace BerkutLampService.Models;

public class DeviceData
{
    [JsonPropertyName("dataId")]
    public string DataId { get; set; }

    [JsonPropertyName("devId")]
    public string DevId { get; set; }

    [JsonPropertyName("productKey")]
    public string ProductKey { get; set; }

    [JsonPropertyName("status")]
    public List<DeviceStatus> Status { get; set; }
}
