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
            // Use the Assert class to test conditions
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
