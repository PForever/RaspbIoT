using DbSingleton.Controller;

namespace DbSingleton
{
    public static class DbControlling
    {
        private static ADataBaseController _controller;
        public static IDataBaseReader DbReader => _controller;
        public static IDataBaseWriter DbWriter => _controller;
        public static bool IsEmpty { get; private set; }

        public static void Initialize(ADataBaseController controller)
        {
            if (!IsEmpty) return;
            _controller = controller;
            IsEmpty = false;
        }
        static DbControlling()
        {
            IsEmpty = true;
            _controller = new EmptyDbController();
        }
    }
}