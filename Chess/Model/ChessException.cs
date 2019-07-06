using System;

namespace Chess.Model
{
    public class ChessException : Exception
    {
        public ChessException(string message) : base(message) { }
    }
}
