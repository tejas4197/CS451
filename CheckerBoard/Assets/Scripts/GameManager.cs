using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isConnected;
    public bool isBlacksTurn;
    public bool changeTurn;
    public GameBoard board;
    public int[] currentPiece = new int[2];

    public bool forceCapture;
    public bool forceSquare;
    public List<CheckerPiece> forcePieces;

    public List<GameObject> globalValid;

    public int movedX, movedY;

    //public ServerPlayer player; //wait what is this object?

    #region "Code that Runs Game"

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

    // A run of the game
    public void Start()
    {
        // the player hits play game, or start whatever
        // code to initialize the game:
        // also the host is black, opponent is red
        // this.initializeOnClient();
        // this.loadClientScene();
        // this.initializeServer();

        board = gameObject.GetComponent<GameBoard>();
        board.CreateBoard();
        board.placeInitialPieces();
        isBlacksTurn = true;
        changeTurn = true;

        // if winstate returns true (meaning a player won, then break the loop)
        /*
        while (!checkWinState(isBlacksTurn))
        {
            this.takeTurn(isBlacksTurn);
            if (isBlacksTurn) { isBlacksTurn = false; }
            else { isBlacksTurn = true; }
        }
        */
        
        // isBlacksturn can be used to determine who won
        this.gameOver(isBlacksTurn);
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
                moveAction("Black Piece", ray);
            } else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                moveAction("Red Piece", ray);
            }           
        }
    }

    public void checkForceCapture()
    {
        forcePieces = new List<CheckerPiece>();

        CheckerPiece piece = board.getPieceOnBoard(movedX, movedY).GetComponent<CheckerPiece>();
        List<GameObject> Lbc = new List<GameObject>();
        Lbc = piece.showValidMoves(board);
        int apX, apY;
        GameObject pieceExist;
        CheckerPiece checkPiece;

        // Force logic for Red Pieces
        if (isBlacksTurn)
        {
            GameObject leftPiece = board.getPieceOnBoard(movedX - 1, movedY + 1);
            GameObject rightPiece = board.getPieceOnBoard(movedX + 1, movedY + 1);

            if (leftPiece != null)
            {
                if (leftPiece.transform.gameObject.tag == "Red Piece")
                {
                    apX = movedX + 1;
                    apY = movedY - 1;
                    pieceExist = board.getPieceOnBoard(apX, apY);

                    if (pieceExist == null && (apX < 8) && (apY < 8))
                    {
                        forcePieces.Add(board.getPieceOnBoard(movedX - 1, movedY + 1).GetComponent<CheckerPiece>());
                        forceCapture = true;
                    }
                }
            }

            if (rightPiece != null)
            {
                if (rightPiece.transform.gameObject.tag == "Red Piece")
                {
                    apX = movedX - 1;
                    apY = movedY - 1;
                    pieceExist = board.getPieceOnBoard(apX, apY);

                    if (pieceExist == null && (apX < 8) && (apY < 8))
                    {
                        forcePieces.Add(board.getPieceOnBoard(movedX + 1, movedY + 1).GetComponent<CheckerPiece>());
                        forceCapture = true;
                    }
                }
            }
        }

        // Force logic for Black Pieces
        else if (!isBlacksTurn)
        {
            GameObject leftPiece = board.getPieceOnBoard(movedX - 1, movedY - 1);
            GameObject rightPiece = board.getPieceOnBoard(movedX + 1, movedY - 1);

            if (leftPiece != null)
            {
                if (leftPiece.transform.gameObject.tag == "Black Piece")
                {
                    apX = movedX + 1;
                    apY = movedY + 1;
                    pieceExist = board.getPieceOnBoard(apX, apY);

                    if (pieceExist == null && (apX < 8) && (apY < 8))
                    {
                        forcePieces.Add(board.getPieceOnBoard(movedX - 1, movedY - 1).GetComponent<CheckerPiece>());
                        forceCapture = true;
                    }
                }
            }

            if (rightPiece != null)
            {
                if (rightPiece.transform.gameObject.tag == "Black Piece")
                {
                    apX = movedX - 1;
                    apY = movedY + 1;
                    pieceExist = board.getPieceOnBoard(apX, apY);

                    if (pieceExist == null && (apX < 8) && (apY < 8))
                    {
                        forcePieces.Add(board.getPieceOnBoard(movedX + 1, movedY - 1).GetComponent<CheckerPiece>());
                        forceCapture = true;
                    }
                }
            }
        }
    }

    public void moveAction(string pieceName, Ray ray)
    {
        //ray shoots out from camera toward click and hits box collider 
        if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("PieceLayer")))
        {

            // Player is forced to capture the attackable piece 
            if (forceCapture)
            {
                Debug.Log("Inside Force Capture");
                CheckerPiece hitPiece = hit.transform.gameObject.GetComponent<CheckerPiece>();

                for (int i = 0; i < forcePieces.Count; i++)
                {
                    if (hitPiece == forcePieces[i])
                    {
                        forceSquare = true;
                        movePiece(pieceName, hit);
                    }
                }

                // ForceSquare is true when player hits the piece that must attack. 
                // Forces player to not be able to hit any other tiles besides the square he must attack.
                if (forceSquare)
                {
                    GameObject hitCell = hit.transform.gameObject;                  

                    for (int i = 0; i < globalValid.Count; i++)
                    {
                        if (hitCell == globalValid[i])
                        {
                            forceSquare = false;
                            forceCapture = false;
                            movePiece(pieceName, hit);
                        }
                    }
                }
            }

            // Regular move action
            else if (forceCapture == false && forceSquare == false)
            {
                Debug.Log("Inside Else");
                movePiece(pieceName, hit);
            }

        }
    }

    public void movePiece(string pieceName, RaycastHit hit)
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
            globalValid = Lbc;

            for (int i = 0; i < Lbc.Count; i++)
            {
                Lbc[i].GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }

        if (hit.transform.gameObject.tag == "Square")
        {
            captureAction(hit);
        }

        Debug.Log(hit.transform.gameObject.tag);
    }

    public void captureAction(RaycastHit hit)
    {
        //if a yellow tile was clicked on
        if (board.squares[hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1]].GetComponent<SpriteRenderer>().color == Color.yellow)
        {
            //Firstly, we're able to take turns after each move
            changeTurn = true;

            //get piece currently clicked on and set it on the new space
            board.getPieceOnBoard(currentPiece[0], currentPiece[1]).GetComponent<PieceMovement>().move(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCell>());
            board.setPieceOnBoard(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1]);
            board.clearPieceOnBoard(currentPiece[0], currentPiece[1]); //empty old space

            movedY = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1];
            movedX = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0];

            //if the difference in y between the old and new space is two spaces, that means a jump was made
            if (Mathf.Abs(hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1] - currentPiece[1]) == 2)
            {
                //get X and Y position of piece jumped over
                int captureX = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0] - currentPiece[0];
                int captureY = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1] - currentPiece[1];
                if (captureX == -2) { captureX = -1; } else { captureX = 1; }
                if (captureY == -2) { captureY = -1; } else { captureY = 1; }
                Destroy(board.getPieceOnBoard(currentPiece[0] + captureX, currentPiece[1] + captureY));
                board.clearPieceOnBoard(currentPiece[0] + captureX, currentPiece[1] + captureY);

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
                        currentPiece[0] = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0];
                        currentPiece[1] = hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1];
                        changeTurn = false;
                    }
                }
            }

            // Checks if piece should be captured
            checkForceCapture();

            //check if someone has won after every move
            checkWinState(isBlacksTurn, board);

            if (changeTurn) //if were changing turns
            {
                if (isBlacksTurn) { isBlacksTurn = false; }
                else { isBlacksTurn = true; }
            }
        }
        //remove previous yellow tiles (if were changing turns)
        if (changeTurn) { board.setOriginalColors(); }
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

    public void initializeOnClient() { }

    public void initializeOnServer() { }

    private IEnumerator loadClientScene() { return null; }

    #endregion
}
