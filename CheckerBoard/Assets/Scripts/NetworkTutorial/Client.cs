using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    //Decide when to connect to a certain host
    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try//to create socket 
        {
            //setup socket
            socket = new TcpClient(host, port);
            stream = socket.GetStream();

            //Create new reader/writer on that stream
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch(Exception e) //Report error if unsuccessful
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }

    //Check every frame if message received 
    private void Update()
    {
        if(socketReady)
        {
            //If there exists incoming message in stream
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                //If there is data to process
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }

    //Sending messages to the server
    public void Send(string data)
    {
        if (!socketReady)
            return;

        //Write and flush data to stream
        writer.WriteLine(data);
        writer.Flush();
    }
    
    //read messages from the server
    private void OnIncomingData(string data)
    {
        Debug.Log(data);
    }

    //Handle case of crash while connected
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    //Close socket when program closes
    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    //Close socket when program is disabled
    private void OnDisable()
    {
        CloseSocket();
    }
}

public class GameClient
{
    public string name;
    public bool isHost;

}
