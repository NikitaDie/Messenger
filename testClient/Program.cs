// See https://aka.ms/new-console-template for more information

using EventTransmitter;
using MessengerProtocolRealization.Message;
using MessengerProtocolRealization.Payloads;
using MessengerProtocolRealization.Transport;

const string host = "127.0.0.19";
const int port = 8080;

TcpTransport tcpTransport = new TcpTransport($"{host}:{port}")
{
    MessageBuilder = new MessageBuilder()
};
_ = tcpTransport.InitializeAsync();

EventDrivenSocket drivenSocket = new EventDrivenSocket(tcpTransport);

TextMessage ms = new TextMessage("Hello, World!");
TextMessage ms2 = new TextMessage("How do you do?");
TextMessage ms3 = new TextMessage("Have you done your homework?");

_ = drivenSocket.EmitAsync("test", ms, ms2, ms3);
_ = drivenSocket.EmitAsync("test2", new TextMessage("This message is for TestHandler"));

_ = drivenSocket.EmitAsync("akaTest", response =>
{
    Console.WriteLine(response.GetValue<TextMessage>(0).Content);
});
while (true)
{
    
}