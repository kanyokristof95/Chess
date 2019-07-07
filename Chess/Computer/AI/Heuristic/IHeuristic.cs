using Chess.Persistence;

namespace Chess.Computer.AI.Heuristic
{
    public interface IHeuristic
    {
        double GetPoint(Table table, Colour player);
    }
}
