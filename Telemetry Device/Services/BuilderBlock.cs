using Telemetry_Device.Models.Interface;

using System.IO;
using System.Collections.Generic;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Services
{
    public class BuilderBlock : IPcapProcessor
    {
        public IEnumerable<PacketData> ReadPacketsFromFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                binaryReader.ReadBytes(ConstantPackets.GLOBAL_HEADER_SIZE);

                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    yield return ReadSinglePacket(binaryReader);
                }
            }
        }

        private PacketData ReadSinglePacket(BinaryReader reader)
        {

            reader.ReadUInt32(); // timestampSeconds
            reader.ReadUInt32(); // timestampMicroseconds
            uint includedLength = reader.ReadUInt32();
            reader.ReadUInt32(); // originalLength


            byte[] payload = reader.ReadBytes((int)includedLength);


            ushort checksum = 0;
            if (payload.Length >= 36) 
            {
                byte udpChecksumMsb = payload[ConstantPackets.UDP_CHECKSUM_HIGH_BYTE_INDEX];
                byte udpChecksumLsb = payload[ConstantPackets.UDP_CHECKSUM_LOW_BYTEINDEX];

                checksum = (ushort)((udpChecksumMsb << 8) | udpChecksumLsb); 
            }
            Console.WriteLine($"Checksum extracted: {checksum:X4}");

            return new PacketData
            {
                PayloadPacket = payload,
                ChecksumPacket = checksum
            };
        }
    }
}



