// See https://aka.ms/new-console-template for more information

using MessengerServer;

Console.WriteLine("Hello, World!");

Server s = new Server();

await s.StartAsync();