using System.Net.Sockets;

namespace ProtocolCore.Message;
public static class ProtoMessageBuilder
{
    public static ProtoMessage Receive(NetworkStream receivedStream)
    {
        ProtoMessage protoMessage  = new ProtoMessage();
        int packetLength = ReadPacketLength(receivedStream);
        
        MemoryStream memStream = new MemoryStream(packetLength);
        memStream.Write(ReadBytesFromNetStream(receivedStream, packetLength), 0, packetLength);
        memStream.Position = 0;
        
        using StreamReader reader = new StreamReader(memStream);
        
        ReadMetadata(protoMessage, reader);
        ReadPayload(protoMessage, reader);

       
        return protoMessage;
    }

    #region Readers

    // potential blocking call!
    private static byte[] ReadBytesFromNetStream(NetworkStream netStream, int count)
    {
        byte[] bytes = new byte[count];
        netStream.ReadExactly(bytes, 0, count);
        return bytes;
    }
    
    private static int ReadPacketLength(NetworkStream stream)
    {
        return BitConverter.ToInt32(ReadBytesFromNetStream(stream, 4).ReverseIfLittleEndian(), 0); //TODO: 4 as const
    }
    
    private static void ReadMetadata(ProtoMessage pm, StreamReader sr)
    {
        sr.BaseStream.Position = 0;

        pm.Id = Convert.ToInt32(sr.ReadLine());
        // getting Message Type
        Enum.TryParse(sr.ReadLine(), out MessageType type);
        pm.Type = type;
        pm.Event = sr.ReadLine();
        
        string? headerLine;
        while(! string.IsNullOrEmpty(headerLine = sr.ReadLine()))
            pm.SetHeader(headerLine);
    }

    private static void ReadPayload(ProtoMessage protoMessage, StreamReader reader)
    {
        List<MemoryStream> payloadStreams = new List<MemoryStream>();
        string? currentPayloadType = null;
        MemoryStream? currentPayloadStream = null;

        while (reader.ReadLine() is { } line)
        {
            // If the current line is the PAYLOAD_SEPARATOR, it means a new payload is starting.
            if (line.Contains(ProtoMessage.PAYLOAD_SEPARATOR))
            {
                // 1. Add the current payload stream and Info
                if (currentPayloadStream != null && currentPayloadType != null)
                {
                    payloadStreams.Add(currentPayloadStream);
                    protoMessage.PayloadsInfo.Add(new PayloadInfo
                    {
                        Type = currentPayloadType, 
                        Stream = currentPayloadStream,
                    });
                }
                
                // 2. Read the next line to get the type of the new payload 
                currentPayloadType = line.Split(ProtoMessage.HEADER_SEPARATOR, 
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1];
                currentPayloadStream = new MemoryStream();
            }
            else
            {
                //If the current line is not a separator, it means it contains payload data.
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(line + "\n");
                currentPayloadStream?.Write(buffer, 0, buffer.Length);
            }
        }
        
        // Add the last payload stream
        if (currentPayloadStream != null && currentPayloadType != null)
        {
            payloadStreams.Add(currentPayloadStream);
            protoMessage.PayloadsInfo.Add(new PayloadInfo
            {
                Type = currentPayloadType, 
                Stream = currentPayloadStream,
            });
        }
        
        // Update the payload length header
        protoMessage.Headers[ProtoMessage.HEADER_PAYLOAD_LEN] = payloadStreams.Sum(ps => (int)ps.Length).ToString();
    }

    #endregion
}