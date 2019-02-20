using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour {
   
    private int[] position = new int[2]; //position x, y
	private bool isOccupied;
    private CheckerPiece piece;   //will be null without piece, or have a piece if occupied 

	//constructors
	// every board cell should recieve a position 
	// however some boardcells will be initialized with pieces: so 
	// isOccupied will be true and p will be a piece
	// otherwise, empty cells will be false and p will be null
	public BoardCell(int[] pos, bool Occupied, CheckerPiece cp) 
	{ 
		position[0] = pos[0];
		position[1] = pos[1];
		isOccupied = Occupied;
		CheckerPiece = cp;
	}
	
	#region "Get Methods"
	
    public int[] getPosition() { return position; }
	
	public bool checkIfOccupied() { return isOccupied; }
	
	public CheckerPiece getPiece() { return CheckerPiece; }
	
	#endregion

	#region "Set Methods"
	
	//this could also have the code to get rid of the visual peice in the game
	public clearSpace() 
	{ 
		this.isOccupied = false; 
		this.CheckerPiece = null; 
		//other code 
	}
	
	//this is probably not needed set we cann set isOccupied in setPiece and setOccupied
	public setOccupied(bool Occupied) { isOccupied = Occupied; }
	
	//this could also have the code to set the visual peice in the game
	public setPiece(CheckerPiece cp) 
	{ 
		this.isOccupied = true; 
		this.CheckerPiece = cp; 
		//other code 
	}
	
	#endregion
}
