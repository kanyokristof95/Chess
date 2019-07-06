using System;
using System.Windows;

using Chess.View;
using Chess.ViewModel;
using Chess.Model;

namespace Chess
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ChessModel _model;
        private ChessViewModel _viewModel;
        private MainWindow _view; // View

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // TODO
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Model
            _model = new ChessModel();

            _viewModel = new ChessViewModel(_model);
            _view = new MainWindow
            {
                DataContext = _viewModel
            };
            _view.Show();

            _model.NewGame();

        }
    }
}
