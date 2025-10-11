using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Security;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Collections.Concurrent;
EventBasedNetListener listener = new (); // listens for evens that happen in the network
NetManager server = new (listener);
NetDataWriter writer = new();
Dictionary<int, NetPeer> clientPeers = new(); // stores all the connected clients
                                               // server display name
string? peerMessage = null;
bool nameShown = true;
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
    System.Console.WriteLine(peer);
    nameShown = true;
    clientPeers.Add(peer.Id, peer);
    //clientPeers.Add(peer.Id,peer);            
    writer.Put("Hello Client!, This is from server" );// Put some string    
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
    peerMessage = dataReader.GetString();
    //Console.WriteLine(peerMessage + " "+ "joined the server");
    dataReader.Recycle();
};

    // if (nameShown)
    // {
    //     foreach (var client in clientPeers.Values)
    //     {
    //         writer.Put(peerMessage + " " + "joined the server");
    //         client.Send(writer, DeliveryMethod.ReliableSequenced);
    //         Console.WriteLine(peerMessage);

    //         // if (key == ConsoleKey.Backspace)
    //         // {              
    //         // peerMessage = peerMessage[..^1];
    //         // }

    //     }
    //     //dataReader.Recycle();
    //     writer.Reset();
    //     nameShown = false;
    //     peerMessage = null;

 
    //     //peerMessage = dataReader.get;
    // }


// };






while (true)
{
    server.PollEvents();



    if (peerMessage != null)
    {
        foreach (var client in clientPeers.Values)
        {

            writer.Put(peerMessage);
            client.Send(writer, DeliveryMethod.ReliableSequenced);
            Console.WriteLine(peerMessage);

            //if (key == ConsoleKey.Backspace)
            //{              
                //peerMessage = peerMessage[..^1];
            //}

        }
        peerMessage = null;
        writer.Reset();
        

        // else if (key == ConsoleKey.Escape)
        // {
        //     server.Stop();
        // }
        // else
        // {
        //     input += (char)key;
        // }

        Thread.Sleep(15);


    }



































    Thread.Sleep(15);
}



