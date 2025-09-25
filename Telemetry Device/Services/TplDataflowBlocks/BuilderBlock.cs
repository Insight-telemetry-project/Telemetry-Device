using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Service.BitManipulation;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Networking;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class BuilderBlock : IBuilderBlock
    {
        private readonly IUdpChecksumCalculator _udpChecksumCalculator;
        public BuilderBlock(IUdpChecksumCalculator checksumCalculator)
        {
            _udpChecksumCalculator = checksumCalculator;
        }

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
                    if (_udpChecksumCalculator.ComputeUdpChecksum(packet.PayloadPacket) == packet.ChecksumPacket)
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
    }

}
