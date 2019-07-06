using System;
using Chess.Persistence;

namespace Chess.Model
{
    public class GameOverEventArgs : EventArgs
    {
        public Colour Winner { get; private set; }

        public StepInformation Reason { get; private set; }

        public GameOverEventArgs(Colour winner, StepInformation reason)
        {
            Winner = winner;
            Reason = reason;
        }
    }
}
