using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ProtocolCore.Payloads.Core;

namespace ProtocolCore.Message
{
    public class PayloadInfo
    {
        public required string Type { get; init; }
        public required MemoryStream Stream { get; init; }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    public class ProtoMessage : IMessage 
    {
        public const char HEADER_SEPARATOR = ':';
        public const string HEADER_PAYLOAD_LEN = "len";
        public const string PAYLOAD_SEPARATOR = "--payload";
        
        public string Event { get; set; }
        public MessageType Type { get; set; }
        public int Id { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public List<PayloadInfo> PayloadsInfo { get; private set; } = new List<PayloadInfo>();
        
        public int PayloadLength
        {
            get
            {
                Headers.TryGetValue(HEADER_PAYLOAD_LEN, out string? value);
        
                if (string.IsNullOrEmpty(value))
                    return 0;
        
                return Convert.ToInt32(value);
            }
        }
        
        #region Headers
        
        public void SetHeader(string key, string value)
            => Headers[key] = value;
        

        public void SetHeader(string header)
        {
            string[] chunks = header.Split(HEADER_SEPARATOR, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            if (chunks.Length >= 2)
                SetHeader(chunks[0], chunks[1]);
            
        }
        
        #endregion

        #region Payloads
        
        public void CleanPayloadCollections()
        {
            PayloadsInfo.Clear();
        }
        
        public void AddPayload(IPayload payload)
        {
            PayloadsInfo.Add(new PayloadInfo 
            { 
                Type = payload.GetPayloadType().ToString(), 
                Stream = payload.GetStream(),
            });
            Headers[HEADER_PAYLOAD_LEN] = PayloadsInfo.Sum(pi => (int)pi.Stream.Length).ToString();
        }

        public void AddPayload(IEnumerable<IPayload> payloads)
        {
            PayloadsInfo.AddRange(payloads.Select(p => new PayloadInfo
            {
                Type = p.GetPayloadType().ToString(), 
                Stream = p.GetStream(),
            }));
            Headers[HEADER_PAYLOAD_LEN] = PayloadsInfo.Sum(pi => (int)pi.Stream.Length).ToString();
        }
        
        public void AddPayload(params IPayload[] payloads)
        {
            PayloadsInfo.AddRange(payloads.Select(p => new PayloadInfo
            { 
                Type = p.GetPayloadType().ToString(), 
                Stream = p.GetStream()
            }));
            Headers[HEADER_PAYLOAD_LEN] = PayloadsInfo.Sum(pi => (int)pi.Stream.Length).ToString();
        }
        
        #endregion

        #region IMessage

        public int PayloadCount => PayloadsInfo.Count;

        public MemoryStream GetStream()
        {
            MemoryStream memStream = new MemoryStream();
            memStream.Write(new byte[4], 0, 4);

            using StreamWriter writer = new StreamWriter(memStream, leaveOpen:true);
            
            // 1. Write Id, Type, Event
            writer.WriteLine(Id);
            writer.WriteLine(Type);
            writer.WriteLine(Event);
                
            // 2. Write Headers
            foreach (KeyValuePair<string, string> h in Headers)
                writer.WriteLine($"{h.Key}:{h.Value}");
            writer.WriteLine();

            // 3. Write Payloads
            foreach (var payloadInfo in PayloadsInfo)
            {
                writer.WriteLine($"{PAYLOAD_SEPARATOR}:{payloadInfo.Type}");
                writer.Flush();
                payloadInfo.Stream.Position = 0;
                payloadInfo.Stream.CopyTo(memStream);
                writer.Write('\n');
            }
            writer.Flush();
            
            // 4. Write the packet length without the first 4 bytes to represent the length itself
            memStream.Position = 0;
            byte[] sizeHeader = ConvertInt((int)memStream.Length - 4);
            memStream.Write(sizeHeader, 0, 4);
            memStream.Position = 0;
            
            // using StreamReader reader = new StreamReader(memStream);
            // string s = reader.ReadToEnd();
            //
            return memStream;
        }

        public T GetValue<T>(int index)
            where T : IReversable
        {
            if (index >= PayloadCount)
                throw new Exception("Index "); //TODO: 

            try
            {
                // Get the type of T
                Type currentType = typeof(T);
                
                // Loop through the inheritance hierarchy
                while (currentType != null)
                {
                    // Look for the GetObj method in the current type
                    MethodInfo? method = currentType.GetMethod("GetObj", BindingFlags.Public | BindingFlags.Static);

                    if (method != null)
                    {
                        MethodInfo genericMethod = method.MakeGenericMethod(typeof(T));
                        MemoryStream pStream = PayloadsInfo[index].Stream;
                        return (T)genericMethod.Invoke(null, new object[] { pStream });
                    }

                    // Move to the base type for next iteration
                    currentType = currentType.BaseType;
                }

                // If not found in any ancestor, throw an exception
                throw new Exception($"Type '{typeof(T)}' or its ancestors do not have a public static 'GetObj' method");
            
            }
            catch (Exception ex)
            {
                //TODO:
                throw new Exception();
            }
        }

        public string? GetPayloadType(int index)
        {
            if (index >= PayloadCount)
                throw new Exception("Index "); //TODO: 
            
            return PayloadsInfo[index].Type;
        }
        
        #endregion
        

        private byte[] ConvertInt(int val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);

            return intBytes;
        }
        
    }
}
