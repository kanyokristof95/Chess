namespace Chess.Persistence
{
    public interface IPersistence
    {
        Table load(string path);

        void save(Table table, string path);
    }
}
