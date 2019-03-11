using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPiece : MonoBehaviour {

    public bool isBlack;
    private bool isKinged;
    public int[] position = new int[2];  // position x, y        
    private PieceMovement movement;      // will handle all piece movement

    //Basically this works as the constructor
    public void Start()
    {
        isKinged = false;
        movement = gameObject.GetComponent<PieceMovement>();
    }

    public CheckerPiece() { }
	
    #region "Get Methods"
	
	public int[] getPosition() { return position; }
	
    //return true if piece is black, false otherwise
	public bool getColor() { return isBlack; }
	
	public bool getIsKing() { return isKinged; }
	
	public PieceMovement getMovement() { return movement; }
	    
    #endregion
    
    #region "Set Methods"
	
    public void setPosition(int x, int y)
    {
        position[0] = x;
        position[1] = y;
    }

	//called when piece is clicked on
    //will highlight valid spots on board	
	public List<GameObject> showValidMoves(GameBoard board)
	{
        // return array list of possible board cells
        if (isKinged) { return movement.getValidKingMoves(position, board); }
        else { return movement.getValidMoves(position, board); }   
    }

    //make a piece a king
    public void promote()
	{
		isKinged = true;
	}
    
    #endregion
}
