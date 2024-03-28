namespace ProtocolCore;

public static class Extensions
{
    public static byte[] ReverseIfLittleEndian(this byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }
}