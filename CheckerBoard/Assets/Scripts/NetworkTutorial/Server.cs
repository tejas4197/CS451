using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    //Can be any open port, I just chose 7070
    public int port = 7070;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    //the server itself where most of the operation happens
    private TcpListener server;
    private bool serverStarted;

    //Called when server is created
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try //to connect to server
        {
            //Accept connection from any ip addess (anyone can connect)
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            //Start listening for client
            StartListening();
            serverStarted = true;
        }
        catch(Exception e) //Log connection error if one occurs
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    //Continually check status of connected clients
    private void Update()
    {
        if (!serverStarted)
            return;

        foreach(ServerClient c in clients)
        {
            //Is the client still connected?
            if(!IsConnected(c.tcp))
            {
                //close socket and add client to list of disconnected clients
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else //we are connected
            {
                NetworkStream s = c.tcp.GetStream();

                //If there is data, create stream reader to interperet incoming data
                if(s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    Debug.Log(data);

                    //If there is data, parse it
                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        //Run through list of disconnected clients
        for(int i = 0; i < disconnectList.Count - 1; i++)
        {
            //Tell our player somebody has disconnected
            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    //Listen for incoming connections
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    //Assign values to remember who client is
    private void AcceptTcpClient(IAsyncResult ar)//IAsyncResult - represents the status of an asynchronous operation
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();
        
        Broadcast("", sc);
    }

    //Check if client is still connected to server
    private bool IsConnected(TcpClient c)
    {
        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                /*
                 Block the current thread for x amount of time (the first parameter) 
                 and wait for a "message" of type "SelectMode" (the second parameter).
                */
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    /*
                      Reads/receives data from the Socket until the size of the 
                      buffer of its first parameter is filled. It returns the bytes 
                      successfully read.
                    */
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    //Server send
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach(ServerClient sc in cl)
        {
            try
            {
                //Get socket stream
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                
                //Write and flush data from server
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e) //Client unreachable
            {
                Debug.Log("Write error : " + e.Message);
            }
        }
    }

    //Server send (Overload)
    private void Broadcast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        Broadcast(data, sc);
    }


    //Server Read
    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log("Host: " + data);
        string[] aData = data.Split('|');
        string clientName = "client";

        Debug.Log(aData[2]);

        switch (aData[0])
        {
            case "CWHO":
                c.clientName = "host";
                c.isHost = (aData[2] == "0") ? false : true;
                //c.clientName
                Broadcast("SCNN|" + clientName, clients);
                break;
            case "CMOV":
                Broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4] + "|", clients);
                break;

        }
    }
}

//Object unique for each connected user
public class ServerClient
{
    public string clientName;
    public TcpClient tcp; //Socket
    public bool isHost;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
