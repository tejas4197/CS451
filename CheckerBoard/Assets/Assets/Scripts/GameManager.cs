using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    gameBoard board;
    void Start(){
        board  = gameObject.GetComponent<gameBoard>();
        board.CreateBoard();
    }
}
