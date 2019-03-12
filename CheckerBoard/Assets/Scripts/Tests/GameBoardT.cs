using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameBoardTs : MonoBehaviour
    {
        public GameObject redTilePrefab;
        public GameObject blackTilePrefab;
        public GameObject redPiece;
        public GameObject blackPiece;
        public GameObject[,] squares = new GameObject[8, 8];
        public GameObject[,] pieces = new GameObject[8, 8];

        public string[,] squaresTest = new string[8, 8];
        public string[,] piecesTest = new string[8, 8];

        public GameBoardTs() { }

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
                squares[pcX, pcY].GetComponent<BoardCellTs>().clearSpace();
            }
        }

        public void setPieceOnBoard(GameObject pc, int pcX, int pcY)
        {
            if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
            {
                pieces[pcX, pcY] = pc;
                squares[pcX, pcY].GetComponent<BoardCellTs>().setPiece(pc.GetComponent<CheckerPieceTs>());
            }
        }

        public string getBoardCellTest(int bcX, int bcY)
        {
            if ((bcX < 8 && bcX >= 0) && (bcY < 8 && bcY >= 0))
            { return squaresTest[bcX, bcY]; }
            else { return null; }
        }

        public string getPieceOnBoardTest(int pcX, int pcY)
        {
            if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
            { return piecesTest[pcX, pcY]; }
            else { return null; }
        }

        public void clearPieceOnBoardTest(int pcX, int pcY)
        {
            if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
            {
                pieces[pcX, pcY] = null;
                squares[pcX, pcY].GetComponent<BoardCellTs>().clearSpace();
            }
        }

        public void setPieceOnBoardTest(GameObject pc, int pcX, int pcY)
        {
            if ((pcX < 8 && pcX >= 0) && (pcY < 8 && pcY >= 0))
            {
                pieces[pcX, pcY] = pc;
                squares[pcX, pcY].GetComponent<BoardCellTs>().setPiece(pc.GetComponent<CheckerPieceTs>());
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
                        //squares[i, j] = Instantiate(blackTilePrefab, new Vector2(i, j), Quaternion.identity);
                        squaresTest[i, j] = "black|" + i.ToString() + "|" + j.ToString();
                    }
                    //If the coordinates are either both odd or even then a white tile is instantiated
                    else
                    {
                        //squares[i, j] = Instantiate(redTilePrefab, new Vector2(i, j), Quaternion.identity);
                        squaresTest[i, j] = "red|" + i.ToString() + "|" + j.ToString();
                    }

                    squaresTest[i, j] += "|false|null"; 
                    //constuct empty non occupied board cells
                    //pos[0] = i;
                    //pos[1] = j;
                    //squares[i, j].GetComponent<BoardCellTs>().BoardCellConstruct(pos, false, null);
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
                        //pieces[i, j] = Instantiate(blackPiece, new Vector2(i, j), Quaternion.identity);
                        //pieces[i, j].GetComponent<CheckerPieceTs>().setPosition(i, j);
                        //squares[i, j].GetComponent<BoardCellTs>().setPiece(pieces[i, j].GetComponent<CheckerPieceTs>());
                        squaresTest[i, j] = "black|" + i.ToString() + "|" + j.ToString();
                    }
                    // Instatiate red pieces in the following coordinates:
                    // 1,7 3,7 5,7 7,7
                    // 0,6 2,6 4,6 6,6
                    // 1,5 3,5 5,5 7,5
                    if (((i % 2 == 1) && (j == 5 || j == 7)) || ((i % 2 == 0) && (j == 6)))
                    {
                        //pieces[i, j] = Instantiate(redPiece, new Vector2(i, j), Quaternion.identity);
                        //pieces[i, j].GetComponent<CheckerPieceTs>().setPosition(i, j);
                        //squares[i, j].GetComponent<BoardCellTs>().setPiece(pieces[i, j].GetComponent<CheckerPieceTs>());
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
                    squares[i * 2, j * 2].GetComponent<SpriteRenderer>().color = squares[i * 2, j * 2].GetComponent<BoardCellTs>().orgColor;
                }
            }
            //brown squares on odd columns
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    squares[(i * 2) + 1, (j * 2) + 1].GetComponent<SpriteRenderer>().color = squares[(i * 2) + 1, (j * 2) + 1].GetComponent<BoardCellTs>().orgColor;
                }
            }
        }

        // This function is used to check the win state of a particular team
        // it returns false immeditely upon discovering that just one piece has possible moves
        public bool getWinState(bool isBlacksTurn, GameBoardTs board)
        {
            //check pieces on squares on even columns
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (squares[i * 2, j * 2].GetComponent<BoardCellTs>().checkIfOccupied())
                    {
                        if (pieces[i * 2, j * 2].GetComponent<CheckerPieceTs>().getColor() != isBlacksTurn)
                        {
                            if (pieces[i * 2, j * 2].GetComponent<CheckerPieceTs>().showValidMoves(board).Count > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            //check pieces on squares on odd columns
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (squares[(i * 2) + 1, (j * 2) + 1].GetComponent<BoardCellTs>().checkIfOccupied())
                    {
                        if (pieces[(i * 2) + 1, (j * 2) + 1].GetComponent<CheckerPieceTs>().getColor() != isBlacksTurn)
                        {
                            if (pieces[(i * 2) + 1, (j * 2) + 1].GetComponent<CheckerPieceTs>().showValidMoves(board).Count > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            //return true when no pieces had any possible moves
            return true;
        }
        #endregion
    }

    public class GameBoardT
    {
        // A Test behaves as an ordinary method
        [Test]
        public void GameBoardTSimplePasses()
        {
            // Use the Assert class to test conditions

            GameBoardTs gMB = new GameObject().AddComponent<GameBoardTs>();

            gMB.CreateBoard();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string[] parse = gMB.getBoardCellTest(i, j).Split('|');
                    if (i % 2 != 0 && j % 2 != 0 || i % 2 == 0 && j % 2 == 0)
                    {
                        Assert.AreEqual(parse[0], "black");
                    }
                    else
                    {
                        Assert.AreEqual(parse[0], "red");
                    }
                    Assert.AreEqual(parse[1], i.ToString());
                    Assert.AreEqual(parse[2], j.ToString());
                    Assert.AreEqual(parse[3], "false");
                    Assert.AreEqual(parse[4], "null");
                }
            }

            gMB.placeInitialPieces();

        }
    }
}
