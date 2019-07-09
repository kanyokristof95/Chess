using System;
using Chess.Model;
using Chess.Persistence;
using Chess.Computer.AI.Heuristic;
using System.Collections.Generic;

namespace Chess.Computer.AI
{
    public class MiniMaxAI : IAI
    {
        private readonly IHeuristic heuristic = new pieceHeuristic();
        private const int level = 3;

        public Tuple<FieldPosition, FieldPosition> GoodStep(Table currentTable)
        {
            Colour colour = currentTable.CurrentPlayer;

            MiniMaxNode root = new MiniMaxNode();
            root.Table = new Table(currentTable);
            root.Level = MiniMaxLevel.Max;
            root.Parent = null;
            root.Value = Double.NegativeInfinity;

            List<MiniMaxNode> leaves = new List<MiniMaxNode>();
            List<MiniMaxNode> extraLeaves = new List<MiniMaxNode>();
            leaves.Add(root);

            // Generating tree
            for(int i = 0; i < level; i++)
            {
                List<MiniMaxNode> newLeaves = new List<MiniMaxNode>();

                foreach(var leaf in leaves)
                {
                    if(leaf.Table.GameStatus == GameStatus.NotInGame)
                    {
                        if(i != level-1)
                        {
                            extraLeaves.Add(leaf);
                        }
                        continue;
                    }

                    var selects = ChessModel.ValidSelectsAndSteps(leaf.Table);
                    foreach (var select in selects)
                    {
                        Table table_select = new Table(leaf.Table);
                        ChessModel.Click(table_select, select.Row, select.Column, true);
                        var steps = ChessModel.ValidSteps(table_select, true);

                        foreach (var step in steps)
                        {
                            Table table_step = new Table(table_select);
                            ChessModel.Click(table_step, step.Row, step.Column, true);

                            MiniMaxNode node = new MiniMaxNode();
                            node.SelectPosition = new FieldPosition(select.Row, select.Column);
                            node.StepPosition = new FieldPosition(step.Row, step.Column);
                            node.Parent = leaf;
                            node.Table = new Table(table_step);
                            node.Level = (leaf.Level == MiniMaxLevel.Min) ? MiniMaxLevel.Max : MiniMaxLevel.Min;
                            node.Value = (node.Level == MiniMaxLevel.Max) ? Double.NegativeInfinity : Double.PositiveInfinity;

                            leaf.Children.Add(node);

                            newLeaves.Add(node);
                        }
                    }
                }

                leaves = newLeaves;
            }

            // Leaves
            leaves.AddRange(extraLeaves);

            foreach (var leaf in leaves)
            {
                leaf.Value = heuristic.GetPoint(leaf.Table, colour);
            }

            // Evaluating tree

            

            for (int i = 0; i < level; i++)
            {
                List<MiniMaxNode> newLeaves = new List<MiniMaxNode>();

                foreach (var leaf in leaves)
                {
                    if (leaf.Parent == null)
                        break;

                    if (!newLeaves.Contains(leaf.Parent))
                        newLeaves.Add(leaf.Parent);

                    if (leaf.Parent.Level == MiniMaxLevel.Max)
                    {
                        if (leaf.Value > leaf.Parent.Value)
                        {
                            leaf.Parent.Value = leaf.Value;
                        }
                    }
                    else
                    {
                        if (leaf.Value < leaf.Parent.Value)
                        {
                            leaf.Parent.Value = leaf.Value;
                        }
                    }
                }

                leaves = newLeaves;
            }

            double max = Double.NegativeInfinity;
            FieldPosition ret_select = null;
            FieldPosition ret_step = null;

            foreach (var node in root.Children)
            {
                if(node.Value >= max)
                {
                    max = node.Value;
                    ret_select = node.SelectPosition;
                    ret_step = node.StepPosition;
                }
            }

            return new Tuple<FieldPosition, FieldPosition>(ret_select, ret_step);
        }
    }
}
