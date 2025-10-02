namespace Telemetry_Device.Models.Interface.Kafka
{
    public interface IKafkaSendMessage
    {
        Task SendMessageAsync(string topic, string message);
    }
}
