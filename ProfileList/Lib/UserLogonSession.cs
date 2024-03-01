using System.Runtime.InteropServices;

namespace ProfileList.Lib
{
    /// <summary>
    /// ユーザーログオン情報を管理
    /// </summary>
    public class UserLogonSession
    {
        #region Static Parameter

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern IntPtr WTSOpenServer(string pServerName);

        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern int WTSEnumerateSessions(
                System.IntPtr hServer,
                int Reserved,
                int Version,
                ref System.IntPtr ppSessionInfo,
                ref int pCount);

        [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
        public static extern void WTSFreeMemory(IntPtr memory);

        [DllImport("Wtsapi32.dll")]
        static extern bool WTSQuerySessionInformation(
            System.IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);


        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);

        [StructLayout(LayoutKind.Sequential)]
        struct WTS_SESSION_INFO
        {
            public Int32 SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        enum WTS_INFO_CLASS
        {
            WTSInitialProgram = 0,
            WTSApplicationName = 1,
            WTSWorkingDirectory = 2,
            WTSOEMId = 3,
            WTSSessionId = 4,
            WTSUserName = 5,
            WTSWinStationName = 6,
            WTSDomainName = 7,
            WTSConnectState = 8,
            WTSClientBuildNumber = 9,
            WTSClientName = 10,
            WTSClientDirectory = 11,
            WTSClientProductId = 12,
            WTSClientHardwareId = 13,
            WTSClientAddress = 14,
            WTSClientDisplay = 15,
            WTSClientProtocolType = 16,
            WTSIdleTime = 17,
            WTSLogonTime = 18,
            WTSIncomingBytes = 19,
            WTSOutgoingBytes = 20,
            WTSIncomingFrames = 21,
            WTSOutgoingFrames = 22,
            WTSClientInfo = 23,
            WTSSessionInfo = 24,
            WTSSessionInfoEx = 25,
            WTSConfigInfo = 26,
            WTSValidationInfo = 27,
            WTSSessionAddressV4 = 28,
            WTSIsRemoteSession = 29
        }

        enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        #endregion
        #region Public Parameter

        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public int SessionID { get; set; }
        public string SessionType { get; set; }
        public string SessionState { get; set; }

        #endregion

        /// <summary>
        /// 現在ログオン中セッションを全取得
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UserLogonSession> GetLoggedOnSession()
        {
            List<UserLogonSession> list = new List<UserLogonSession>();

            IntPtr serverHandle = WTSOpenServer(Environment.MachineName);
            IntPtr buffer = IntPtr.Zero;
            int count = 0;
            int retVal = WTSEnumerateSessions(serverHandle, 0, 1, ref buffer, ref count);
            int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            IntPtr current = buffer;
            uint bytes = 0;

            if (retVal != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += dataSize;

                    IntPtr userNamePtr = IntPtr.Zero;
                    IntPtr domainNamePtr = IntPtr.Zero;
                    IntPtr sessionTypePtr = IntPtr.Zero;

                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSUserName, out userNamePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSDomainName, out domainNamePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSWinStationName, out sessionTypePtr, out bytes);

                    list.Add(new UserLogonSession()
                    {
                        UserName = Marshal.PtrToStringAnsi(userNamePtr),
                        UserDomain = Marshal.PtrToStringAnsi(domainNamePtr),
                        SessionID = si.SessionID,
                        SessionType = Marshal.PtrToStringAnsi(sessionTypePtr),
                        SessionState = si.State.ToString(),
                    });

                    WTSFreeMemory(userNamePtr);
                    WTSFreeMemory(domainNamePtr);
                    WTSFreeMemory(sessionTypePtr);
                }
            }
            WTSFreeMemory(buffer);
            WTSCloseServer(serverHandle);

            return list;
        }

        /// <summary>
        /// RDP接続を切断
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            IntPtr serverHandle = WTSOpenServer(Environment.MachineName);
            bool result = WTSDisconnectSession(serverHandle, SessionID, false);
            WTSCloseServer(serverHandle);
            return result;
        }

        /// <summary>
        /// ログオフ
        /// </summary>
        /// <returns></returns>
        public bool Logoff()
        {
            IntPtr serverHandle = WTSOpenServer(Environment.MachineName);
            bool result = WTSLogoffSession(serverHandle, SessionID, false);
            WTSCloseServer(serverHandle);
            return result;
        }

        /// <summary>
        /// アクティブ状態かどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return SessionState == WTS_CONNECTSTATE_CLASS.WTSActive.ToString();
        }
    }
}
