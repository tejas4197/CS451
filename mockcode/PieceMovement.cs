using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour {

	public PieceMovement() {}
	
	#region "Get Methods"
	
    public ArrayList<BoardCell> getValidMoves(int[] pos, GameBoard board)
	{
		//use pos and board to look on the board for available moves
		//mad logic
	}
	
	public ArrayList<BoardCell> getValidKingMoves(int pos[], GameBoard board)
	{
		//use pos and board to look on the board for valid king moves
		//mad logic
	}
	
	#endregion
	
	#region "Set Methods"
	
	//will actually move piece
	public void move(ArrayList<BoardCell> validMoves, BoardCell bc)
	{
		//check if chosen BoardCell is valid
		//move piece on board
		//code to visual mv
	}
	
	#endregion
}
