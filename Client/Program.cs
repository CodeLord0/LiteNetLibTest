using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.Marshalling;
using LiteNetLib;
using LiteNetLib.Utils;
EventBasedNetListener listener = new();
NetDataWriter writer = new();
NetManager client = new(listener);
string input = "";
string name;

while (true)
{

    System.Console.WriteLine("Enter a spicy name: ");

    name = Console.ReadLine(); // name that will be shown in the server

    break;
}

client.Start();// starts all clients operations

var serverPeer = client.Connect("figure-liberia.gl.at.ply.gg" /* host IP or name */, 10389 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);

listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    Console.WriteLine(dataReader.GetString(200 /* max length of string */)); // gets the message from the server
    dataReader.Recycle();
};


while (true)
{
    client.PollEvents(); // rerun all events

    if (Console.KeyAvailable) //if any key on the keyboard was pressed
    {
        var key = Console.ReadKey(false).Key; // stores the key that was pressed on the keyborad(non-blocking)

        if (key == ConsoleKey.Enter) // if enter key is pressed
        {

            writer.Put(name + ": " + input); // put the the data into writer
            serverPeer.Send(writer, DeliveryMethod.ReliableSequenced); //send the data to the server

            writer.Reset(); // clear the data in the writer
            input = ""; // resets input to an empty string

        }

        else
        {
            input += (char)key; // add each keyboard character into input
        }

        if (key == ConsoleKey.Backspace) // doest work yet
        {
            input = input[..^1];
            
        }

        if (key == ConsoleKey.Escape) // stops the client and quits the application
        {
            client.Stop();
            break;
        }

    }

    Thread.Sleep(15);

}