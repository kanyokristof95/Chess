using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Model;
using Chess.ViewModel;
using Chess.Persistence;

namespace Chess.Computer
{
    public class ChessComputer
    {
        private readonly Random _random = new Random();

        private ChessModel _model;
        private ChessViewModel _viewModel;

        private GameMode _gameMode;
        private Colour _colour;

        public ChessComputer(ChessModel model, ChessViewModel viewModel)
        {
            _model = model;
            _model.NextTurn += _model_NextTurn;

            _viewModel = viewModel;
            _viewModel.StartGame += _viewModel_StartGame;
        }

        private void _viewModel_StartGame(object sender, StartGameEventArgs e)
        {
            _gameMode = e.GameMode;
            _colour = (e.GameMode == GameMode.PlayerVsComputer) ? Colour.Black : Colour.White;

            if (_gameMode == GameMode.ComputerVsPlayer)
                OnNextTurn(_colour);

        }

        private void _model_NextTurn(object sender, ColourEventArgs e)
        {
            OnNextTurn(e.Colour);
        }

        private void OnNextTurn(Colour colour)
        {
            if (_gameMode == GameMode.PlayerVsPlayer)
                return;

            if (_model.GetGameStatus() == GameStatus.NotInGame)
                return;

            if (colour == _colour)
            {
                var selects = _model.ValidSelects();

                FieldPosition select = selects[_random.Next(selects.Count)];
                _model.Click(select.Row, select.Column, _colour);

                var steps = _model.ValidSteps();
                FieldPosition step = steps[_random.Next(steps.Count)];
                _model.Click(step.Row, step.Column, _colour);
            }
        }
    }
}
