using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameBoard : MonoBehaviour
{
    public GameObject redTilePrefab; 
    public GameObject blackTilePrefab;
    //public Material redMat, blackMat;
    
    public GameObject[ , ] squares  = new GameObject [8,8];

    //the CreateBoard method uses the nested loop to generate x,y values for the board coordinates
    public void CreateBoard(){
        for(int i=0; i<8; i++){
            for (int j=0; j<8; j++){
                //A red tile only occurs when (i, j) blocks are both either even or odd 
                if (i % 2 != 0 && j % 2 != 0 || i % 2 == 0 && j % 2 == 0) {
                    squares [i, j] = Instantiate (redTilePrefab, new Vector2 (i, j), Quaternion.identity);
                }
                //If the coordinates are either both odd or even then a black tile is instantiated
                else{
                    squares [i, j] = Instantiate (blackTilePrefab, new Vector2 (i, j), Quaternion.identity);
                }   
            }
        }
    }
}
