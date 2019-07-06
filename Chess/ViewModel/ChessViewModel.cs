using System;
using System.Collections.ObjectModel;
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

            Fields = new ObservableCollection<GridField>();

            NewGameCommand = new DelegateCommand(param => { NewGame(); });

            UndoCommand = new DelegateCommand(param => { Undo(); });
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
            Refresh();
        }

        private void Refresh()
        {
            var list = _model.ValidSteps();

            foreach(var field in Fields)
            {
                int row = field.Row;
                char column = field.Column;
                field.Colour = _model.GetField(row, column).Colour;
                field.Piece = _model.GetField(row, column).Piece;
                field.Selected = (_model.SelectedField != null && _model.SelectedField.Row == row && _model.SelectedField.Column == column);
                field.Optional = list.Exists(f => f.Row == row && f.Column == column);
            }

            if(_model.GetGameStatus() == GameStatus.NotInGame)
            {
                Status = "Game Over - ";

                if (_model.GetStepInformation() == StepInformation.CheckMate)
                {
                    Status += "Check Mate - Winner: ";
                    Status += (_model.GetCurrentPlayer() == Colour.White) ? "Black player" : "White player";
                }
                else if (_model.GetStepInformation() == StepInformation.Stalemate)
                {
                    Status += "Stalemate";
                }
                
            } else
            {
                Status = (_model.GetCurrentPlayer() == Colour.White) ? "White player" : "Black player";

                if (_model.GetStepInformation() == StepInformation.Check)
                {
                    Status += " - Check";
                }
            }
        }

        public ObservableCollection<GridField> Fields { get; }

        public DelegateCommand NewGameCommand { get; }

        public DelegateCommand UndoCommand { get; }

        private String _status;

        public String Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }
    }
}
