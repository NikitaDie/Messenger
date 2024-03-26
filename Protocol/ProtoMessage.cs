using Protocol.Payloads.Core;

namespace Protocol
{
    public class ProtoMessage
    {
        private const char HEADER_SEPARATOR = ':';
        private const string HEADER_PAYLOAD_COUNT = "cnt";
        private const string HEADER_PAYLOAD_LEN = "len";
        private const string HEADER_PAYLOAD_TYPE = "ptype";
        public string? Action { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public IPayload? Payload { get; set; }
        public MemoryStream? PayloadStream { get; internal set; }

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

        public string? PayloadType
        {
            get
            {
                Headers.TryGetValue(HEADER_PAYLOAD_TYPE, out string? value);

                return value;
            }
        }

        public void SetHeader(string key, string value)
        {
            Headers[key] = value;
        }

        public void SetHeader(string header)
        {
            string[] chunks = header.Split(HEADER_SEPARATOR, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            SetHeader(chunks[0], chunks[1]);
        }

        public void SetPayload(IPayload payload)
        {
            Payload = payload;

            PayloadStream = payload.GetStream();

            Headers[HEADER_PAYLOAD_LEN] = PayloadStream.Length.ToString();
            //Headers[HEADER_PAYLOAD_TYPE] = Payload.GetPayloadType();
        }

        public MemoryStream GetStream()
        {
            MemoryStream memStream = new MemoryStream();
            memStream.Write(new byte[4], 0, 4);

            StreamWriter writer = new StreamWriter(memStream);

            writer.WriteLine(Action);
            foreach (KeyValuePair<string, string> h in Headers)
                writer.WriteLine($"{h.Key}:{h.Value}");
            writer.WriteLine();
            writer.Flush();

            if (Payload is not null && PayloadStream is not null)
            {
                PayloadStream.Position = 0;
                PayloadStream.CopyTo(memStream);
            }

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
