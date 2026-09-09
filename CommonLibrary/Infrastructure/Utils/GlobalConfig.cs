
namespace CommonLibrary.Infrastructure.Utils
{
    public class GlobalConfig
    {
        static private GlobalConfig obj = null;
        private static readonly Lazy<GlobalConfig> _instance =
        new Lazy<GlobalConfig>(() => new GlobalConfig());
        private static readonly object _lock = new object();
        private GlobalConfig()
        {

        }
        public string ConnectionString { get; set; }
        public string LoanMonitoringReportingConnectionString { get; set; }
        public string UserManagementConnectionString { get; set; }
        public string UserManagementApi { get; set; }
        //static public GlobalConfig Instance

        //{
        //    get
        //    {
        //        if (obj == null)
        //        {
        //            lock (_lock)
        //            {
        //                if (obj == null)
        //                    return new GlobalConfig();
        //            }
        //        }
        //        if (obj == null)
        //            obj = new GlobalConfig();
        //        return obj;
        //    }
        //}

        public static GlobalConfig Instance => _instance.Value;
    }
}
