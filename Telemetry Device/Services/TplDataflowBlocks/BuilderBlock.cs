using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Service.BitManipulation;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class BuilderBlock : IBuilderBlock
    {
        public BufferBlock<PacketData> CreateBuilderBlock(string filePath)
        {
            BufferBlock<PacketData> buffer = new BufferBlock<PacketData>();

            Task.Run(() =>
            {
                using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using BinaryReader binaryReader = new BinaryReader(fileStream);

                binaryReader.ReadBytes(ConstantPackets.GLOBAL_HEADER_SIZE);

                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    PacketData packet = ReadSinglePacket(binaryReader);
                    if (ComputeUdpChecksum(packet.PayloadPacket) == packet.ChecksumPacket)
                    {
                        buffer.Post(packet);
                    }
                }

                buffer.Complete();
            });

            return buffer;
        }

        public PacketData ReadSinglePacket(BinaryReader reader)
        {
            _ = reader.ReadUInt32(); // timestampSeconds
            _ = reader.ReadUInt32(); // timestampMicroseconds
            uint includedLength = reader.ReadUInt32();
            _ = reader.ReadUInt32(); // originalLength

            byte[] payload = reader.ReadBytes((int)includedLength);
            byte udpChecksumMsb = payload[ConstantPackets.UDP_CHECKSUM_HIGH_BYTE_INDEX];
            byte udpChecksumLsb = payload[ConstantPackets.UDP_CHECKSUM_LOW_BYTEINDEX];

            ushort checksum = (ushort)(udpChecksumMsb << ConstantBits.BITS_IN_BYTE | udpChecksumLsb);

            return new PacketData
            {
                PayloadPacket = payload,
                ChecksumPacket = checksum
            };
        }







        public ushort ComputeUdpChecksum(byte[] frame)
        {
            uint checksum = 0;
            for (int offset = ConstantPackets.START_IP_HEADER; offset <= ConstantPackets.END_IP_HEADER; offset += 2)
                AddWord(ref checksum, (ushort)((frame[ConstantPackets.FRAME_IP_HEADER_OFFSET + offset] << ConstantBits.BITS_IN_BYTE)
                                             | frame[ConstantPackets.FRAME_IP_HEADER_OFFSET + offset + 1]));

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

        public void AddWord(ref uint sum, ushort word)
        {
            sum += word;
            sum = (sum & 0xFFFF) + (sum >> ConstantBits.WORD);
        }

    }

}
