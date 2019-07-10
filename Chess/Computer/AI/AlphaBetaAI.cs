using System;
using Chess.Computer.AI.Heuristic;
using Chess.Persistence;
using Chess.Model;
using System.Collections.Generic;

namespace Chess.Computer.AI
{
    class AlphaBetaAI : IAI
    {
        private readonly IHeuristic heuristic = new pieceHeuristic();
        private const int level = 3;
        private Colour colour;
        private Random rand = new Random();

        public Tuple<FieldPosition, FieldPosition> GoodStep(Table currentTable)
        {
            colour = currentTable.CurrentPlayer;

            Node root = new Node();
            root.Level = MiniMaxLevel.Max;
            root.Value = Double.NegativeInfinity;
            root.Table = new Table(currentTable);
            root.Pos = 0;
            
            Node act = root;

            calc(root);

            List<Node> list = new List<Node>();
            double max = Double.NegativeInfinity;

            foreach (var node in root.Children)
            {
                if (node.Value == max)
                    list.Add(node);

                if (node.Value > max)
                {
                    list = new List<Node>();
                    list.Add(node);
                    max = node.Value;
                }
            }

            Node selected = list[rand.Next(list.Count)];

            return new Tuple<FieldPosition, FieldPosition>(selected.SelectPosition, selected.StepPosition);
        }

        private void calc(Node node)
        {
            if(node.Pos == level || node.Table.GameStatus == GameStatus.NotInGame)
            {
                node.Value = heuristic.GetPoint(node.Table, colour);
            } else
            {
                var selects = ChessModel.ValidSelects(node.Table);
                foreach (var select in selects)
                {
                    Table table_select = new Table(node.Table);
                    ChessModel.Click(table_select, select.Row, select.Column, true);
                    var steps = ChessModel.ValidSteps(table_select, true);

                    foreach (var step in steps)
                    {
                        Table table_step = new Table(table_select);
                        ChessModel.Click(table_step, step.Row, step.Column, true);

                        Node actChild = new Node();
                        actChild.SelectPosition = new FieldPosition(select.Row, select.Column);
                        actChild.StepPosition = new FieldPosition(step.Row, step.Column);
                        actChild.Parent = node;
                        actChild.Table = new Table(table_step);
                        actChild.Pos = node.Pos + 1;
                        actChild.Level = (node.Level == MiniMaxLevel.Min) ? MiniMaxLevel.Max : MiniMaxLevel.Min;
                        actChild.Value = (actChild.Level == MiniMaxLevel.Max) ? Double.NegativeInfinity : Double.PositiveInfinity;

                        node.Children.Add(actChild);
                        calc(actChild);
                        
                        if(node.Level == MiniMaxLevel.Max)
                        {
                            if (actChild.Value <= node.Value)
                                continue;

                            node.Value = actChild.Value;
                        } else
                        {
                            if (actChild.Value >= node.Value)
                                continue;


                            node.Value = actChild.Value;
                        }
                    }
                }

            }
        }
    }
}
