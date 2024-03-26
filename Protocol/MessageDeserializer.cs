using System.Text;
using System.Text.Json;
using Protocol.Payloads.Core;

namespace Protocol;

public static class MessageDeserializer 
{
    public static T? Deserialize<T>(MemoryStream mStream)
        where T : IPayload
    {
        if (typeof(T).IsSubclassOf(typeof(JsonPayload)))
        {
            return DeserializeJson<T>(mStream);
        }
        
        // Add processing of other types if necessary

        throw new ArgumentException($"Unsupported type: {typeof(T).Name}");
    }

    private static T? DeserializeJson<T>(MemoryStream mStream)
    {
        // Rewind the stream to the beginning
        mStream.Seek(0, SeekOrigin.Begin);

        // Read the bytes from the stream
        byte[] bytes = new byte[mStream.Length];
        mStream.Read(bytes, 0, bytes.Length);

        // Decode the bytes into a string using UTF-8 encoding
        string jsonString = Encoding.UTF8.GetString(bytes);
                    
        return JsonSerializer.Deserialize<T>(jsonString);
    }
}