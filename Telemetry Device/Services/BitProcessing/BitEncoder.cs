using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;

namespace SendRecieveUDP.Service.BitManipulation
{
    public class BitEncoder : IBitEncoder
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIndex = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIndex = ConstantBits.MAX_BIT_INDEX_IN_BYTE - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);

                int bitValue = (int)((value >> (bitCount - ConstantBits.SINGLE_BIT_VALUE - indexInByte)) & ConstantBits.SINGLE_BIT_VALUE);

                if (bitValue == ConstantBits.SINGLE_BIT_VALUE)
                    buffer[byteIndex] |= (byte)(ConstantBits.SINGLE_BIT_VALUE << bitIndex);
                else
                    buffer[byteIndex] &= (byte)~(ConstantBits.SINGLE_BIT_VALUE << bitIndex);
            }
        }

        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {
            ulong value = ConstantBits.NO_OFFSET;
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIndex = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIndex = ConstantBits.MAX_BIT_INDEX_IN_BYTE - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);

                int bit = (buffer[byteIndex] >> bitIndex) & ConstantBits.SINGLE_BIT_VALUE;
                value = (value << ConstantBits.SINGLE_BIT_VALUE) | (ulong)bit; 
            }
            return value;
        }
    }
}
