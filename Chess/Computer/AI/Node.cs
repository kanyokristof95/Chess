using System.Collections.Generic;
using Chess.Persistence;

namespace Chess.Computer.AI
{
    public enum MiniMaxLevel
    {
        Max,
        Min
    }

    public class Node
    {
        public FieldPosition SelectPosition { get; set; }

        public FieldPosition StepPosition { get; set; }

        public MiniMaxLevel Level { get; set; }

        public double Value { get; set; }

        public Table Table { get; set; }

        public int Pos { get; set; }

        public List<Node> Children { get; set; }

        public Node Parent { get; set; }

        public Node()
        {
            Children = new List<Node>();
        }
    }
}
