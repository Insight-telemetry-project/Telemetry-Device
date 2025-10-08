using SendRecieveUDP.Model.Constant;

namespace Telemetry_Device.Models.Constant
{
    public class ConstantPackets
    {
        public const int GLOBAL_HEADER_SIZE = 24;
        public const int HEADER_BYTE_OFFSET = 32;
        public const int HEADER_BIT_OFFSET = 256;
        public const int UDP_CHECKSUM_HIGH_BYTE_INDEX = 30;
        public const int UDP_CHECKSUM_LOW_BYTEINDEX = 31;
        public const string BASE_DIRECTORY_OF_FILES = "Data";
    }
}


