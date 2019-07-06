using System;

namespace Chess.ViewModel
{
    public class StartGameEventArgs : EventArgs
    {
        public GameMode GameMode { get; private set; }

        public StartGameEventArgs(GameMode gameMode)
        {
            GameMode = gameMode;
        }
    }
}
