using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chess.Model;
using Chess.Persistence;

namespace Chess.ViewModel
{
    public class ChessViewModel : ViewModelBase
    {
        #region Fields

        private readonly ChessModel _model;

        private GameMode _gameMode;

        #endregion


        #region Constructor

        public ChessViewModel(ChessModel model)
        {
            _model = model;
            _model.GameOver += _model_GameOver;
            _model.Check += _model_Check;
            _model.RefreshFields += _model_RefreshFields;

            Fields = new ObservableCollection<GridField>();

            NewGameCommand = new DelegateCommand(param => { NewGame((GameMode) param); });
            UndoCommand = new DelegateCommand(param => { Undo(); });
            ExitCommand = new DelegateCommand(param => { Exit(); });
        }

        #endregion


        #region Private methods 

        private void NewGame(GameMode gameMode)
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

            _gameMode = gameMode;
            if (gameMode != GameMode.PlayerVsPlayer)
            {
                OnStartGame(_gameMode);
            }
        }

        private void Undo()
        {
            if (_gameMode != GameMode.PlayerVsPlayer)
                return;

            _model.Undo();
            Refresh();
        }

        private void Exit()
        {
            App.Current.Shutdown();
        }

        private void ClickButton(object param)
        {
            var obj = param as Tuple<int, char>;
            int row = obj.Item1;
            char column = obj.Item2;

            try
            {
                Colour colour = Colour.Empty;
                switch (_gameMode)
                {
                    case GameMode.PlayerVsPlayer:
                        colour = _model.GetCurrentPlayer();
                        break;
                    case GameMode.PlayerVsComputer:
                        colour = Colour.White;
                        break;
                    case GameMode.ComputerVsPlayer:
                        colour = Colour.Black;
                        break;
                }
                _model.Click(row, column, colour);
            }
            catch (ChessException e)
            {
                OnMessage(e.Message, "Warning");
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

        #endregion


        #region Properties 

        public ObservableCollection<GridField> Fields { get; }

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

        #endregion


        #region Commands

        public DelegateCommand NewGameCommand { get; }

        public DelegateCommand UndoCommand { get; }

        public DelegateCommand ExitCommand { get; }

        #endregion


        #region Event handlers

        private void _model_GameOver(object sender, GameOverEventArgs e)
        {
            Status = "Game Over";
            
            if (e.Reason == StepInformation.CheckMate)
                OnMessage("Check mate! The winner is the " + e.Winner.ToString() + " player!", "Game over");
            else
                OnMessage("Stalemate!", "Game Over");
        }

        private void _model_Check(object sender, EventArgs e)
        {
            Status += " - Check";
            OnMessage("Check", "Warning");
        }

        private void _model_RefreshFields(object sender, FieldsEventArgs e)
        {
            Refresh(e.Fields);
        }

        #endregion


        #region Events

        public event EventHandler<MessageEventArgs> Message;

        private void OnMessage(string text, string caption)
        {
            Message?.Invoke(this, new MessageEventArgs(text, caption));
        }

        public event EventHandler<StartGameEventArgs> StartGame;

        private void OnStartGame(GameMode mode)
        {
            StartGame?.Invoke(this, new StartGameEventArgs(mode));
        }

        #endregion

    }
}
