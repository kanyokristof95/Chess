namespace Chess.Persistence
{
    public class Table
    {
        #region Fields

        private Field[,] _table;

        #endregion


        #region Constants

        private const int size = 8;

        #endregion


        #region Properties 

        public GameStatus GameStatus { get; set; }

        public StepInformation StepInformation { get; set; }

        public Colour CurrentPlayer { get; set; }

        public StepStatus StepStatus { get; set; }

        public FieldPosition SelectedField { get; set; }

        public FieldPosition LastStep { get; set; }

        #endregion


        #region Constructors

        public Table()
        {
            _table = new Field[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _table[i, j] = new Field(); // Empty
                }
            }
        }

        public Table(Table table)
        {
            GameStatus = table.GameStatus;
            StepInformation = table.StepInformation;
            CurrentPlayer = table.CurrentPlayer;
            StepStatus = table.StepStatus;
            if(table.SelectedField != null)
                SelectedField = new FieldPosition(table.SelectedField.Row, table.SelectedField.Column);

            LastStep = table.LastStep;

            _table = new Field[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _table[i, j] = new Field()
                    {
                        Colour = table._table[i, j].Colour,
                        Piece = table._table[i, j].Piece,
                        Step = table._table[i, j].Step
                    };
                }
            }
        }

        #endregion


        #region Public methods

        public int convert(int row)
        {
            return 8 - row;
        }

        public int convert(char column)
        {
            return (int)column - (int)'a';
        }

        public Field this[int row, char column]
        {
            get
            {
                return _table[convert(row), convert(column)];
            }

            set
            {
                _table[convert(row), convert(column)] = value;
            }
        }

        public void swap(FieldPosition a, FieldPosition b)
        {
            int a_row = convert(a.Row);
            int a_column = convert(a.Column);

            int b_row = convert(b.Row);
            int b_column = convert(b.Column);

            swap(ref _table[a_row, a_column], ref _table[b_row, b_column]);
        }

        private void swap(ref Field a, ref Field b)
        {
            Field tmp = a;
            a = b;
            b = tmp;
        }

        #endregion

    }
}