using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameManagerTs : MonoBehaviour
    {
        public static GameManagerTs Instance { set; get; }
        public GameBoardTs board;
        //public Text turnText;
        //public Text winLoseText;
        //public Client c;
        public bool isBlacksTurn;
        public bool playerBlack;
        public bool changeTurn;
        public int[] currentPiece = new int[2];
        public int[] oldPiece = new int[2];

        // Force Capture Global Vars
        public bool forceCapture;
        public bool forceSquare;
        public List<CheckerPieceTs> forcePieces;
        public List<GameObject> globalValid;

        #region "Code that Runs Game"   

        // Start the game
        public void Start()
        {
            // the player hits play game, or start whatever
            // code to initialize the game:
            // also the host is black, opponent is red
            Instance = this;

            //connect client
            //c = FindObjectOfType<Client>();
            //if (c != null) { playerBlack = c.isHost; }

            //set board up
            board = gameObject.GetComponent<GameBoardTs>();
            board.CreateBoard();
            board.placeInitialPieces();
            isBlacksTurn = true;
            changeTurn = true;
        }

        //game loop
        public void Update()
        {
            //if screen is clicked on
            if (Input.GetMouseButtonDown(0))
            {
                if (isBlacksTurn && playerBlack)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    rayAction("Black Piece", ray);
                }
                else if (!isBlacksTurn && !playerBlack)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    rayAction("Red Piece", ray);
                }
            }

            if (isBlacksTurn)
            {
                //turnText.text = "Black's Turn";
                //turnText.color = new Color(0, 0, 0, 1);
            }
            else
            {
                //turnText.text = "Red's Turn";
                //turnText.color = new Color(255, 0, 0);
            }
        }

        public void checkWinState(bool isBlacksTurn, GameBoardTs board)
        {
            if (board.getWinState(isBlacksTurn, board))
            {
                if (isBlacksTurn)
                {
                   // winLoseText.text = "Black Wins";
                }
                else
                {
                    //winLoseText.text = "Red Wins";
                }
            }
        }

        public void rayAction(string pieceName, Ray ray)
        {
            //ray shoots out from camera toward click and hits box collider 
            if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("PieceLayer")) && (playerBlack == isBlacksTurn))
            {

                if (forceCapture)
                {
                    CheckerPieceTs hitPiece = hit.transform.gameObject.GetComponent<CheckerPieceTs>();

                    scanCaptureForcePieces();

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
                    movePiece(pieceName, hit);
                }
            }
        }

        public void movePiece(string pieceName, RaycastHit hit)
        {
            if ((hit.transform.gameObject.tag == pieceName) && changeTurn)
            {
                //if new piece is clicked on
                if (hit.transform.gameObject.GetComponent<CheckerPieceTs>().getPosition() != currentPiece)
                {
                    //record position of piece currently clicked on
                    currentPiece[0] = hit.transform.gameObject.GetComponent<CheckerPieceTs>().getPosition()[0];
                    currentPiece[1] = hit.transform.gameObject.GetComponent<CheckerPieceTs>().getPosition()[1];
                    //remove previous yellow tiles
                    board.setOriginalColors();
                }

                List<GameObject> Lbc = new List<GameObject>();
                Lbc = hit.transform.gameObject.GetComponent<CheckerPieceTs>().showValidMoves(board);
                globalValid = Lbc;

                for (int i = 0; i < Lbc.Count; i++)
                {
                    Lbc[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }

            if (hit.transform.gameObject.tag == "Square")
            {
                //if a yellow tile was clicked on
                if (board.squares[hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1]].GetComponent<SpriteRenderer>().color == Color.yellow)
                {
                    string toForce = "no";

                    if (forceCapture)
                    {
                        toForce = "yes";
                    }

                    if (!forceCapture && toForce == "yes")
                    {
                        forceCapture = true;
                    }

                    if (forceCapture && toForce == "no")
                    {
                        forceCapture = false;
                    }

                    //Usually, we're able to take turns after each move
                    changeTurn = true;

                    board.getPieceOnBoard(currentPiece[0], currentPiece[1]).GetComponent<PieceMovementTs>().move(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCellTs>());
                    board.setPieceOnBoard(board.getPieceOnBoard(currentPiece[0], currentPiece[1]), hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1]);
                    board.clearPieceOnBoard(currentPiece[0], currentPiece[1]); //empty old space

                    //if the difference in y between the old and new space is two spaces, that means a jump was made
                    if (Mathf.Abs(hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1] - currentPiece[1]) == 2)
                    {
                        //Eliminate piece that was jumped over
                        eliminatePiece(hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[0], hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1], currentPiece[0], currentPiece[1]);

                        //after jumping set original colors
                        board.setOriginalColors();

                        //can we jump again?
                        List<GameObject> jumpAgain = new List<GameObject>();
                        jumpAgain = hit.transform.gameObject.GetComponent<BoardCellTs>().getPiece().showValidMoves(board);

                        foreach (GameObject jb in jumpAgain)
                        {
                            if (Mathf.Abs(jb.GetComponent<BoardCellTs>().getPosition()[1] - hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1]) == 2)
                            {
                                jb.GetComponent<SpriteRenderer>().color = Color.yellow;
                                oldPiece[0] = currentPiece[0];
                                oldPiece[1] = currentPiece[1];
                                currentPiece[0] = hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[0];
                                currentPiece[1] = hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1];
                                changeTurn = false;
                            }
                        }
                    }

                    if (changeTurn)
                    {
                        //c.Send("CMOV|" + hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[0] + "|" + hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1] + "|" + currentPiece[0] + "|" + currentPiece[1] + "|" + "yes" + "|" + toForce);

                        //remove previous yellow tiles (if were changing turns)
                        board.setOriginalColors();
                    }
                    else
                    {
                        //c.Send("CMOV|" + hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[0] + "|" + hit.transform.gameObject.GetComponent<BoardCellTs>().getPosition()[1] + "|" + oldPiece[0] + "|" + oldPiece[1] + "|" + "no" + "|" + toForce);
                    }
                }
            }
        }

        public void Turn(string cTurn)
        {
            //if were changing turns
            if (cTurn == "yes") { isBlacksTurn = !isBlacksTurn; }
        }

        //Function that handles all actual game moves
        public void gameMove(int squareX, int squareY, int pieceX, int pieceY, string toChange, string toForce)
        {
            if (!forceCapture && toForce == "yes")
            {
                forceCapture = true;
            }

            if (forceCapture && toForce == "no")
            {
                forceCapture = false;
            }

            //get board cell we're touching
            GameObject square = board.getBoardCell(squareX, squareY);

            //If the piece exists (on the other client), move it
            if (board.getPieceOnBoard(pieceX, pieceY) != null)
            {
                //Move the piece
                GameObject piece = board.getPieceOnBoard(pieceX, pieceY);
                piece.GetComponent<PieceMovementTs>().move(piece, square.GetComponent<BoardCellTs>());
                board.setPieceOnBoard(piece, square.GetComponent<BoardCellTs>().getPosition()[0], square.GetComponent<BoardCellTs>().getPosition()[1]);
                //Remove piece from previous position
                board.clearPieceOnBoard(pieceX, pieceY);
                //Check if piece was a jump
                eliminatePiece(squareX, squareY, pieceX, pieceY);
            }

            //check if someone has won after every move
            checkWinState(isBlacksTurn, board);

            if (changeTurn) scanForCapture();
        }

        //Function that removes pieces when jumped over
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

        //Function that checks if there are any available jumps for a team
        //That player must jump if there is
        public void scanForCapture()
        {
            forcePieces = new List<CheckerPieceTs>();
            GameObject pieceExist;
            if (isBlacksTurn) // When black turn finish, check if red has to capture
            {
                CheckerPieceTs redPiece;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        pieceExist = board.getPieceOnBoard(x, y);

                        if (pieceExist != null)
                        {
                            redPiece = pieceExist.GetComponent<CheckerPieceTs>();

                            if (redPiece.transform.gameObject.tag == "Red Piece")
                            {
                                List<GameObject> validMoves = new List<GameObject>();
                                validMoves = redPiece.showValidMoves(board);

                                foreach (GameObject move in validMoves)
                                {
                                    if (Mathf.Abs(redPiece.getPosition()[1] - move.GetComponent<BoardCellTs>().getPosition()[1]) == 2)
                                    {
                                        forcePieces.Add(redPiece);
                                        forceCapture = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!isBlacksTurn) // When Red finishes, check if black must force capture
            {
                CheckerPieceTs blackPiece;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        pieceExist = board.getPieceOnBoard(x, y);

                        if (pieceExist != null)
                        {
                            blackPiece = pieceExist.GetComponent<CheckerPieceTs>();

                            if (blackPiece.transform.gameObject.tag == "Black Piece")
                            {
                                List<GameObject> validMoves = new List<GameObject>();
                                validMoves = blackPiece.showValidMoves(board);

                                foreach (GameObject move in validMoves)
                                {
                                    if (Mathf.Abs(blackPiece.getPosition()[1] - move.GetComponent<BoardCellTs>().getPosition()[1]) == 2)
                                    {
                                        forcePieces.Add(blackPiece);
                                        forceCapture = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void scanCaptureForcePieces()
        {
            forcePieces = new List<CheckerPieceTs>();
            GameObject pieceExist;
            if (!isBlacksTurn) // When black turn finish, check if red has to capture
            {
                CheckerPieceTs redPiece;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        pieceExist = board.getPieceOnBoard(x, y);

                        if (pieceExist != null)
                        {
                            redPiece = pieceExist.GetComponent<CheckerPieceTs>();

                            if (redPiece.transform.gameObject.tag == "Red Piece")
                            {
                                List<GameObject> validMoves = new List<GameObject>();
                                validMoves = redPiece.showValidMoves(board);

                                foreach (GameObject move in validMoves)
                                {
                                    if (Mathf.Abs(redPiece.getPosition()[1] - move.GetComponent<BoardCellTs>().getPosition()[1]) == 2)
                                    {
                                        forcePieces.Add(redPiece);
                                        forceCapture = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (isBlacksTurn) // When Red finishes, check if black must force capture
            {
                CheckerPieceTs blackPiece;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        pieceExist = board.getPieceOnBoard(x, y);

                        if (pieceExist != null)
                        {
                            blackPiece = pieceExist.GetComponent<CheckerPieceTs>();

                            if (blackPiece.transform.gameObject.tag == "Black Piece")
                            {
                                List<GameObject> validMoves = new List<GameObject>();
                                validMoves = blackPiece.showValidMoves(board);

                                foreach (GameObject move in validMoves)
                                {
                                    if (Mathf.Abs(blackPiece.getPosition()[1] - move.GetComponent<BoardCellTs>().getPosition()[1]) == 2)
                                    {
                                        forcePieces.Add(blackPiece);
                                        forceCapture = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnGUI()
        {
            int W = Screen.width, H = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, 0, W, H * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = H * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            string text = "Version: " + Application.version;
            GUI.Label(rect, text, style);
        }

        #endregion
    }

    public class GameManagerT
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            GameManagerTs Gm = new GameManagerTs();
            //GameBoardTs Bd = new GameBoardTs(); 
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {

            yield return null;
        }
    }
}
