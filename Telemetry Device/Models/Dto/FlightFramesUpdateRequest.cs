namespace Telemetry_Device.Models.Dto
{
    public class FlightFramesUpdateRequest
    {
        public int MasterIndex { get; set; }
        public int ExpectedFrames { get; set; }
    }
}
