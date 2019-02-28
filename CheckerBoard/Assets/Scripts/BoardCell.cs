using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour {
   
    public int[] position = new int[2];  //position x, y
    public Color orgColor = new Color(); //original non yellow color of board cell
	private bool isOccupied;
    private CheckerPiece piece;   //will be null without piece, or have a piece if occupied 

	// constructor
	// every board cell should recieve a position 
	// Some boardcells will have non null pieces; isOccupied will be true and p will be a piece
	// otherwise, empty cells will be false and p will be null
	public void BoardCellConstruct(int[] pos, bool Occupied, CheckerPiece cp) 
	{ 
		position[0] = pos[0];
		position[1] = pos[1];
		isOccupied = Occupied;
		CheckerPiece piece = cp;
	}
	
	#region "Get Methods"
	
    public int[] getPosition() { return position; }
	
	public bool checkIfOccupied() { return isOccupied; }
	
	public CheckerPiece getPiece() { return piece; }
	
	#endregion

	#region "Set Methods"
	
	//code to make the space unoccupied
	public void clearSpace() 
	{ 
		this.isOccupied = false; 
		this.piece = null; 
	}
	
	//code to set the peice on board cell
	public void setPiece(CheckerPiece cp) 
	{ 
		this.isOccupied = true; 
		this.piece = cp; 
	}
	
	#endregion
}
