using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LightweightChat
{
    public class ChatMessage
    {
        public string? Text { get; set; }
    }

    class Program : INetEventListener
    {
        private NetManager _client;
        private NetPacketProcessor _packetProcessor;
        private NetPeer? _serverPeer;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            _packetProcessor = new NetPacketProcessor();
            _client = new NetManager(this);
            _client.Start();

            Console.WriteLine("Connecting to server...");
            _client.Connect("127.0.0.1", 9050, "chat_key");

            _packetProcessor.SubscribeReusable<ChatMessage>(OnMessageReceived);

            while (true)
            {
                _client.PollEvents();

                if (_serverPeer != null && Console.KeyAvailable)
                {
                    string? text = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var msg = new ChatMessage { Text = text };

                        //manual serilization for comptibility
                        var writer = new NetDataWriter();
                        _packetProcessor.Write(writer, msg);
                        _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
                    }
                }

                Thread.Sleep(15);
            }
        }

        // --- LiteNetLib event handlers ---

        public void OnPeerConnected(NetPeer peer)
        {
            _serverPeer = peer;
            Console.WriteLine("Connected to server!");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("Disconnected from server");
            _serverPeer = null;
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            try
            {
                _packetProcessor.ReadAllPackets(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading packet: {ex.Message}");
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine($"Network error: {socketError}");
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

       //Just for because this version requires this.....
        public void OnConnectionRequest(ConnectionRequest request)
        {
            // Clients don’t accept incoming connections but I'm sure you know that
            request.Reject();
        }

        // --- Chat message handler ---
        private void OnMessageReceived(ChatMessage message)
        {
            Console.WriteLine($"{message.Text}");
        }
    }
}
