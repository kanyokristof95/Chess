using Chess.Persistence;

namespace Chess.Computer.AI.Heuristic
{
    class pieceHeuristic : IHeuristic
    {
        public double GetPoint(Table table, Colour playerColour)
        {
            double score = 0;
            for(int row = 1; row <= 8; row++)
            {
                for(char column = 'a'; column <= 'h'; column++)
                {
                    double tmp = 0;

                    if(table[row, column].Colour == playerColour)
                    {
                        switch (table[row, column].Piece)
                        {
                            case Piece.Pawn:
                                score += 1;
                                break;
                            case Piece.Bishop:
                                score += 3;
                                break;
                            case Piece.Knight:
                                score += 3;
                                break;
                            case Piece.Rook:
                                score += 5;
                                break;
                            case Piece.Queen:
                                score += 9;
                                break;
                        }
                    } else
                    {
                        switch (table[row, column].Piece)
                        {
                            case Piece.Pawn:
                                score -= 0.5;
                                break;
                            case Piece.Bishop:
                                score -= 2;
                                break;
                            case Piece.Knight:
                                score -= 2;
                                break;
                            case Piece.Rook:
                                score -= 4;
                                break;
                            case Piece.Queen:
                                score -= 8;
                                break;
                        }
                    }
                    
                    if((row == 4 || row == 5) && (column == 'd' || column == 'e') && table[row, column].Piece != Piece.Empty)
                    {
                        if (table[row, column].Colour == playerColour)
                            score += 0.1;
                        else
                            score -= 0.05;
                    } else
                    {
                        if(row > 2 && row < 7)
                        {
                            if (table[row, column].Colour == playerColour)
                                score += 0.04;
                            else
                                score -= 0.02;
                        }
                    }
                }
            }

            if(table.GameStatus == GameStatus.NotInGame)
            {
                if(table.StepInformation == StepInformation.CheckMate)
                {
                    if(table.CurrentPlayer == playerColour)
                    {
                        score = double.NegativeInfinity;
                    } else
                    {
                        score = double.PositiveInfinity;
                    }
                } else
                {
                    score = double.PositiveInfinity;
                }
            } else
            {
                if (table.StepInformation == StepInformation.Check)
                {
                    if (table.CurrentPlayer == playerColour)
                        score -= 0.5;
                    else
                        score += 0.5;
                }
            }

            return score;
        }
    }
}
