using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Security;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
EventBasedNetListener listener = new (); // listens for evens that happen in the network

NetManager server = new (listener); // central hub of the server

NetDataWriter writer = new(); // method where packets (data) are put into to be sent

Dictionary<int, NetPeer> clientPeers = new(); // stores all the connected clients

string? peerMessage = null; // message server will send to client

server.Start(5615 /* port */); // server should listen for a connection on specified port


listener.ConnectionRequestEvent += request => // lambda function that subscirbes listener to request
{
    if (server.ConnectedPeersCount < 10 /* max connections */) // only allows max number of clients

        request.AcceptIfKey("SomeConnectionKey"); // the key the server expects fromthe server
    else

        request.Reject(); // if request is not expected key requect client request        
};


listener.PeerConnectedEvent += peer => // represent the last connected  client
{
    System.Console.WriteLine(peer); // displays the ip of the connected peer, mainly for debugging

    clientPeers.Add(peer.Id, peer); // add the connected client to the dictionary
          
    writer.Put("Hello Client!, This is from server" );// stores the data to be sent in the writer class

    peer.Send(writer, DeliveryMethod.ReliableOrdered); // send the message to the server

    writer.Reset(); // delete the packet the writer is holding
};


listener.PeerDisconnectedEvent += (peer, info) => // when a peer leaves the server
{
    clientPeers.Remove(peer.Id); // removes peer from the client dictionary

    Console.WriteLine("We lost connection from a client");

    peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
};


listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) => // when the server receives a packet
{
    peerMessage = dataReader.GetString(); // gets the packet sent by the client and stores it into the peerMessage

    dataReader.Recycle(); // equivalent of a garbage collector
};


while (true)
{
    server.PollEvents(); //re-run all events

    if (peerMessage != null) // if peerMessage has a data in it
    {

        foreach (var client in clientPeers.Values) // represent each individual client in the dictionary
        {

            writer.Put(peerMessage); // places data in writer ready to be disributed

            client.Send(writer, DeliveryMethod.ReliableSequenced); // send the data to all the clients in the dictionary

            Console.WriteLine(peerMessage); // writes the message that was sent(mainly for debugging)

        }

        peerMessage = null; // reset peer message

        writer.Reset(); // clear the writer method

        Thread.Sleep(15); // code should sleep to prevent excessive cpu usage


    }


}
