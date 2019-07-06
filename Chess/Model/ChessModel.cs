using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Persistence;

namespace Chess.Model
{
    public class ChessModel
    {
        #region Fields 

        private Table _table;

        private List<Table> _previousList;

        #endregion


        #region Constants

        public const int capacity = 100;

        #endregion


        #region Properties

        public FieldPosition SelectedField { get { return _table.SelectedField; } }

        #endregion

        
        #region Constructor

        public ChessModel() {}

        #endregion


        #region Public methods

        public void NewGame()
        {
            _table = new Table();
            _previousList = new List<Table>();

            _table[1, 'a'].set(Colour.White, Piece.Rook);
            _table[1, 'h'].set(Colour.White, Piece.Rook);

            _table[1, 'b'].set(Colour.White, Piece.Knight);
            _table[1, 'g'].set(Colour.White, Piece.Knight);

            _table[1, 'c'].set(Colour.White, Piece.Bishop);
            _table[1, 'f'].set(Colour.White, Piece.Bishop);

            _table[1, 'd'].set(Colour.White, Piece.Queen);
            _table[1, 'e'].set(Colour.White, Piece.King);

            for (char column = 'a'; column <= 'h'; column++)
            {
                _table[2, column].set(Colour.White, Piece.Pawn);
            }

            _table[8, 'a'].set(Colour.Black, Piece.Rook);
            _table[8, 'h'].set(Colour.Black, Piece.Rook);

            _table[8, 'b'].set(Colour.Black, Piece.Knight);
            _table[8, 'g'].set(Colour.Black, Piece.Knight);

            _table[8, 'c'].set(Colour.Black, Piece.Bishop);
            _table[8, 'f'].set(Colour.Black, Piece.Bishop);

            _table[8, 'd'].set(Colour.Black, Piece.Queen);
            _table[8, 'e'].set(Colour.Black, Piece.King);

            for (char column = 'a'; column <= 'h'; column++)
            {
                _table[7, column].set(Colour.Black, Piece.Pawn);
            }

            _table.GameStatus = GameStatus.InGame;
            _table.CurrentPlayer = Colour.White;
            _table.StepStatus = StepStatus.WaitingForSelect;
            _table.SelectedField = null;
        }

        public static bool Click(Table table, int row, char column, bool check)
        {
            if (table.GameStatus == GameStatus.NotInGame)
                throw new ChessException("Game Over");

            if (table.StepStatus == StepStatus.WaitingForSelect)
                return SelectField(table, row, column);
            else
                return Step(table, row, column, check);
        }

        public void Click(int row, char column, Colour player)
        {
            if (player != _table.CurrentPlayer)
                throw new ChessException("It isn't your turn!");

            Piece startPiece = Piece.Empty;
            List<FieldPosition> list = new List<FieldPosition>();
        
            int s_row = 0;
            char s_column = '-';

            if (_table.StepStatus == StepStatus.WaitingForStep)
            {
                list.AddRange(ValidSteps());
                list.Add(_table.SelectedField);
                s_row = SelectedField.Row;
                s_column = SelectedField.Column;
                startPiece = _table[s_row, s_column].Piece;
            }

            Table old = new Table(_table);

            if(Click(_table, row, column, true))
            {
                if (_previousList.Count == capacity)
                    _previousList.RemoveAt(0);

                _previousList.Add(old);

                if (row == 1 && player == Colour.White && startPiece == Piece.King)
                {
                    list.Add(new FieldPosition(1, 'a'));
                    list.Add(new FieldPosition(1, 'h'));
                }

                if (row == 8 && player == Colour.Black && startPiece == Piece.King)
                {
                    list.Add(new FieldPosition(8, 'a'));
                    list.Add(new FieldPosition(8, 'h'));
                }

                OnRefreshFields(list);

                if (_table.GameStatus == GameStatus.NotInGame)
                {
                    OnGameOver(GetOtherColour(), _table.StepInformation);
                } else
                {
                    if(_table.StepInformation == StepInformation.Check)
                    {
                        OnCheck();
                    }
                }

                OnNextTurn(_table.CurrentPlayer);
            } else
            {
                list.AddRange(ValidSteps());
                FieldPosition pos = _table.SelectedField;
                if(pos != null)
                    list.Add(_table.SelectedField);

                OnRefreshFields(list);
            }
        }

        public void Unselect()
        {
            _table.StepStatus = StepStatus.WaitingForSelect;
            _table.SelectedField = null;
        }

        public void Undo()
        {
            if (_previousList.Count == 0)
                return;

            Table table = _previousList.Last();
            _previousList.Remove(table);
            _table = table;
            Unselect();
        }

        #endregion


        #region Private methods

        private static bool SelectField(Table table, int row, char column)
        {
            Field field = table[row, column];
            if (field.Colour != table.CurrentPlayer)
                throw new ChessException("The piece isn't yours!");

            table.StepStatus = StepStatus.WaitingForStep;
            table.SelectedField = new FieldPosition(row, column);

            return false;
        }

        private static bool Step(Table table, int row, char column, bool check)
        {
            if (table.SelectedField.Row == row && table.SelectedField.Column == column)
            {
                table.StepStatus = StepStatus.WaitingForSelect;
                table.SelectedField = null;
                return false;
            }

            Field field = table[row, column];
            if (field.Colour == table.CurrentPlayer)
            {
                SelectField(table, row, column);
                return false;
            }

            // Check is valid step
            bool isValid = true;

            if (check)
                isValid = IsValidStep(table, row, column, check);

            if (isValid)
            {
                if (table[row, column].Piece != Piece.Empty)
                {
                    table[row, column].Piece = Piece.Empty;
                    table[row, column].Colour = Colour.Empty;
                    table[row, column].Step = 0;
                }

                table[table.SelectedField.Row, table.SelectedField.Column].Step++;


                // Castling
                if (table[table.SelectedField.Row, table.SelectedField.Column].Piece == Piece.King && table.SelectedField.Row == row &&
                    Math.Abs(table.SelectedField.Column - column) == 2)
                {
                    if (column > table.SelectedField.Column)
                    {
                        table.swap(new FieldPosition(row, (char)(column + 1)), new FieldPosition(row, (char)(column - 1)));
                        table[row, (char)(column + 1)].Step++;
                    }
                    else
                    {
                        table.swap(new FieldPosition(row, (char)(column + 1)), new FieldPosition(row, (char)(column - 2)));
                        table[row, (char)(column - 2)].Step++;
                    }
                }

                table.swap(new FieldPosition(row, column), new FieldPosition(table.SelectedField.Row, table.SelectedField.Column));


                // Transform piece
                if (table.CurrentPlayer == Colour.White && row == 8 && table[row, column].Piece == Piece.Pawn)
                    table[row, column].Piece = Piece.Queen;

                if (table.CurrentPlayer == Colour.Black && row == 1 && table[row, column].Piece == Piece.Pawn)
                    table[row, column].Piece = Piece.Queen;

                NextPlayer(table);
                table.StepStatus = StepStatus.WaitingForSelect;
                table.SelectedField = null;

                table.StepInformation = StepInformation.Normal;

                if (check)
                {
                    if (IsCheck(table)) {
                        if(IsStaleMate(table))
                        {
                            table.GameStatus = GameStatus.NotInGame;
                            table.StepInformation = StepInformation.CheckMate;
                        } else
                        {
                            table.StepInformation = StepInformation.Check;
                        }
                    } else
                    {
                        if (IsStaleMate(table))
                        {
                            table.GameStatus = GameStatus.NotInGame;
                            table.StepInformation = StepInformation.Stalemate;
                        }
                    }
                }

                return true;
            }
            else
            {
                throw new ChessException("Invalid step!");
            }
        }

        private static bool IsCheck(Table table)
        {
            Table newTable = new Table(table);
            NextPlayer(newTable);
            if (IsKingAttackable(newTable))
                return true;

            return false;
        }

        private static bool IsStaleMate(Table table)
        {
            foreach (var item in ValidSelects(table))
            {
                Table newTable = new Table(table);
                Click(newTable, item.Row, item.Column, false);
                var list = ValidSteps(newTable, true);
                if (list.Count > 0)
                    return false;
            }

            return true;
        }


        private static bool IsSafe(Table table, int row, char column)
        {
            Table newTable = new Table(table);
            Step(newTable, row, column, false);
            return !IsKingAttackable(newTable);
        }

        private static bool IsKingAttackable(Table table)
        {
            FieldPosition pos = null;

            for (int row = 1; row <= 8; row++)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    if (table[row, column].Piece == Piece.King && table[row, column].Colour != table.CurrentPlayer)
                        pos = new FieldPosition(row, column);
                }
            }

            foreach (var item in ValidSelects(table))
            {
                Table newTable = new Table(table);
                Click(newTable, item.Row, item.Column, false);
                var list = ValidSteps(newTable, false);
                if (list.Exists(e => e.Row == pos.Row && e.Column == pos.Column))
                    return true;
            }

            return false;
        }

        private static Colour GetOtherColour(Table table)
        {
            if (table.CurrentPlayer == Colour.White)
                return Colour.Black;

            return Colour.White;
        }

        private Colour GetOtherColour()
        {
            return GetOtherColour(_table);
        }

        private static void NextPlayer(Table table)
        {
            if (table.CurrentPlayer == Colour.White)
                table.CurrentPlayer = Colour.Black;
            else
                table.CurrentPlayer = Colour.White;
        }

        #endregion


        #region Check step validity

        private static bool IsValidStep(Table table, int row, char column, bool checkCheck)
        {
            bool ret;

            if (table.StepStatus == StepStatus.WaitingForSelect)
                return false;

            if (table.SelectedField == null)
                return false;

            switch (table[table.SelectedField.Row, table.SelectedField.Column].Piece)
            {
                case Piece.King:
                    ret = IsValidStep_King(table, row, column, checkCheck);
                    break;
                case Piece.Queen:
                    ret = IsValidStep_Queen(table, row, column);
                    break;
                case Piece.Rook:
                    ret = IsValidStep_Rook(table, row, column);
                    break;
                case Piece.Bishop:
                    ret = IsValidStep_Bishop(table, row, column);
                    break;
                case Piece.Knight:
                    ret = IsValidStep_Knight(table, row, column);
                    break;
                case Piece.Pawn:
                    ret = IsValidStep_Pawn(table, row, column);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (checkCheck)
            {
                if (ret)
                {
                    Table newTable = new Table(table);
                    Step(newTable, row, column, false);
                    if (!IsKingAttackable(newTable))
                        return true;
                }
            } else
            {
                return ret;
            }

            return false;
        }

        private static bool IsValidStep_King(Table table, int row, char column, bool checkCheck)
        {
            int s_row = table.SelectedField.Row;
            char s_column = table.SelectedField.Column;

            if (table[row, column].Colour == table.CurrentPlayer)
                return false;

            if(Math.Abs(row-s_row) <= 1 && Math.Abs(column-s_column) <= 1)
            {
                return true;
            }

            if(table[s_row, s_column].Step == 0)
            {
                if(row == 1 && column == 'g' && table[1, 'h'].Piece == Piece.Rook && table[1, 'h'].Colour == Colour.White && table[1, 'h'].Step == 0
                    && table[row, (char)(s_column + 1)].Piece == Piece.Empty && table[row, (char) (s_column + 2)].Piece == Piece.Empty)
                {
                    if(!checkCheck || (IsSafe(table, row, (char)(s_column + 1))))
                        return true;
                }

                if (row == 1 && column == 'c' && table[1, 'a'].Piece == Piece.Rook && table[1, 'a'].Colour == Colour.White && table[1, 'a'].Step == 0
                    && table[row, (char)(s_column - 1)].Piece == Piece.Empty && table[row, (char)(s_column - 2)].Piece == Piece.Empty && table[row, (char)(s_column - 3)].Piece == Piece.Empty)
                {
                    if (!checkCheck || (IsSafe(table, row, (char)(s_column - 1))))
                        return true;
                }

                if (row == 8 && column == 'g' && table[8, 'h'].Piece == Piece.Rook && table[8, 'h'].Colour == Colour.Black && table[8, 'h'].Step == 0
                    && table[row, (char)(s_column + 1)].Piece == Piece.Empty && table[row, (char)(s_column + 2)].Piece == Piece.Empty)
                {
                    if (!checkCheck || (IsSafe(table, row, (char)(s_column + 1))))
                        return true;
                }

                if (row == 8 && column == 'c' && table[8, 'a'].Piece == Piece.Rook && table[8, 'a'].Colour == Colour.Black && table[8, 'a'].Step == 0
                    && table[row, (char)(s_column - 1)].Piece == Piece.Empty && table[row, (char)(s_column - 2)].Piece == Piece.Empty && table[row, (char)(s_column - 3)].Piece == Piece.Empty)
                {
                    if (!checkCheck || (IsSafe(table, row, (char)(s_column - 1))))
                        return true;
                }
            }

            return false;
        }

        private static bool IsValidStep_Queen(Table table, int row, char column)
        {
            int s_row = table.SelectedField.Row;
            char s_column = table.SelectedField.Column;

            if (table[row, column].Colour == table.CurrentPlayer)
                return false;

            if (s_row == row)
            {
                if (column > s_column)
                {
                    char ind = (char)(s_column + 1);

                    while (ind < column && table[s_row, ind].Piece == Piece.Empty)
                    {
                        ind++;
                    }

                    if (ind == column)
                        return true;
                }

                if (column < s_column)
                {
                    char ind = (char)(s_column - 1);

                    while (ind > column && table[s_row, ind].Piece == Piece.Empty)
                    {
                        ind--;
                    }

                    if (ind == column)
                        return true;
                }
            }

            if (s_column == column)
            {
                if (row > s_row)
                {
                    int ind = (s_row + 1);

                    while (ind < row && table[ind, s_column].Piece == Piece.Empty)
                    {
                        ind++;
                    }

                    if (ind == row)
                        return true;
                }

                if (row < s_row)
                {
                    int ind = (s_row - 1);

                    while (ind > row && table[ind, s_column].Piece == Piece.Empty)
                    {
                        ind--;
                    }

                    if (ind == row)
                        return true;
                }
            }

            if (Math.Abs(row - s_row) == Math.Abs(column - s_column))
            {
                if (row > s_row & column > s_column)
                {
                    int row_ind = (s_row + 1);
                    char col_ind = (char)(s_column + 1);

                    while (row_ind < row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind++;
                        col_ind++;
                    }

                    if (row_ind == row)
                        return true;
                }

                if (row > s_row & column < s_column)
                {
                    int row_ind = (s_row + 1);
                    char col_ind = (char)(s_column - 1);

                    while (row_ind < row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind++;
                        col_ind--;
                    }

                    if (row_ind == row)
                        return true;
                }

                if (row < s_row & column > s_column)
                {
                    int row_ind = (s_row - 1);
                    char col_ind = (char)(s_column + 1);

                    while (row_ind > row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind--;
                        col_ind++;
                    }

                    if (row_ind == row)
                        return true;
                }

                if (row < s_row & column < s_column)
                {
                    int row_ind = (s_row - 1);
                    char col_ind = (char)(s_column - 1);

                    while (row_ind > row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind--;
                        col_ind--;
                    }

                    if (row_ind == row)
                        return true;
                }
            }

            return false;
        }

        private static bool IsValidStep_Rook(Table table, int row, char column)
        {
            int s_row = table.SelectedField.Row;
            char s_column = table.SelectedField.Column;

            if (table[row, column].Colour == table.CurrentPlayer)
                return false;

            if(s_row == row)
            {
                if(column > s_column)
                {
                    char ind = (char) (s_column + 1);

                    while(ind < column && table[s_row, ind].Piece == Piece.Empty)
                    {
                        ind++;
                    }

                    if (ind == column)
                        return true;
                }

                if(column < s_column)
                {
                    char ind = (char)(s_column - 1);

                    while (ind > column && table[s_row, ind].Piece == Piece.Empty)
                    {
                        ind--;
                    }

                    if (ind == column)
                        return true;
                }
            }

            if(s_column == column)
            {
                if(row > s_row)
                {
                    int ind = (s_row + 1);

                    while (ind < row && table[ind, s_column].Piece == Piece.Empty)
                    {
                        ind++;
                    }

                    if (ind == row)
                        return true;
                }

                if(row < s_row)
                {
                    int ind = (s_row - 1);

                    while (ind > row && table[ind, s_column].Piece == Piece.Empty)
                    {
                        ind--;
                    }

                    if (ind == row)
                        return true;
                }
            }

            return false;
        }

        private static bool IsValidStep_Bishop(Table table, int row, char column)
        {
            int s_row = table.SelectedField.Row;
            char s_column = table.SelectedField.Column;

            if(Math.Abs(row - s_row) == Math.Abs(column - s_column) && table[row, column].Colour != table.CurrentPlayer)
            {
                if (row > s_row & column > s_column)
                {
                    int row_ind = (s_row + 1);
                    char col_ind = (char) (s_column + 1);

                    while (row_ind < row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind++;
                        col_ind++;
                    }

                    if (row_ind == row)
                        return true;
                }

                if (row > s_row & column < s_column)
                {
                    int row_ind = (s_row + 1);
                    char col_ind = (char)(s_column - 1);

                    while (row_ind < row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind++;
                        col_ind--;
                    }

                    if (row_ind == row)
                        return true;
                }

                if (row < s_row & column > s_column)
                {
                    int row_ind = (s_row - 1);
                    char col_ind = (char)(s_column + 1);

                    while (row_ind > row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind--;
                        col_ind++;
                    }

                    if (row_ind == row)
                        return true;
                }

                if (row < s_row & column < s_column)
                {
                    int row_ind = (s_row - 1);
                    char col_ind = (char)(s_column - 1);

                    while (row_ind > row && table[row_ind, col_ind].Piece == Piece.Empty)
                    {
                        row_ind--;
                        col_ind--;
                    }

                    if (row_ind == row)
                        return true;
                }
            }

            return false;
        }

        private static bool IsValidStep_Knight(Table table, int row, char column)
        {
            int s_row = table.SelectedField.Row;
            char s_column = table.SelectedField.Column;

            if(table[row, column].Colour != table.CurrentPlayer)
            {
                if (Math.Abs(s_row - row) == 2 && Math.Abs(s_column - column) == 1 || Math.Abs(s_row - row) == 1 && Math.Abs(s_column - column) == 2)
                    return true;
            }

            return false;
        }

        private static bool IsValidStep_Pawn(Table table, int row, char column)
        {
            int s_row = table.SelectedField.Row;
            char s_column = table.SelectedField.Column;

            if (table.CurrentPlayer == Colour.White)
            {
                if (s_column == column && s_row + 1 == row && table[s_row + 1, s_column].Piece == Piece.Empty)
                    return true;

                if (s_column == column && s_row + 2 == row && table[s_row + 1, s_column].Piece == Piece.Empty && table[s_row + 2, s_column].Piece == Piece.Empty && table[s_row, s_column].Step == 0)
                    return true;

                if (s_column - 1 == column && s_row + 1 == row && table[s_row + 1, (char)(s_column - 1)].Piece != Piece.Empty && table[s_row + 1, (char)(s_column - 1)].Colour == Colour.Black)
                    return true;

                if (s_column + 1 == column && s_row + 1 == row && table[s_row + 1, (char)(s_column + 1)].Piece != Piece.Empty && table[s_row + 1, (char)(s_column + 1)].Colour == Colour.Black)
                    return true;
            } else
            {
                if (s_column == column && s_row - 1 == row && table[s_row - 1, s_column].Piece == Piece.Empty)
                    return true;

                if (s_column == column && s_row - 2 == row && table[s_row - 1, s_column].Piece == Piece.Empty && table[s_row - 2, s_column].Piece == Piece.Empty && table[s_row, s_column].Step == 0)
                    return true;

                if (s_column - 1 == column && s_row - 1 == row && table[s_row - 1, (char)(s_column - 1)].Piece != Piece.Empty && table[s_row - 1, (char)(s_column - 1)].Colour == Colour.White)
                    return true;

                if (s_column + 1 == column && s_row - 1 == row && table[s_row - 1, (char)(s_column + 1)].Piece != Piece.Empty && table[s_row - 1, (char)(s_column + 1)].Colour == Colour.White)
                    return true;
            }

            return false;
        }

        #endregion

        
        #region Getters

        public static List<FieldPosition> ValidSelects(Table table)
        {
            if (table.StepStatus == StepStatus.WaitingForStep)
                return new List<FieldPosition>(0);

            var list = new List<FieldPosition>();

            for (int row = 1; row <= 8; row++)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    if(table[row, column].Piece != Piece.Empty && table[row, column].Colour == table.CurrentPlayer)
                    {
                        list.Add(new FieldPosition(row, column));
                    }
                }
            }

            return list;
        }

        public List<FieldPosition> ValidSelects()
        {
            var allSelects = ValidSelects(_table);

            List<FieldPosition> selects = new List<FieldPosition>();
            foreach (var item in allSelects)
            {
                Table newTable = new Table(_table);
                ChessModel.Click(newTable, item.Row, item.Column, false);
                var t_steps = ChessModel.ValidSteps(newTable, true);
                if (t_steps.Count > 0)
                    selects.Add(item);
            }

            return selects;
        }


        public static List<FieldPosition> ValidSteps(Table table, bool checkCheck)
        {
            if (table.StepStatus == StepStatus.WaitingForSelect)
                return new List<FieldPosition>(0);

            var list = new List<FieldPosition>();

            for (int row = 1; row <= 8; row++)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    if (IsValidStep(table, row, column, checkCheck))
                    {
                        list.Add(new FieldPosition(row, column));
                    }
                }
            }

            return list;
        }

        public List<FieldPosition> ValidSteps()
        {
            return ValidSteps(_table, true);
        }


        public static Field GetField(Table table, int row, char column)
        {
            return new Field(table[row, column]);
        }

        public Field GetField(int row, char column) {
            return GetField(_table, row, column);
        }


        public static GameStatus GetGameStatus(Table table)
        {
            return table.GameStatus;
        }

        public GameStatus GetGameStatus()
        {
            return GetGameStatus(_table);
        }


        public static Colour GetCurrentPlayer(Table table)
        {
            return table.CurrentPlayer;
        }

        public Colour GetCurrentPlayer()
        {
            return GetCurrentPlayer(_table);
        }


        public static StepInformation GetStepInformation(Table table)
        {
            return table.StepInformation;
        }

        public StepInformation GetStepInformation()
        {
            return GetStepInformation(_table);
        }

        public Table GetTable()
        {
            return new Table(_table);
        }

        #endregion


        #region Event

        public event EventHandler<GameOverEventArgs> GameOver;

        private void OnGameOver(Colour winner, StepInformation reason)
        {
            GameOver?.Invoke(this, new GameOverEventArgs(winner, reason));
        }


        public event EventHandler Check;

        private void OnCheck()
        {
            Check?.Invoke(this, EventArgs.Empty);
        }


        public event EventHandler<FieldsEventArgs> RefreshFields;

        private void OnRefreshFields(List<FieldPosition> list)
        {
            RefreshFields?.Invoke(this, new FieldsEventArgs(list));
        }


        public event EventHandler<ColourEventArgs> NextTurn;

        private void OnNextTurn(Colour colour)
        {
            NextTurn?.Invoke(this, new ColourEventArgs(colour));
        }


        #endregion

    }
}
