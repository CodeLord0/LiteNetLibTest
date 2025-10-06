using LiteNetLib;
using LiteNetLib.Utils;
EventBasedNetListener listener = new EventBasedNetListener();
NetManager client = new NetManager(listener);
client.Start();
client.Connect("localhost" /* host IP or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);

listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
    NetDataWriter writer = new NetDataWriter();


    writer.Put($"We got : {dataReader.GetString(100 /*max length of string */)}");

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