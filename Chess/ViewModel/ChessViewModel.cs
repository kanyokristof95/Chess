using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Chess.Model;
using Chess.Persistence;

namespace Chess.ViewModel
{
    public class ChessViewModel : ViewModelBase
    {
        private readonly ChessModel _model;

        public ChessViewModel(ChessModel model)
        {
            _model = model;
            _model.GameOver += _model_GameOver;
            _model.Check += _model_Check;
            _model.RefreshFields += _model_RefreshFields;

            Fields = new ObservableCollection<GridField>();

            NewGameCommand = new DelegateCommand(param => { NewGame(); });

            UndoCommand = new DelegateCommand(param => { Undo(); });

            ExitCommand = new DelegateCommand(param => { Exit(); });
        }

        private void NewGame()
        {
            _model.NewGame();

            Fields.Clear();

            for (int row = 8; row >= 1; row--)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    Fields.Add(new GridField
                    {
                        Row = row,
                        Column = column,
                        Colour = _model.GetField(row, column).Colour,
                        Piece = _model.GetField(row, column).Piece,
                        Selected = false,
                        Optional = false,
                        Background = ((row + (int) column) % 2 == 0) ? Persistence.Colour.Black : Persistence.Colour.White,
                        ClickCommand = new DelegateCommand(param => ClickButton(param))
                    }); 
                }
            }

            Status = "White player";
        }

        public void Undo()
        {
            _model.Undo();
            Refresh();
        }

        public void Exit()
        {
            App.Current.Shutdown();
        }

        public void ClickButton(object param)
        {
            var obj = param as Tuple<int, char>;
            int row = obj.Item1;
            char column = obj.Item2;

            try
            {
                _model.Click(row, column, _model.GetCurrentPlayer());
            } catch(ChessException e)
            {
                MessageBox.Show(e.Message); // TODO
            }
        }

        private void Refresh(IEnumerable<FieldPosition> positions)
        {
            var list = _model.ValidSteps();

            foreach (var position in positions)
            {
                int f_row = position.Row;
                char f_column = position.Column;
                int row = 8 - f_row;
                int column = f_column - 'a';

                GridField field = Fields[row * 8 + column];
                field.Colour = _model.GetField(f_row, f_column).Colour;
                field.Piece = _model.GetField(f_row, f_column).Piece;
                field.Selected = (_model.SelectedField != null && _model.SelectedField.Row == f_row && _model.SelectedField.Column == f_column);
                field.Optional = list.Exists(f => f.Row == f_row && f.Column == f_column);
            }

            Status = (_model.GetCurrentPlayer() == Colour.White) ? "White player" : "Black player";
        }

        private void Refresh()
        {
            Refresh(Fields.Select(e => new FieldPosition(e.Row, e.Column)));
        }

        public ObservableCollection<GridField> Fields { get; }

        public DelegateCommand NewGameCommand { get; }

        public DelegateCommand UndoCommand { get; }

        public DelegateCommand ExitCommand { get; }

        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }



        private void _model_GameOver(object sender, GameOverEventArgs e)
        {
            Status = "Game Over";
            //Refresh(); // TODO - delete

            if (e.Reason == StepInformation.CheckMate)
                MessageBox.Show("Check mate, winner is " + e.Winner.ToString() + " playere", "Game over");
            else
                MessageBox.Show("Stalemate", "Game Over");
        }

        private void _model_Check(object sender, EventArgs e)
        {
            Status += " - Chess";
            MessageBox.Show("Check", "Information");
        }

        private void _model_RefreshFields(object sender, FieldsEventArgs e)
        {
            Refresh(e.Fields);
        }
    }
}
