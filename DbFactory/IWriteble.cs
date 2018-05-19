using DbSingleton.Controller;

namespace DbSingleton
{
    public interface IWriteble
    {
        IDataBaseWriter DbWriter { get; }
    }
}