namespace Telemetry_Device.Models.Interface.Networking
{
    public interface IFlightCompletionNotifier
    {
        Task NotifyFlightCompletedAsync(int masterIndex, int expectedFrames);
    }
}
