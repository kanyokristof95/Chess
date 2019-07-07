using System.Collections.Generic;
using Chess.Persistence;

namespace Chess.Computer.AI
{
    public enum MiniMaxLevel
    {
        Max,
        Min
    }

    public class MiniMaxNode
    {
        public FieldPosition SelectPosition { get; set; }

        public FieldPosition StepPosition { get; set; }

        public MiniMaxLevel Level { get; set; }

        public double Value { get; set; }

        public Table Table { get; set; }

        public List<MiniMaxNode> Children { get; set; }

        public MiniMaxNode Parent { get; set; }

        public MiniMaxNode()
        {
            Children = new List<MiniMaxNode>();
        }
    }
}
