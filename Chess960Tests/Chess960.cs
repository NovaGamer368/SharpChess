using SharpChess.Model;
using SharpChess.Model.AI;
using System.Diagnostics;

namespace Chess960Tests
{
    [TestClass]
    public class Chess960
    {
        [TestMethod]
        public void ChessFENStringTest()
        {
            string fenString = Fen.GenerateChess960FEN();
            Console.WriteLine(fenString);
            Fen.SetBoardPosition(fenString);
            Console.WriteLine("Board: " + Fen.GetBoardPosition());
            Assert.AreNotEqual(fenString, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        [TestMethod]
        public void ChessFENBoardValidiation()
        {
            string fenString = Fen.GenerateChess960FEN();
            Console.WriteLine(fenString);
            Fen.SetBoardPosition(fenString);
            Console.WriteLine("Board Changed: " + Fen.GetBoardPosition());
            Assert.IsTrue(fenString == Fen.GetBoardPosition());
        }


        //---------------------------------------------------------------------------- Moves Tests ----------------------------------------------------------
        [TestMethod]
        public void SortByScoreTest()
        {

            Moves moves = new Moves();
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 0));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 3));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 1));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 3));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 4));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 0));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 6));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 2));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 3));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 8));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 5));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 6));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 7));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 8));
            moves.Add(new Move(0, 0, Move.MoveNames.NullMove, null, null, null, null, 0, 0));

            moves.SortByScore();

            for (int i = 0; i < moves.Count - 1; i++)
            {
                Assert.IsTrue(moves[i].Score >= moves[i + 1].Score);
            }
        }
        //---------------------------------------------------------------------------- Player Tests ----------------------------------------------------------
        [TestMethod]
        public void RecordPossibleKillerMoveTest()
        {
            int Ply = 10;

            KillerMoves.Clear();

            Piece piece = new Piece();
            PiecePawn piecePawn = new PiecePawn(piece);
            piece.Top = piecePawn;

            Move move1 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(0), Board.GetSquare(1), null, 0, 20);
            KillerMoves.RecordPossibleKillerMove(Ply, move1);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move1);
            Assert.IsNull(KillerMoves.RetrieveB(Ply));

            Move move2 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(2), Board.GetSquare(3), null, 0, 10);
            KillerMoves.RecordPossibleKillerMove(Ply, move2);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move1);
            Assert.IsTrue(KillerMoves.RetrieveB(Ply) == move2);

            Move move3 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(4), Board.GetSquare(5), null, 0, 15);
            KillerMoves.RecordPossibleKillerMove(Ply, move3);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move1);
            Assert.IsTrue(KillerMoves.RetrieveB(Ply) == move3);

            Move move4 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(6), Board.GetSquare(7), null, 0, 30);
            KillerMoves.RecordPossibleKillerMove(Ply, move4);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move4);
            Assert.IsTrue(KillerMoves.RetrieveB(Ply) == move1);

            // Start again
            KillerMoves.Clear();

            Move move5 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(16), Board.GetSquare(17), null, 0, 200);
            KillerMoves.RecordPossibleKillerMove(Ply, move5);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move5);
            Assert.IsNull(KillerMoves.RetrieveB(Ply));

            Move move6 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(18), Board.GetSquare(19), null, 0, 300);
            KillerMoves.RecordPossibleKillerMove(Ply, move6);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move6);
            Assert.IsTrue(KillerMoves.RetrieveB(Ply) == move5);
        }

        [TestMethod]
        public void SameKillerMoveWithHigherScoreReplacesSlotEntry()
        {
            const int Ply = 10;

            KillerMoves.Clear();

            Piece piece = new Piece();
            PiecePawn piecePawn = new PiecePawn(piece);
            piece.Top = piecePawn;

            Move move1 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(0), Board.GetSquare(1), null, 0, 20);

            // Add a move
            KillerMoves.RecordPossibleKillerMove(Ply, move1);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move1);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply).Score == 20);
            Assert.IsNull(KillerMoves.RetrieveB(Ply));

            // Add same move AGAIN, but with higher score. Move should be replaced, using higher score.
            Move move2 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(0), Board.GetSquare(1), null, 0, 30);
            KillerMoves.RecordPossibleKillerMove(Ply, move2);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply) == move2);
            Assert.IsTrue(KillerMoves.RetrieveA(Ply).Score == 30);
            Assert.IsNull(KillerMoves.RetrieveB(Ply));

            // Add same move AGAIN, but with LOWER score. No killer moves should be changed
            Move move3 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(0), Board.GetSquare(1), null, 0, 10);
            KillerMoves.RecordPossibleKillerMove(Ply, move3);
            Assert.IsTrue(Move.MovesMatch(KillerMoves.RetrieveA(Ply), move2));
            Assert.IsTrue(KillerMoves.RetrieveA(Ply).Score == 30);
            Assert.IsNull(KillerMoves.RetrieveB(Ply));

            // Now add a different move, and check it goes in slot B
            Move move4 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(2), Board.GetSquare(3), null, 0, 5);
            KillerMoves.RecordPossibleKillerMove(Ply, move4);
            Assert.IsTrue(Move.MovesMatch(KillerMoves.RetrieveA(Ply), move3));
            Assert.IsTrue(KillerMoves.RetrieveA(Ply).Score == 30);
            Assert.IsTrue(Move.MovesMatch(KillerMoves.RetrieveB(Ply), move4));
            Assert.IsTrue(KillerMoves.RetrieveB(Ply).Score == 5);

            // Now improve score of the move that is in slot B. 
            // Slot B's score should be updated. Slot A should stay the same.
            // Slot's A & B should be SWAPPED.
            Move move5 = new Move(0, 0, Move.MoveNames.Standard, piece, Board.GetSquare(2), Board.GetSquare(3), null, 0, 100);
            KillerMoves.RecordPossibleKillerMove(Ply, move5);
            Assert.IsTrue(Move.MovesMatch(KillerMoves.RetrieveA(Ply), move5));
            Assert.IsTrue(KillerMoves.RetrieveA(Ply).Score == 100);
            Assert.IsTrue(Move.MovesMatch(KillerMoves.RetrieveB(Ply), move3));
            Assert.IsTrue(KillerMoves.RetrieveB(Ply).Score == 30);
        }
    }
}