using System.ComponentModel;
using System.Net.Security;
using LiteNetLib;
using LiteNetLib.Utils;

EventBasedNetListener listener = new (); // listens for evens that happen in the network
NetManager server = new (listener);
NetPeer? clientPeer = null;
NetDataWriter writer = new();
string input = "";
NetPacketReader reader;
 //acts as the server, central hub;
server.Start(9050 /* port */); // server should listen for a connection on specified port
 

listener.ConnectionRequestEvent += request => // lambda function that subscirbes listener to request
{
    if (server.ConnectedPeersCount < 10 /* max connections */) // only allows max number of clients
        request.AcceptIfKey("SomeConnectionKey"); // the key the server expects fromthe server
    else
        request.Reject(); // if request is not expected key requect client request
        
};


listener.PeerConnectedEvent += peer => // to represent the client
{
    clientPeer = peer;
    Console.WriteLine("We got connection: from a client!");  // Show peer IP             
    writer.Put("Hello Client!, This is from server");// Put some string
    writer.Reset();
    System.Console.WriteLine("am running ");
    peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
    

};

listener.PeerDisconnectedEvent += (peer, info) =>
{
    Console.WriteLine("We lost connection from a client");  // Show peer IP
    clientPeer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
};

listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    //writer.Put($"We got: {dataReader.GetString()}");
    //Console.WriteLine($"a peer said: {dataReader.GetString()}");
    dataReader.Recycle();

};






while (true)
{
    server.PollEvents();


    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(false).Key; // non-blocking

        if (key == ConsoleKey.Enter)
        {


            writer.Put(input);
            clientPeer.Send(writer, DeliveryMethod.ReliableSequenced);
            writer.Reset();
            System.Console.WriteLine(input);
            input = "";
  

            //Console.WriteLine();
        }
        else
        {
            input += (char)key;
        }

        if (key == ConsoleKey.Backspace)
        {
            input = input[..^1];
        }


        if (key == ConsoleKey.Escape) {
                        server.Stop();
        }



    }


   
































        Thread.Sleep(15);
}



