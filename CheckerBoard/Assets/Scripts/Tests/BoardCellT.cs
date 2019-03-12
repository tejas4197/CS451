using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BoardCellTs : MonoBehaviour
    {
        public int[] position = new int[2];  //position x, y
        public Color orgColor = new Color(); //original non yellow color of board cell
        private bool isOccupied;
        private CheckerPieceTs piece;   //will be null without piece, or have a piece if occupied 

        // constructor
        // every board cell should recieve a position 
        // Some boardcells will have non null pieces; isOccupied will be true and p will be a piece
        // otherwise, empty cells will be false and p will be null     
        public void BoardCellConstruct(int[] pos, bool Occupied, CheckerPieceTs cp)
        {
            position[0] = pos[0];
            position[1] = pos[1];
            isOccupied = Occupied;
            CheckerPieceTs piece = cp;
        }
        
        #region "Get Methods"

        public int[] getPosition() { return position; }

        public bool checkIfOccupied() { return isOccupied; }

        public CheckerPieceTs getPiece() { return piece; }

        #endregion

        #region "Set Methods"

        //code to make the space unoccupied
        public void clearSpace()
        {
            this.isOccupied = false;
            this.piece = null;
        }

        //code to set the peice on board cell       
        public void setPiece(CheckerPieceTs cp)
        {
            this.isOccupied = true;
            this.piece = cp;
        }
        #endregion
    }

    public class BoardCellT
    {
        // A Test behaves as an ordinary method
        [Test]
        public void BoardCellTSimplePasses()
        {
            // Use the Assert class to test conditions
            BoardCellTs Bc = new GameObject().AddComponent<BoardCellTs>();
            Bc.clearSpace();
            Assert.IsFalse(Bc.checkIfOccupied());
            //Assert.IsTrue(Bc.checkIfOccupied());
            //Assert.AreEqual(Bc.checkIfOccupied());
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator BoardCellTWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
