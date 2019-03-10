using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//Manages the menu and the unity canvas. The menu is divided into 3 parts:
//The main panel
//Connect panel
//Host panel 
public class menuManager : MonoBehaviour
{
    public static menuManager Instance {set; get;}
    public GameObject mainMenu, serverMenu, connectMenu; //Gameobjects that refer to the three menus in the scene
    public GameObject serverPrefab, clientPrefab;

    void Start()
    {
        Instance = this;
        //Disable server and connect menu
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
//disable main menu and activate server menu
    public void ConnectButton()
    {
        mainMenu.SetActive(false); 
        connectMenu.SetActive(true); 
    }

    //Handles hosting the server so instantiates the server prefab, disables the main menu and activates server menu
    public void HostButton()
    {
        try{
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.ConnectToServer("127.0.0.1", 7070);
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }

    //level two of connecting to server. the function finds the host address either by reading the input field or 
    //if it is empty, it defaults to localhost
    public void ConnectToServerBtn(){
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if(hostAddress == "")
            hostAddress = "127.0.0.1";        
        //connect to host with the designated port
        try{
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.ConnectToServer(hostAddress, 7070); 
            connectMenu.SetActive(false);
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
    }
    public void BackBtn(){
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }
}
