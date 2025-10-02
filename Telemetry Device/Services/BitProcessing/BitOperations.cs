using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
namespace SendRecieveUDP.Service.BitManipulation
{
    public class BitOperations : IBitOperations
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIndex = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIndex = ConstantBits.BITS_IN_BYTE - 1 - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);
                int bitValue = (int)((value >> (bitCount - ConstantBits.STARTING_INDEX - indexInByte)) & ConstantBits.STARTING_INDEX);
                if (bitValue == ConstantBits.STARTING_INDEX)
                    buffer[byteIndex] |= (byte)(ConstantBits.STARTING_INDEX << bitIndex);
                else
                    buffer[byteIndex] &= (byte)~(ConstantBits.STARTING_INDEX << bitIndex);
            }
        }
        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {
            ulong value = ConstantBits.NO_OFFSET;
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIndex = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIndex = ConstantBits.BITS_IN_BYTE - 1 - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);
                int bit = (buffer[byteIndex] >> bitIndex) & ConstantBits.STARTING_INDEX;
                value = (value << ConstantBits.STARTING_INDEX) | (ulong)bit; 
            }
            return value;
        }
    }
}
