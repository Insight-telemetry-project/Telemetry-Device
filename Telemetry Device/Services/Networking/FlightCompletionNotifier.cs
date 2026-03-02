using Telemetry_Device.Models.Dto;
using Telemetry_Device.Models.Interface.Networking;

namespace Telemetry_Device.Services.Networking
{
    public class FlightCompletionNotifier : IFlightCompletionNotifier
    {
        private readonly HttpClient _httpClient;

        public FlightCompletionNotifier(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task NotifyFlightCompletedAsync(int masterIndex, int expectedFrames)
        {
            FlightFramesUpdateRequest request = new FlightFramesUpdateRequest
            {
                MasterIndex = masterIndex,
                ExpectedFrames = expectedFrames
            };

            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "Kafka/update-expected-frames",
                    request);

            response.EnsureSuccessStatusCode();
        }
    }
}
