using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public GameObject redTilePrefab;
    public GameObject blackTilePrefab;
    
    public BoardCell[ , ] bCells = new BoardCell[8, 8];

    public GameBoard() { }
	
	public BoardCell getBoardCell(int bc)
	{
		return bCells[bc];
	}
	
	public void createBoard()
	{
		//initialize board
		//by using a loop to create the board cells
		// int bc = 0;
		// for (int y=0; y<8; y++)
		// {  
		//		for (int x=0; x<8; x++)
		//		{
		//          int[2] pos = {x, y}    
		//			bCells[bc] = new BoardCell(pos, false, null);         
		//      }
	    // }
		//
		// 0,1 1,1 2,1
		// 0,0 1,0 2,0
		//
		// 8 9 10
		// 0 1 2 ...
	}
	
	public void placeInitialPieces()
	{
		//logic to put all pieces in their initial spots too	
		// int c = 0;
		//
		// for (int i=0; i < 24; i++)
		// {       
	    //      CheckerPiece cp = new CheckerPiece(bCells[c].getPostion(), bool color);
		//		bCells[c].setPeice(cp); 
		//      
		//      some math logic to put pieces in correct spaces, shouldnt be too hard
		//      c += 2;
	    //
	    // }
	}
}
