using Chess.Model;
using Chess.ViewModel;
using Chess.Persistence;
using Chess.Computer.AI;
using System.Threading.Tasks;

namespace Chess.Computer
{
    public class ChessComputer
    {
        private readonly IAI aI;

        private ChessModel _model;
        private ChessViewModel _viewModel;

        private GameMode _gameMode;
        private Colour _colour;

        public ChessComputer(ChessModel model, ChessViewModel viewModel)
        {
            aI = new AlphaBetaAI();

            _model = model;
            _model.NextTurn += _model_NextTurn;

            _viewModel = viewModel;
            _viewModel.StartGame += _viewModel_StartGame;
        }

        private void _viewModel_StartGame(object sender, StartGameEventArgs e)
        {
            _gameMode = e.GameMode;
            switch (e.GameMode)
            {
                case GameMode.PlayerVsPlayer:
                    _colour = Colour.Empty;
                    break;
                case GameMode.PlayerVsComputer:
                    _colour = Colour.Black;
                    break;
                case GameMode.ComputerVsPlayer:
                    _colour = Colour.White;
                    break;
            }

            if (_gameMode == GameMode.ComputerVsPlayer)
                OnNextTurn(_colour);
        }

        private void _model_NextTurn(object sender, ColourEventArgs e)
        {
            OnNextTurn(e.Colour);
        }

        private async void OnNextTurn(Colour colour)
        {
            await Task.Run(() =>
            {
                if (_gameMode == GameMode.PlayerVsPlayer)
                    return;

                if (_model.GetGameStatus() == GameStatus.NotInGame)
                    return;

                if (colour == _colour)
                {
                    var tuple = aI.GoodStep(_model.GetTable());
                    var select = tuple.Item1;
                    var step = tuple.Item2;

                    _model.Click(select.Row, select.Column, _colour);
                    _model.Click(step.Row, step.Column, _colour);
                }
            });
        }
    }
}
