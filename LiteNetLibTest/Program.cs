using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Security;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

EventBasedNetListener listener = new (); // listens for evens that happen in the network
NetManager server = new (listener);
//NetPeer? clientPeer = null;
NetDataWriter writer = new();
Dictionary<int, NetPeer?> clientPeers = new(); // stores all the connected clients
string input = "";
string? peerMessage = null;

//acts as the server, central hub;
server.Start(5615 /* port */); // server should listen for a connection on specified port


listener.ConnectionRequestEvent += request => // lambda function that subscirbes listener to request
{
    if (server.ConnectedPeersCount < 10 /* max connections */) // only allows max number of clients
        request.AcceptIfKey("SomeConnectionKey"); // the key the server expects fromthe server
    else
        request.Reject(); // if request is not expected key requect client request

    
        
};


listener.PeerConnectedEvent += peer => // to represent the client
{
    clientPeers.Add(peer.Id,peer);
    Console.WriteLine("We got connection: from a client!");  // Show peer IP             
    writer.Put("Hello Client!, This is from server");// Put some string
    peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
    writer.Reset();
    

};

listener.PeerDisconnectedEvent += (peer, info) =>
{
    clientPeers.Remove(peer.Id);
    Console.WriteLine("We lost connection from a client");  // Show peer IP
    peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
};

listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    //writer.Put($"We got: {dataReader.GetString()}");
    peerMessage = dataReader.GetString();
    //Console.WriteLine($"a peer said: {dataReader.GetString()}");
    //dataReader.Recycle();   

};






while (true)
{
    server.PollEvents();


    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(false).Key; // non-blocking

        //if (key == ConsoleKey.Enter)
        //{

        if (peerMessage != null)
        {
            foreach (var client in clientPeers.Values)
            {

                writer.Put(peerMessage);
                client.Send(writer, DeliveryMethod.ReliableSequenced);
                Console.WriteLine(peerMessage);


                if (key == ConsoleKey.Backspace)
                {              
                    peerMessage = peerMessage[..^1];
                }

            }
            writer.Reset();
    }

        //input = "";
            //Console.WriteLine();
        //}





        else if (key == ConsoleKey.Escape)
        {
            server.Stop();
        }
        else
        {
            input += (char)key;
        }

        Thread.Sleep(15);


    }



































    Thread.Sleep(15);
}



