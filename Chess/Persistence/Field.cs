using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Persistence;

namespace Chess.Persistence
{
    public class Field
    {   
        public Field() {
            Colour = Colour.Empty;
            Piece = Piece.Empty;
        }

        public Field(Field field)
        {
            Colour = field.Colour;
            Piece = field.Piece;
        }

        public Colour Colour { get; set; }

        public Piece Piece { get; set; }

        public int Step { get; set; }

        public void set(Colour colour, Piece piece)
        {
            Colour = colour;
            Piece = piece;
            Step = 0;
        }
    }
}
