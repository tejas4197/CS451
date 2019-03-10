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
    public bool isHost;

    private GameClient player;

    //Prevent component from destroy on load
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

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
        Debug.Log("Client: " + data);
        string[] aData = data.Split('|');
        string clientName = "host";

        switch(aData[0])
        {
            case "":
                UserConnected("client");
                Send("CWHO|" + clientName + "|" + ((isHost)?1:0).ToString());
                break;
            case "SCNN|":
                UserConnected(aData[1]);
                break;


        }
    }

    private void UserConnected(string name)
    {
        GameClient c = new GameClient();
        c.name = name;
        player = c;

        menuManager.Instance.StartGame();
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
