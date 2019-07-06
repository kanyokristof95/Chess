using System.Windows;
using Chess.View;
using Chess.ViewModel;
using Chess.Model;
using Chess.Computer;

namespace Chess
{
    public partial class App : Application
    {
        #region Fields

        private ChessModel _model;
        private ChessComputer _computer;
        private ChessViewModel _viewModel;
        private MainWindow _view;

        #endregion


        #region Constructor

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        #endregion


        #region Event handlers

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // TODO
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new ChessModel();

            _viewModel = new ChessViewModel(_model);
            _viewModel.Message += _viewModel_Message;

            _computer = new ChessComputer(_model, _viewModel);

            _view = new MainWindow
            {
                DataContext = _viewModel
            };
            _view.Show();
        }

        private void _viewModel_Message(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Text, e.Caption);
        }

        #endregion

    }
}
