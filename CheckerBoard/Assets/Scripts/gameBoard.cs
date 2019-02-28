using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public GameObject redTilePrefab;
    public GameObject blackTilePrefab;
    public GameObject redPiece;
    public GameObject blackPiece;
    public GameObject[ , ] squares = new GameObject[8, 8];
    public GameObject[ , ] pieces = new GameObject[8, 8];

    public GameBoard(){ }

    #region "Handle Board Items"

    public GameObject getBoardCell(int bcX, int bcY)
    {
        if ((bcX < 8 && bcX >= 0) && (bcY < 8 && bcY >= 0))
        { return squares[bcX, bcY]; }
        else { return null; }
    }

    public GameObject getPieceOnBoard(int pcX, int pcY)
    {
        if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
        { return pieces[pcX, pcY]; }
        else { return null; }
    }

    public void clearPieceOnBoard(int pcX, int pcY)
    {
        if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
        {
            pieces[pcX, pcY] = null;
            squares[pcX, pcY].GetComponent<BoardCell>().clearSpace();
        }
    }

    public void setPieceOnBoard(GameObject pc, int pcX, int pcY)
    {
        if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
        {
            pieces[pcX, pcY] = pc;
            squares[pcX, pcY].GetComponent<BoardCell>().setPiece(pc.GetComponent<CheckerPiece>());
        }
    }

    #endregion

    #region "Handle Board"

    //the CreateBoard method uses the nested loop to generate x,y values for the board coordinates
    public void CreateBoard()
    {
        int[] pos = new int[2];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {                
                //A black tile only occurs when (i, j) blocks are both either even or odd 
                if (i % 2 != 0 && j % 2 != 0 || i % 2 == 0 && j % 2 == 0)
                {
                    squares[i, j] = Instantiate(blackTilePrefab, new Vector2(i, j), Quaternion.identity);
                }
                //If the coordinates are either both odd or even then a white tile is instantiated
                else
                {
                    squares[i, j] = Instantiate(redTilePrefab, new Vector2(i, j), Quaternion.identity);
                }

                //constuct empty non occupied board cells
                pos[0] = i;
                pos[1] = j;
                squares[i, j].GetComponent<BoardCell>().BoardCellConstruct(pos, false, null);
            }
        }
    }

    //This function puts all the pieces in their first spots on the board
    public void placeInitialPieces()
    {
        //logic to put all pieces in their initial spots 
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                // Instatiate black pieces in the following coordinates:
                // 0,2 2,2 4,2 6,2
                // 1,1 3,1 5,1 7,1
                // 0,0 2,0 4,0 6,0
                if (((i % 2 == 0) && (j == 0 || j == 2)) || ((i % 2 == 1) && (j == 1)))
                {
                    pieces[i, j] = Instantiate(blackPiece, new Vector2(i, j), Quaternion.identity);
                    pieces[i, j].GetComponent<CheckerPiece>().setPosition(i, j);
                    squares[i, j].GetComponent<BoardCell>().setPiece(pieces[i, j].GetComponent<CheckerPiece>());
                }
                // Instatiate red pieces in the following coordinates:
                // 1,7 3,7 5,7 7,7
                // 0,6 2,6 4,6 6,6
                // 1,5 3,5 5,5 7,5
                if (((i % 2 == 1) && (j == 5 || j == 7)) || ((i % 2 == 0) && (j == 6)))
                {
                    pieces[i, j] = Instantiate(redPiece, new Vector2(i, j), Quaternion.identity);
                    pieces[i, j].GetComponent<CheckerPiece>().setPosition(i, j);
                    squares[i, j].GetComponent<BoardCell>().setPiece(pieces[i, j].GetComponent<CheckerPiece>());
                }
            }
        }
    }

    // This function is used to clear the board of the yellow move squares; It only loops through the brown spaces
    public void setOriginalColors()
    {
        //brown squares on even colums
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                squares[i * 2, j * 2].GetComponent<SpriteRenderer>().color = squares[i * 2, j * 2].GetComponent<BoardCell>().orgColor;
            }
        }
        //brown squares on odd columns
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                squares[(i * 2) + 1, (j * 2) + 1].GetComponent<SpriteRenderer>().color = squares[(i * 2) + 1, (j * 2) + 1].GetComponent<BoardCell>().orgColor;
            }
        }
    }

    #endregion
}
