using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;

namespace Telemetry_Device.Services
{
    public static class PcapProcessor
    {
        public static TransformBlock<string, List<Packet>> CreatePcapReaderBlock()
        {
            return new TransformBlock<string, List<Packet>>(filePath =>
            {
                var packets = new List<Packet>();

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var br = new BinaryReader(fs))
                {
                    br.ReadBytes(24);

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        uint ts_sec = br.ReadUInt32();
                        uint ts_usec = br.ReadUInt32();
                        uint incl_len = br.ReadUInt32();
                        uint orig_len = br.ReadUInt32();

                        byte[] payload = br.ReadBytes((int)incl_len);

                        packets.Add(new Packet
                        {
                            TimestampSeconds = ts_sec,
                            TimestampMicroseconds = ts_usec,
                            IncludedLength = incl_len,
                            OriginalLength = orig_len,
                            Payload = payload
                        });
                    }
                }

                return packets;
            });
        }


    }
}
