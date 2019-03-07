using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkGameManager : NetworkBehaviour
{
    public bool isConnected;
    public bool isBlacksTurn;
    public GameBoard board;
    public int[] currentPiece = new int[2];
    //public ServerPlayer player; //wait what is this object?

    #region "Code that Runs Game"

    public bool checkWinState(bool isBlacksTurn)
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
        if(!isLocalPlayer){
            return;
        }       
        //if screen is clicked on
        if (Input.GetMouseButtonDown(0))
        {          
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            //ray shoots out from camera toward click and hits box collider 
            if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("PieceLayer")))
            {                
                if (hit.transform.gameObject.tag == "Piece") 
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
                        //get piece currently clicked on and set it on the new space
                        board.getPieceOnBoard(currentPiece[0], currentPiece[1]).GetComponent<PieceMovement>().move(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCell>());      
                        board.setPieceOnBoard(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCell>().getPosition()[1]);
                        board.clearPieceOnBoard(currentPiece[0], currentPiece[1]); //empty old space

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
                        }
                    }
                    //remove previous yellow tiles
                    board.setOriginalColors();
                }

                Debug.Log(hit.transform.gameObject.tag);
            }
        }
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
