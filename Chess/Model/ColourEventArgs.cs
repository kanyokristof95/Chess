using System;
using Chess.Persistence;

namespace Chess.Model
{
    public class ColourEventArgs : EventArgs
    {
        public Colour Colour { get; private set; }

        public ColourEventArgs(Colour colour)
        {
            Colour = colour;
        }
    }
}
