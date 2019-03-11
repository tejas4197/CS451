using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; } 
    public bool isConnected;
    public bool isBlacksTurn;
    public bool playerBlack;
    public bool changeTurn;
    public bool clientThing;
    public int[] currentPiece = new int[2];
    public int[] oldPiece = new int[2];
    public GameBoard board;
    public Client c; 

    #region "Code that Runs Game"

    // A run of the game
    public void Start()
    {
        //Version of the game
        Debug.Log("Application Version : " + Application.version);

        // code to initialize the game:   
        Instance = this;

        // the host is black, opponent is red
        c = FindObjectOfType<Client>();
        playerBlack = c.isHost;

        //Initialize Board
        board = gameObject.GetComponent<GameBoard>();
        board.CreateBoard();
        board.placeInitialPieces();

        //Turn order (black goes first)
        isBlacksTurn = true;               
        changeTurn = true;
    }

    //game loop
    public void Update()
    {        
        //if screen is clicked on
        if (Input.GetMouseButtonDown(0))
        {             
            if (isBlacksTurn)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                rayAction("Black Piece", ray);
            } else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                rayAction("Red Piece", ray);
            }           
        }
    }

    public bool checkWinState(bool isBlacksTurn, GameBoard board)
    {
        if (board.getWinState(isBlacksTurn, board))
        {
            if (isBlacksTurn)
            {
                Debug.Log("Black Wins");
            }
            else
            {
                Debug.Log("Red Wins");
            }
        }

        return true;
    }

    public void rayAction(string pieceName, Ray ray)
    {
        //ray shoots out from camera toward click and hits box collider 
        if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("PieceLayer")) && (playerBlack == isBlacksTurn))
        {
            if ((hit.transform.gameObject.tag == pieceName) && changeTurn)
            {
                //if new piece is clicked on
                if (hit.transform.gameObject.GetComponent<CheckerPiece>().getPosition() != currentPiece)
                {
                    //record position of piece currently clicked on
                    currentPiece[0] = hit.transform.gameObject.GetComponent<CheckerPiece>().getPosition()[0];
                    currentPiece[1] = hit.transform.gameObject.GetComponent<CheckerPiece>().getPosition()[1];
                    //remove previous yellow tiles
                    board.setOriginalColors();
                }

                List<GameObject> Lbc = new List<GameObject>();
                Lbc = hit.transform.gameObject.GetComponent<CheckerPiece>().showValidMoves(board);

                for (int i = 0; i < Lbc.Count; i++)
                {
                    Lbc[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }

            if (hit.transform.gameObject.tag == "Square")
            {
                //if a yellow tile was clicked on
                if (board.squares[hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1]].GetComponent<SpriteRenderer>().color == Color.yellow)
                {
                    //Firstly, we're able to take turns after each move
                    changeTurn = true;

                    board.getPieceOnBoard(currentPiece[0], currentPiece[1]).GetComponent<PieceMovement>().move(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCell>());
                    board.setPieceOnBoard(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1]);
                    board.clearPieceOnBoard(currentPiece[0], currentPiece[1]); //empty old space

                    //if the difference in y between the old and new space is two spaces, that means a jump was made
                    if (Mathf.Abs(hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1] - currentPiece[1]) == 2)
                    {
                        eliminatePiece(hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1], currentPiece[0], currentPiece[1]);

                        //after jumping set original colors
                        board.setOriginalColors();

                        //can we jump again?
                        List<GameObject> jumpAgain = new List<GameObject>();
                        jumpAgain = hit.transform.gameObject.GetComponent<BoardCell>().getPiece().showValidMoves(board);

                        foreach (GameObject jb in jumpAgain)
                        {
                            if (Mathf.Abs(jb.GetComponent<BoardCell>().getPosition()[1] - hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1]) == 2)
                            {
                                jb.GetComponent<SpriteRenderer>().color = Color.yellow;
                                oldPiece[0] = currentPiece[0];
                                oldPiece[1] = currentPiece[1];
                                currentPiece[0] = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0];
                                currentPiece[1] = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1];                               
                                changeTurn = false;
                            }
                        }
                    }

                    if (changeTurn)
                    {
                        c.Send("CMOV|" + hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0] + "|" + hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1] + "|" + currentPiece[0] + "|" + currentPiece[1] + "|1");
                        //remove previous yellow tiles (if were changing turns)
                        board.setOriginalColors();
                    }
                    else
                    {
                        c.Send("CMOV|" + hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0] + "|" + hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1] + "|" + oldPiece[0] + "|" + oldPiece[1] + "|0");
                    }
                }                             
            }
        }
    }

    public void Turn(int cTurn)
    {
        //if were changing turns
        if (cTurn == 1) { isBlacksTurn = !isBlacksTurn; }
    }
  
    public void gameMove(int squareX, int squareY, int pieceX, int pieceY)
    {
        //get board cell we're touching
        GameObject square = board.getBoardCell(squareX, squareY);   

        //If the piece exits (on the other client), move the jump 
        if (board.getPieceOnBoard(pieceX, pieceY) != null)
        {
            GameObject piece = board.getPieceOnBoard(pieceX, pieceY);
            piece.GetComponent<PieceMovement>().move(piece, square.GetComponent<BoardCell>());
            board.setPieceOnBoard(piece, square.GetComponent<BoardCell>().getPosition()[0], square.GetComponent<BoardCell>().getPosition()[1]);
            board.clearPieceOnBoard(pieceX, pieceY); 
            eliminatePiece(squareX, squareY, pieceX, pieceY);
        }
              
        //check if someone has won after every move
        checkWinState(isBlacksTurn, board);        
    }

    public void eliminatePiece(int squareX, int squareY, int pieceX, int pieceY)
    {
        //if the difference in y between the old and new space is two spaces, that means a jump was made
        if (Mathf.Abs(squareY - pieceY) == 2)
        {
            //get X and Y position of piece jumped over
            int captureX = squareX - pieceX;
            int captureY = squareY - pieceY;
            if (captureX == -2) { captureX = -1; } else { captureX = 1; }
            if (captureY == -2) { captureY = -1; } else { captureY = 1; }
            Destroy(board.getPieceOnBoard(pieceX + captureX, pieceY + captureY));
            board.clearPieceOnBoard(pieceX + captureX, pieceY + captureY);
        }
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
}
