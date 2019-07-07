using System;
using Chess.Persistence;

namespace Chess.Computer.AI
{
    public interface IAI
    {
        Tuple<FieldPosition, FieldPosition> GoodStep(Table currentTable);
    }
}
