using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPiece : MonoBehaviour {

    private int[] position = new int[2]; // position x, y
	private bool isBlack;
    private bool isKinged;
    private PieceMovement movement;      // will handle all piece movement

	public CheckerPiece(int[] pos, bool black)
	{
		position[0] = pos[0];
		position[1] = pos[1];
		this.isBlack = black;
		movement = new PieceMovement();	
	}
	
    #region "Get Methods"
	
	public int[] getPosition() { return position; }
	
	public bool getColor() { return isBlack; }
	
	public bool getIsKing() { return isKinged; }
	
	public PieceMovement getMovement { return movement; }
	    
    #endregion
    
    #region "Set Methods"
	
	//called when piece is clicked on
    //will highlight valid spots on board	
	public ArrayList<BoardCell> showValidMoves(GameBoard board)
	{
		//
		// if (isKinged) { movement.getValidMoves(position, board); }
		// else { movement.getValidKingMoves(position, board); }
	    // return array list of possible board cells
	}
	
	//promote should probably be here I think..?
	//since movement doesnt have a way to access the bool isKinged to change it
	public void promote()
	{
		isKinged = true;
		//code to switch regular piece with king piece
	}
    
    #endregion
}
