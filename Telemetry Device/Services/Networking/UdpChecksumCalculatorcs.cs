using SendRecieveUDP.Model.Constant;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Networking;

namespace Telemetry_Device.Services.Networking
{
    public class UdpChecksumCalculator : IUdpChecksumCalculator
    {
        public ushort ComputeUdpChecksum(byte[] frame)
        {
            uint checksum = 0;

            for (int offset = ConstantPackets.START_IP_HEADER; offset <= ConstantPackets.END_IP_HEADER; offset += 2)
            {
                AddWord(ref checksum, (ushort)((frame[ConstantPackets.FRAME_IP_HEADER_OFFSET + offset] << ConstantBits.BITS_IN_BYTE)
                                             | frame[ConstantPackets.FRAME_IP_HEADER_OFFSET + offset + 1]));
            }

            AddWord(ref checksum, ConstantNetwork.UDP_PROTOCOL_NUMBER);
            AddWord(ref checksum, ConstantPackets.FIXED_UDP_LENGTH);

            for (int udpWordOffset = 0; udpWordOffset < ConstantPackets.FIXED_UDP_LENGTH; udpWordOffset += 2)
            {
                if (udpWordOffset != ConstantPackets.CHECKSUM_OFFSET)
                {
                    int frameByteIndex = ConstantPackets.FRAME_UDP_HEADER_OFFSET + udpWordOffset;
                    byte highByte = frame[frameByteIndex];
                    byte lowByte = (udpWordOffset + 1 < ConstantPackets.FIXED_UDP_LENGTH) ? frame[frameByteIndex + 1] : (byte)0;

                    AddWord(ref checksum, (ushort)((highByte << ConstantBits.BITS_IN_BYTE) | lowByte));
                }
            }

            return (ushort)~checksum;
        }

        private void AddWord(ref uint sum, ushort word)
        {
            sum += word;
            sum = (sum & 0xFFFF) + (sum >> ConstantBits.WORD);
        }
    }
}
