using Microsoft.Win32.SafeHandles;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace ProfileList.Lib
{
    public class ConsoleLogon
    {
        const int PIP_CONNECT_TIMEOUT = 3000;

        #region CheckLogonUser

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        private static bool GetImpersonationContext(string userName, string domainName, string password)
        {
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;
            bool resultBool = false;
            try
            {
                SafeTokenHandle safeTokenHandle;
                bool returnValue = LogonUser(userName, domainName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out safeTokenHandle);
                if (returnValue) { resultBool = true; }
            }
            catch (Exception) { throw; }
            return resultBool;
        }

        private sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle() : base(true) { }

            [DllImport("kernel32.dll")]
            //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }

        /// <summary>
        /// 対象ユーザーがログオン可能かどうかをチェック
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static bool CheckLogonUser(string userName, string password, string domainName = "")
        {
            try
            {
                return GetImpersonationContext(userName, domainName, password);
            }
            catch { }
            return false;
        }

        #endregion

        /// <summary>
        /// グローバルMutexを使用して、エージェントが実行中かどうかをチェック
        /// </summary>
        /// <returns>true⇒エージェントが実行中</returns>
        public static bool CheckRunningAgent()
        {
            using (var mutex = new Mutex(false, Item.Setting.RLAgentMutexKey, out bool createNew))
            {
                return !createNew;
            }
        }

        /// <summary>
        /// ログオン開始
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static bool Enter(string userName, string password, string domainName = "")
        {
            using (var pipe = new NamedPipeClientStream(
                ".",
                Item.Setting.RLAgentPipeKey,
                PipeDirection.Out,
                PipeOptions.None,
                TokenImpersonationLevel.None,
                HandleInheritability.None))
            {
                try
                {
                    pipe.Connect(PIP_CONNECT_TIMEOUT);
                    using (var writer = new StreamWriter(pipe))
                    {
                        writer.WriteLine($"{userName}\n{password}\n{domainName}");
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
