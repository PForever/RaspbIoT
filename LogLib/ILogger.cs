namespace LogLib
{
    public interface ILogger
    {
        void Wright(LogLvl lvl, object sender, string message);
        void WrightFormat(LogLvl lvl, object sender, params object[] message);
    }
}