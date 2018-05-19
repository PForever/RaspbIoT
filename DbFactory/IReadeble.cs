using DbSingleton.Controller;

namespace DbSingleton
{
    public interface IReadeble
    {
        IDataBaseReader DbReader { get; }
    }
}