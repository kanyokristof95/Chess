namespace Chess.Persistence
{
    public class FieldPosition
    {
        public int Row { get; private set; }

        public char Column { get; private set; }

        public FieldPosition(int row, char column)
        {
            Row = row;
            Column = column;
        }
    }
}
