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
    public GameBoard board;
    public int[] currentPiece = new int[2];
    public Server player;
    public Client c;

    public bool mustJump; 
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
        Instance = this;

        c = FindObjectOfType<Client>();

        board = gameObject.GetComponent<GameBoard>();
        board.CreateBoard();
        board.placeInitialPieces();
        isBlacksTurn = true;        
        playerBlack = c.isHost;
        changeTurn = true;
        mustJump = false;

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
            Debug.Log("BlackTurn is " + isBlacksTurn + " and playerBLack is " + playerBlack);
            if (isBlacksTurn && playerBlack)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                rayAction("Black Piece", ray);
            } else if (!isBlacksTurn && !playerBlack)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                rayAction("Red Piece", ray);
            }           
        }
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
                    string toChange = "no";
                    if (changeTurn && !mustJump)
                    {
                        changeTurns();
                        toChange = "yes";
                        board.setOriginalColors();
                    }

                    gameMove(hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1], currentPiece[0], currentPiece[1], toChange);
                    c.Send("CMOV|" + hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0] + "|" + hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1] + "|" + currentPiece[0] + "|" + currentPiece[1] + "|" + toChange);
                   
                }
                //remove previous yellow tiles (if were changing turns)
                      
            }

           // Debug.Log(hit.transform.gameObject.tag);
        }
    }
  
    public void gameMove(int squareX, int squareY, int pieceX, int pieceY, string toChange)
    {
       
        if (toChange == "yes")
        {
            changeTurns();
            mustJump = false;
        }

        //get board cell we're touching
        GameObject square = board.getBoardCell(squareX, squareY);
        GameObject piece = board.getPieceOnBoard(pieceX, pieceY);
        
        if(piece != null && square != null)
        {
            piece.GetComponent<PieceMovement>().move(board.getPieceOnBoard(pieceX, pieceY), square.GetComponent<BoardCell>());
            board.setPieceOnBoard(board.getPieceOnBoard(pieceX, pieceY), square.GetComponent<BoardCell>().getPosition()[0], square.GetComponent<BoardCell>().getPosition()[1]);
            changeTurn = true;
        }      

        //get piece currently clicked on and set it on the new space
        board.clearPieceOnBoard(pieceX, pieceY); //empty old space

        //if the difference in y between the old and new space is two spaces, that means a jump was made
        if (Mathf.Abs(square.GetComponent<BoardCell>().getPosition()[1] - pieceY) == 2)
        {
            //get X and Y position of piece jumped over
            int captureX = square.GetComponent<BoardCell>().getPosition()[0] - pieceX;
            int captureY = square.GetComponent<BoardCell>().getPosition()[1] - pieceY;
            if (captureX == -2) { captureX = -1; } else { captureX = 1; }
            if (captureY == -2) { captureY = -1; } else { captureY = 1; }
            Destroy(board.getPieceOnBoard(pieceX + captureX, pieceY + captureY));
            board.clearPieceOnBoard(pieceX + captureX, pieceY + captureY);

            //after jumping set original colors
            board.setOriginalColors();

            //can we jump again?
            List<GameObject> jumpAgain = new List<GameObject>();
            jumpAgain = square.GetComponent<BoardCell>().getPiece().showValidMoves(board);

            foreach (GameObject jb in jumpAgain)
            {
                if (Mathf.Abs(jb.GetComponent<BoardCell>().getPosition()[1] - square.GetComponent<BoardCell>().getPosition()[1]) == 2)
                {
                    jb.GetComponent<SpriteRenderer>().color = Color.yellow;
                    pieceX = square.GetComponent<BoardCell>().getPosition()[0];
                    pieceY = square.GetComponent<BoardCell>().getPosition()[1];
                    changeTurn = false;
                    mustJump = true;
                }
            }
        }

        //check if someone has won after every move
        checkWinState(isBlacksTurn, board);
    }

    public void changeTurns()
    {
        Debug.Log("Inside changeTurns func");
        if (isBlacksTurn)
        {
            Debug.Log("Blacks Turn become false");
            isBlacksTurn = false;

        }
        else
        {
            Debug.Log("Blacks Turn become true");
            isBlacksTurn = true;
        }
  //      c.Send("CHMOV|" + isBlacksTurn);
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
