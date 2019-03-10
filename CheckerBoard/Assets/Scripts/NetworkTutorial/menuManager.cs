using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    public static menuManager Instance {set; get;}
    public GameObject mainMenu, serverMenu, connectMenu;
    public GameObject serverPrefab, clientPrefab;

    void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

   
    public void ConnectButton()
    {
        mainMenu.SetActive(false); 
        connectMenu.SetActive(true); 
    }
    public void HostButton()
    {
        try{
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }

    public void ConnectToServerBtn(){
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if(hostAddress == "")
            hostAddress = "127.0.0.1";

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

    }
}
