using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
    public class ChessException : Exception
    {
        public ChessException(string message) : base(message) { }
    }
}
