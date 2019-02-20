using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
     
    public bool isConnected;           
    public bool isBlacksTurn;
	public GameBoard board;
	public ServerPlayer player; //wait what is this object?

	#region "Code that Runs Game"
	
	public bool checkWinState(isBlacksTurn) 
	{
		//checks if the person who just went has won
		//by cycling through boardCells
		//check if occupied, check if checkerpiece is oppponent
		//check if opponents peice has valid moves.
		//
		//also use some made logic to check for check mates
		//basically if they have valid moves
		//check if one of those moves can result in a non check mate
		//if one valid non check mate move is found, break loop
		//
		//if none of the opponents peices have valid non check mate moves
		//the player wins, return true
		//
		//return bool 
	} 
	
	public void Start()
	{
		// heres a run of the game
		// the player hits play game, or start whatever
		// code to initialize the game:
		// also the host is black, opponent is red
		// this.initializeOnClient();
		// this.loadClientScene();
		// this.initializeServer();
		// board = new GameBoard();
		// board.createBoard();
		// board.placeInitialPieces();
		// while loop on !checkWinState
		// so if winstate returns true (meaning a player won, then break the loop)
		// while (!this.checkWinState(isBlacksTurn))
		// {
		// 		this.takeTurn(isBlacksTurn)
		//      set isBlacksTurn to the opposite of what it currently is
		// } 
		// isBlacksturn can be used to determine who won
		// this.gameOver(isBlacksTurn);
	}
	
	public void takeTurn(bool isBlacksTurn) 
	{
		//if checks whose turn it is
		//player code
		//if player quits the game call quitGame
		//player selects wrong space, nothing happens
		//player selects their own piece
		//arraylist valid moves = board.getBoardCell([selected space]).getPiece().showValidMoves(this.board);
		//use arraylist to highlight valid spaces
		//player clicks on valid space
		//board.getBoardCell([selected space]).getPiece().getMovement().move(arraylist of valid moves, board.getBoardCell([selected space]));
		//check if opponent piece was jumped		
		//by checking the new position (difference on x and y should by 2)
        //board.getBoardCell([selected space]).clearSpace();		
		//check if move can king piece
		//if it can then
		//board.getBoardCell([selected space]).getPiece().promote();
	}
	
	public void gameOver(bool isBlacksTurn)
	{
		//check who won
		//display who won and lost
		//play again, quit?
	}
	
	//reason for quit game: player will not necessarily quit game after they win, since they can choose to play agian
	public void quitGame()
	{
		//code to quit game and announce to oppponent the surrender
		//this.gameOver(bool isBlacksTurn)
	}
	
	#endregion
	
    #region "Initialization Methods"
	
	public void initializeOnClient() {}
	
	public void initializeOnServer() {}
	
	private IEnumerator loadClientScene() {}

    #endregion
}
