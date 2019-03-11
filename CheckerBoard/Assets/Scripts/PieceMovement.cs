using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour {

    public PieceMovement() {}
	
	#region "Get Methods"
	
    //This method returns a List of valid moves for a regular piece
    public List<GameObject> getValidMoves(int[] pos, GameBoard board)
	{
        List<GameObject> Lbc = new List<GameObject>();     //regular moves
        List<GameObject> jumpLbc = new List<GameObject>(); //possible jump moves
        int x = pos[0];
        int y = pos[1];
        int check;

        // First, if its black
        // true if black, false if not
        if (board.getBoardCell(x, y).GetComponent<BoardCell>().getPiece().getColor())
        {
            // upper left
            check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x - 1, y + 1), board.getBoardCell(x - 2, y + 2));
            if (check == 1) { Lbc.Add(board.getBoardCell(x - 1, y + 1)); }
            else if (check == 2) { jumpLbc.Add(board.getBoardCell(x - 2, y + 2)); }

            // upper right
            check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x + 1, y + 1), board.getBoardCell(x + 2, y + 2));
            if (check == 1) { Lbc.Add(board.getBoardCell(x + 1, y + 1)); }
            else if (check == 2) { jumpLbc.Add(board.getBoardCell(x + 2, y + 2)); }
        }
        else //else, it must be red
        {
            // bottom left
            check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x - 1, y - 1), board.getBoardCell(x - 2, y - 2));
            if (check == 1) { Lbc.Add(board.getBoardCell(x - 1, y - 1)); }
            else if (check == 2) { jumpLbc.Add(board.getBoardCell(x - 2, y - 2)); }

            // bottom right
            check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x + 1, y - 1), board.getBoardCell(x + 2, y - 2));
            if (check == 1) { Lbc.Add(board.getBoardCell(x + 1, y - 1)); }
            else if (check == 2) { jumpLbc.Add(board.getBoardCell(x + 2, y - 2)); }
        }
            
        if (jumpLbc.Count == 0) { return Lbc; }
        else { return jumpLbc; } //if there are jumps, a jump must be made
    }

    //This method returns a List of valid moves for a kinged piece
    public List<GameObject> getValidKingMoves(int[] pos, GameBoard board)
	{
        List<GameObject> Lbc = new List<GameObject>();
        List<GameObject> jumpLbc = new List<GameObject>();
        int x = pos[0];
        int y = pos[1];
        int check;

        // A king can move in all directions

        // upper left
        check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x - 1, y + 1), board.getBoardCell(x - 2, y + 2));
        if (check == 1) { Lbc.Add(board.getBoardCell(x - 1, y + 1)); }
        else if (check == 2) { jumpLbc.Add(board.getBoardCell(x - 2, y + 2)); }

        // upper right
        check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x + 1, y + 1), board.getBoardCell(x + 2, y + 2));
        if (check == 1) { Lbc.Add(board.getBoardCell(x + 1, y + 1)); }
        else if (check == 2) { jumpLbc.Add(board.getBoardCell(x + 2, y + 2)); }

        // bottom left
        check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x - 1, y - 1), board.getBoardCell(x - 2, y - 2));
        if (check == 1) { Lbc.Add(board.getBoardCell(x - 1, y - 1)); }
        else if (check == 2) { jumpLbc.Add(board.getBoardCell(x - 2, y - 2)); }

        // bottom right
        check = checkSpace(board.getBoardCell(x, y), board.getBoardCell(x + 1, y - 1), board.getBoardCell(x + 2, y - 2));
        if (check == 1) { Lbc.Add(board.getBoardCell(x + 1, y - 1)); }
        else if (check == 2) { jumpLbc.Add(board.getBoardCell(x + 2, y - 2)); }

        if (jumpLbc.Count == 0) { return Lbc; }
        else { return jumpLbc; } //A king must also take a jump if available
    }

    //Since I was writing repetitive code, I just put the code to check spaces to move to here
    //the reason I return numbers is to distinguish between jump moves and and regular moves
    //That way I can check that if there are available jumps
    public int checkSpace(GameObject mySpace, GameObject nextSpace, GameObject jumpSpace)
    {
        //check that space exists
        if (nextSpace != null)
        {
            //check if space is empty. Add it if it is return 1
            if (!nextSpace.GetComponent<BoardCell>().checkIfOccupied())
            { return 1; }
            else //check if you can jump the piece thats there
            {
                if (mySpace.GetComponent<BoardCell>().getPiece().getColor() != nextSpace.GetComponent<BoardCell>().getPiece().getColor()) //is it an opposing piece?
                {
                    //if the space to jump to exists and its free
                    if (jumpSpace != null && !jumpSpace.GetComponent<BoardCell>().checkIfOccupied()) 
                    {
                        return 2;
                    }
                }
            }
        }
        //if nothing works, return 0
        return 0;
    }
	
	#endregion
	
	#region "Set Methods"
	
	//will actually move piece
	public void move(GameObject piece, BoardCell bc)
	{
        //create vector to move piece
        int moveX = bc.getPosition()[0] - piece.GetComponent<CheckerPiece>().getPosition()[0]; 
        int moveY = bc.getPosition()[1] - piece.GetComponent<CheckerPiece>().getPosition()[1];
        Vector3 v = new Vector3(moveX, moveY, 0);

        //set new position of piece in CheckerPiece
        piece.GetComponent<CheckerPiece>().setPosition(bc.getPosition()[0], bc.getPosition()[1]);
        
        // is this the other side of the board? King the piece then
        if ((bc.getPosition()[1] == 0) || (bc.getPosition()[1] == 7))
        {
            
            if (!piece.GetComponent<CheckerPiece>().getIsKing()) //if its not already king
            {
                piece.GetComponent<CheckerPiece>().promote();
                //piece.GetComponent<CheckerPiece>().promote() = kingPiece;
                if (piece.GetComponent<CheckerPiece>().getColor())
                    piece.GetComponent<SpriteRenderer>().color = Color.gray;
                else
                {
                    piece.GetComponent<SpriteRenderer>().color = Color.magenta;

                }
            }
                   
        }

        //move piece physically
        piece.transform.Translate(v, Space.World);
    }
	
	#endregion
}
