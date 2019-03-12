using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CheckerPieceTs : MonoBehaviour
    {

        public bool isBlack;
        private bool isKinged;
        public int[] position = new int[2];  // position x, y        
        private PieceMovementTs movement;      // will handle all piece movement

        //Basically this works as the constructor
        public void Start()
        {
            isKinged = false;
            movement = gameObject.GetComponent<PieceMovementTs>();
        }

        public CheckerPieceTs() { }

        #region "Get Methods"

        public int[] getPosition() { return position; }

        //return true if piece is black, false otherwise
        public bool getColor() { return isBlack; }

        public bool getIsKing() { return isKinged; }

        public PieceMovementTs getMovement() { return movement; }

        #endregion

        #region "Set Methods"

        public void setPieceMovement(PieceMovementTs pm)
        {
            movement = pm;
        }

        public void setPosition(int x, int y)
        {
            position[0] = x;
            position[1] = y;
        }

        //called when piece is clicked on
        //will highlight valid spots on board	
        public List<GameObject> showValidMoves(GameBoardTs board)
        {
            // return array list of possible board cells
            if (isKinged) { return movement.getValidKingMoves(position, board); }
            else { return movement.getValidMoves(position, board); }
        }

        //make a piece a king
        public void promote()
        {
            isKinged = true;
        }

        #endregion
    }

    public class CheckerPieceT
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CheckerPieceTsSimplePasses()
        {
            // Test Game Board
            GameBoardTs gb = new GameObject().AddComponent<GameBoardTs>();
            gb.CreateBoard();
            gb.placeInitialPieces();

            // Test Checker Piece
            CheckerPieceTs cp = new GameObject().AddComponent<CheckerPieceTs>();
            cp.Start();

            // Test Board Cell
            BoardCellTs b1 = new GameObject().AddComponent<BoardCellTs>();
            BoardCellTs b2 = new GameObject().AddComponent<BoardCellTs>();
            BoardCellTs myCell = new GameObject().AddComponent<BoardCellTs>();
            int[] p1 = new int[] { 1, 3 };
            int[] p2 = new int[] { 3, 3 };
            int[] p3 = new int[] { 2, 2 };
            b1.GetComponent<BoardCellTs>().BoardCellConstruct(p1, false, cp);
            b2.GetComponent<BoardCellTs>().BoardCellConstruct(p2, false, cp);
            myCell.GetComponent<BoardCellTs>().BoardCellConstruct(p3, false, cp);

            // Test Piece Movement
            PieceMovementTs pm = new GameObject().AddComponent<PieceMovementTs>();

            Debug.Log("Piece should initially not be king");
            Assert.IsFalse(cp.getIsKing());

            Debug.Log("Testing piece promotion to king");
            cp.promote();
            Assert.IsTrue(cp.getIsKing());

            Debug.Log("Piece position set at [1,1]");
            cp.setPosition(1, 1);
            Assert.AreEqual(cp.getPosition()[0], 1);
            Assert.AreEqual(cp.getPosition()[1], 1);

            Debug.Log("Testing Valid moves should be greater than 0 at Poisition (2,2)");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CheckerPieceTsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
