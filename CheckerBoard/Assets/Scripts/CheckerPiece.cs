using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPiece : MonoBehaviour {

    public int[] position = new int[2];  // position x, y    
    public bool isBlack;
    private bool isKinged;
    private PieceMovement movement;      // will handle all piece movement

    //Basically this works as the constructor
    public void Start()
    {
        isKinged = false;
        movement = gameObject.GetComponent<PieceMovement>();
    }

    //constructor not being used
    public CheckerPiece(int[] pos, bool black)
	{
        //position[0] = pos[0];
        //position[1] = pos[1];
        //this.isBlack = black;
        //movement = new PieceMovement();	
    }
	
    #region "Get Methods"
	
	public int[] getPosition() { return position; }
	
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

    //used for checking for possible future moves of a piece
    public bool checkIfPossibleMoves(GameBoard board)
    {
        List<GameObject> Lbc = new List<GameObject>(); //moves

        // return array list of possible board cells
        if (isKinged) { Lbc = movement.getValidKingMoves(position, board); }
        else { Lbc = movement.getValidMoves(position, board); }

        //Are there possible moves? If not, just return false.
        if (Lbc.Count > 0)
        {
            //Now we also have to check that the spaces we'd move to
            //wouldnt just be places that the opponent could jump us
            //so there should also be spaces to move to after our first movement
            //this occurs when:
            //1. when your moving one place, and none of the next possible pieces are
            //   occupied by an opponent, where the place theyd jump to is empty
            //2. 
            //basically im trying to check for a check mate
            foreach (GameObject bc in Lbc)
            {
                Debug.Log(bc.GetComponent<BoardCell>().getPosition()[0]+", "+bc.GetComponent<BoardCell>().getPosition()[1]);
                
                //if theres even one safe place to go to, return true
                //wont work because theres no piece in the space
                if (bc.GetComponent<BoardCell>().checkIfOccupied())
                {

                }
                if (movement.getValidMoves(bc.GetComponent<BoardCell>().getPosition(), board).Count > 0)
                {
                    return true;
                }
            }

            //if no safe moves were found
            return false;
        }
        else
        {
            return false;
        }
    }

    //make a piece a king
    public void promote()
	{
		isKinged = true;
		//code to switch regular piece with king piece visually?
	}
    
    #endregion
}
