using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.Marshalling;
using LiteNetLib;
using LiteNetLib.Utils;
EventBasedNetListener listener = new ();
NetDataWriter writer = new ();
NetManager client = new(listener);
string input = "";
Console.Write("Enter a spicy name: ");
string name = Console.ReadLine();
//NetPeer? serverPeer = null;

client.Start();
var serverPeer = client.Connect("figure-liberia.gl.at.ply.gg" /* host IP or name */, 10389 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);

listener.PeerConnectedEvent += peer =>
{
    //serverPeer = peer;
};


listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    Console.WriteLine(dataReader.GetString(200 /* max length of string */));
    dataReader.Recycle();
};



while (true)
{
    client.PollEvents();
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(false).Key; // non-blocking

        if (key == ConsoleKey.Enter)
        {
            writer.Put(name + ": " + input);
            serverPeer.Send(writer, DeliveryMethod.ReliableSequenced);
            writer.Reset();
            System.Console.WriteLine(input);
            input = "";

        }

        else
        {
            input += (char)key;
        }

        if (key == ConsoleKey.Backspace)
        {
            input = input[..^1];
        }

        if (key == ConsoleKey.Escape)
        {
            client.Stop();
        }

    }
    Thread.Sleep(15);

}