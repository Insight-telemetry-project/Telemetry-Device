using System;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;

namespace Telemetry_Device.Services
{
    public class Decoder
    {
        private readonly FieldMapping _mapping;

        public Decoder(FieldMapping mapping)
        {
            _mapping = mapping;
        }

        public TransformBlock<Packet, DecodedPacket> CreateDecoderBlock()
        {
            return new TransformBlock<Packet, DecodedPacket>(packet =>
            {
                var decoded = new DecodedPacket
                {
                    Timestamp = packet.TimestampSeconds + packet.TimestampMicroseconds / 1_000_000.0
                };

                const int BaseOffset = 32;

                T ReadValue<T>(int byteLocation) where T : struct
                {
                    int realOffset = BaseOffset + byteLocation;
                    if (realOffset + 4 > packet.Payload.Length)
                        return default;

                    if (typeof(T) == typeof(float))
                        return (T)(object)BitConverter.ToSingle(packet.Payload, realOffset);

                    if (typeof(T) == typeof(int))
                        return (T)(object)BitConverter.ToInt32(packet.Payload, realOffset);

                    throw new NotSupportedException($"Type {typeof(T)} is not supported");
                }

                foreach (var (setter, offset) in _mapping.FloatFields)
                    setter(decoded, ReadValue<float>(offset));

                foreach (var (setter, offset) in _mapping.IntFields)
                    setter(decoded, ReadValue<int>(offset));

                return decoded;
            });
        }
    }
}
