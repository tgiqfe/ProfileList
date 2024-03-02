using System.Management;

namespace ProfileList.Lib
{
    /*
    public class MachineInfo2
    {
        #region Private parameters

        private static string _domainName = null;
        private static bool? _isDomainMachine = false;
        private static string[] _systemSIDs = null;
        private static IEnumerable<UserLogonSession> _sessions = null;

        #endregion

        #region Public properties

        public static string ComputerName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public static string DomainName
        {
            get
            {
                _domainName ??= (new ManagementClass("Win32_ComputerSystem").
                    GetInstances().
                    OfType<ManagementObject>().
                    First())["Domain"] as string;
                return _domainName;
            }
        }

        public static bool IsDomainMachine
        {
            get
            {
                _isDomainMachine ??= (bool)(new ManagementClass("Win32_ComputerSystem").
                    GetInstances().
                    OfType<ManagementObject>().
                    First())["PartOfDomain"];
                return _isDomainMachine ?? false;
            }
        }

        public static string[] SystemSIDs
        {
            get
            {
                _systemSIDs ??= new ManagementClass("Win32_SystemAccount").
                    GetInstances().
                    OfType<ManagementObject>().
                    Select(x => x["SID"] as string).
                    ToArray();
                return _systemSIDs;
            }
        }

        public static IEnumerable<UserLogonSession> UserLogonSessions
        {
            get
            {
                _sessions ??= UserLogonSession.GetLoggedOnSession();
                return _sessions;
            }
        }

        #endregion
    }
    */
}
