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
        private readonly TransformManyBlock<string, PacketData> _transformBlock;

        public BuilderBlock(IUdpChecksumCalculator checksumCalculator)
        {
            _udpChecksumCalculator = checksumCalculator;

            _transformBlock = new TransformManyBlock<string, PacketData>(pcapFilePath =>
            {
                return ReadPackets(pcapFilePath);
            });
        }

        private IEnumerable<PacketData> ReadPackets(string pcapFilePath)
        {
            using FileStream fileStream = new FileStream(pcapFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader binaryReader = new BinaryReader(fileStream);
            binaryReader.ReadBytes(ConstantPackets.GLOBAL_HEADER_SIZE);

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                PacketData packet = ReadSinglePacket(binaryReader);

                if (_udpChecksumCalculator.ComputeUdpChecksum(packet.Payload) == packet.Checksum)
                {
                    yield return packet;
                }
            }
        }

        public ITargetBlock<string> Input => _transformBlock;
        public ISourceBlock<PacketData> Output => _transformBlock;

        public PacketData ReadSinglePacket(BinaryReader reader)
        {
            _ = reader.ReadUInt32();
            _ = reader.ReadUInt32();
            uint includedLength = reader.ReadUInt32();
            _ = reader.ReadUInt32();

            byte[] payload = reader.ReadBytes((int)includedLength);
            byte udpChecksumMsb = payload[ConstantPackets.UDP_CHECKSUM_HIGH_BYTE_INDEX];
            byte udpChecksumLsb = payload[ConstantPackets.UDP_CHECKSUM_LOW_BYTEINDEX];

            ushort checksum = (ushort)(udpChecksumMsb << ConstantBits.BITS_IN_BYTE | udpChecksumLsb);

            return new PacketData
            {
                Payload = payload,
                Checksum = checksum
            };
        }
    }
}
