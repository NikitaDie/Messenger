using System.Net.Sockets;

namespace Protocol;

public class ProtoMessageBuilder
{
    private NetworkStream _netStrteam;
    private MemoryStream _memStream = null!;

    public ProtoMessageBuilder(NetworkStream netStrteam)
    {
        this._netStrteam = netStrteam;
    }

    public ProtoMessage Receive()
    {
        int readingSize = ConvertToInt(ReadBytes(4));

        _memStream = new MemoryStream(readingSize);
        _memStream.Write(ReadBytes(readingSize), 0, readingSize);
        _memStream.Position = 0;

        ProtoMessage pm  = new ProtoMessage();
        
        using StreamReader sr = new StreamReader(_memStream);

        ExtractMetadata(pm, sr);
        ExtrtactpayloadStream(pm);

        _memStream.Dispose();

        return pm;
    }

    private void ExtractMetadata(ProtoMessage pm, StreamReader sr)
    {
        sr.BaseStream.Position = 0;

        pm.Action = sr.ReadLine();

        string headerLine;
        while(! string.IsNullOrEmpty(headerLine = sr.ReadLine()))
            pm.SetHeader(headerLine);
    }

    private void ExtrtactpayloadStream(ProtoMessage pm)
    {
        int payloadLength = pm.PayloadLength;

        _memStream.Seek(-payloadLength, SeekOrigin.End);

        MemoryStream payloadStream = new MemoryStream(payloadLength);
        _memStream.CopyTo(payloadStream);
        payloadStream.Position = 0;

        pm.PayloadStream = payloadStream;
    }

    private byte[] ReadBytes(int count)
    {
        
        byte[] bytes = new byte[count];
        _netStrteam.ReadExactly(bytes, 0, count);

        return bytes;
    }

    private int ConvertToInt(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToInt32(bytes, 0);
    }
}
