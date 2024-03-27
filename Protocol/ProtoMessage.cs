using Protocol.Payloads.Core;


namespace Protocol
{
    public class PayloadInfo
    {
        public Type Type { get; set; }
        public MemoryStream Stream { get; set; }
        
    }
    
    public class ProtoMessage
    {
        public const char HEADER_SEPARATOR = ':';
        public const string HEADER_PAYLOAD_LEN = "len";
        public const string PAYLOAD_SEPARATOR = "--payload";
        
        public string? Action { get; set; }
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
                Type = payload.GetPayloadType(), 
                Stream = payload.GetStream(),
            });
            Headers[HEADER_PAYLOAD_LEN] = PayloadsInfo.Sum(pi => (int)pi.Stream.Length).ToString();
        }

        public void AddPayload(IEnumerable<IPayload> payloads)
        {
            PayloadsInfo.AddRange(payloads.Select(p => new PayloadInfo
            {
                Type = p.GetPayloadType(), 
                Stream = p.GetStream(),
            }));
            Headers[HEADER_PAYLOAD_LEN] = PayloadsInfo.Sum(pi => (int)pi.Stream.Length).ToString();
        }
        
        public void AddPayload(params IPayload[] payloads)
        {
            PayloadsInfo.AddRange(payloads.Select(p => new PayloadInfo
            { 
                Type = p.GetPayloadType(), 
                Stream = p.GetStream()
            }));
            Headers[HEADER_PAYLOAD_LEN] = PayloadsInfo.Sum(pi => (int)pi.Stream.Length).ToString();
        }
        
        #endregion
        
        
        public MemoryStream GetStream()
        {
            MemoryStream memStream = new MemoryStream();
            memStream.Write(new byte[4], 0, 4);

            StreamWriter writer = new StreamWriter(memStream);
            
            // 1. Write Action
            writer.WriteLine(Action);
                
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
            
            // 4. Write the packet length without the first 4 bytes to represent the length itself
            memStream.Position = 0;
            byte[] sizeHeader = ConvertInt((int)memStream.Length - 4);
            memStream.Write(sizeHeader, 0, 4);
            memStream.Position = 0;
            
            return memStream;
        }

        private byte[] ConvertInt(int val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);

            return intBytes;
        }
        
    }
}
