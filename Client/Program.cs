using System.Runtime.InteropServices.Marshalling;
using LiteNetLib;
using LiteNetLib.Utils;
EventBasedNetListener listener = new EventBasedNetListener();
NetDataWriter writer = new NetDataWriter();
NetManager client = new NetManager(listener);
//NetPeer? peer = null;
client.Start();
client.Connect("localhost" /* host IP or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);

listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    //peer = fromPeer;

    Console.WriteLine("We got: " + dataReader.GetString(200 /* max length of string */));

    dataReader.Recycle();
    
};




while (true)
{


    client.PollEvents();
    Thread.Sleep(15);

    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Escape)
            System.Console.WriteLine("client has stopped");
            client.Stop();
    }
}

