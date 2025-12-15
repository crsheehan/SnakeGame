// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2025 UofU-CS3500. All rights reserved.
// </copyright>


using System.Net.Sockets;

namespace Networking;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{
    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer(Action<NetworkConnection> handleConnect, int port)
    {
        //Create listener
        TcpListener listener = TcpListener.Create(port);

        //Start listener
        listener.Start();

        // Constantly wait for connection
        while (true)
        {
            if (listener.Pending()) //Will be true if client tries to connect
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkConnection connection = new NetworkConnection(client);
                Thread
                    t = new Thread(() =>
                        handleConnect(
                            connection)); //call delegate handleConnect on a new thread with the new client connection to network
                t.Start(); //Start the thread the new clients on
            }
        }
    }
}