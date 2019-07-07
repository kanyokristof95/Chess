using System;
using Chess.Model;
using Chess.Persistence;

namespace Chess.Computer.AI
{
    public class RandomStepAI : IAI
    {
        private Random _random = new Random();

        public Tuple<FieldPosition, FieldPosition> GoodStep(Table currentTable)
        {
            Colour colour = currentTable.CurrentPlayer;
            var selects = ChessModel.ValidSelectsAndSteps(currentTable);

            FieldPosition select = selects[_random.Next(selects.Count)];
            ChessModel.Click(currentTable, select.Row, select.Column, true);

            var steps = ChessModel.ValidSteps(currentTable, true);
            FieldPosition step = steps[_random.Next(steps.Count)];

            return new Tuple<FieldPosition, FieldPosition>(select, step);
        }
    }
}
