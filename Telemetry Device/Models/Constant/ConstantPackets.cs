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
        public const int START_IP_HEADER = 12;
        public const int END_IP_HEADER = 18;
        public const int CHECKSUM_OFFSET = 6;
        public const ushort FIXED_UDP_LENGTH = 57;
        public const int FRAME_IP_HEADER_OFFSET = 4;
        public const int FRAME_UDP_HEADER_OFFSET = 24;
        public const string FLIGHT_ID = "Master Index";
    }
}


