namespace Telemetry_Device.Models.Interface.Networking
{
    public interface IUdpChecksumCalculator
    {
        ushort ComputeUdpChecksum(byte[] frame);
    }
}
