using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

//  参考)
//  https://stackoverflow.com/questions/32522545/retrieve-user-logontime-on-terminal-service-with-remote-desktop-services-api
//  [WTS_INFO_CLASS]
//  https://learn.microsoft.com/ja-jp/windows/win32/api/wtsapi32/ne-wtsapi32-wts_info_class
//  [WTSINFOA]
//  https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ns-wtsapi32-wtsinfoa

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WTSINFOA
        {
            const int WINSTATIONNAME_LENGTH = 32;
            const int DOMAIN_LENGTH = 17;
            const int USERNAME_LENGTH = 20;

            private WTS_CONNECTSTATE_CLASS State;
            private int SessionId;
            private int IncomingBytes;
            private int OutgoingBytes;
            private int IncomingFrames;
            private int OutgoingFrames;
            private int IncomingCompressedBytes;
            private int OutgoingCompressedBytes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WINSTATIONNAME_LENGTH)]
            private byte[] WinStationNameRaw;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DOMAIN_LENGTH)]
            private byte[] DomainRaw;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = USERNAME_LENGTH + 1)]
            private byte[] UserNameRaw;
            private long ConnectTimeUTC;
            private long DisconnectTimeUTC;
            private long LastInputTimeUTC;
            private long LogonTimeUTC;
            private long CurrentTimeUTC;

            public string WinStationName { get { return Encoding.ASCII.GetString(WinStationNameRaw); } }
            public string Domain { get { return Encoding.ASCII.GetString(DomainRaw); } }
            public string UserName { get { return Encoding.ASCII.GetString(UserNameRaw); } }
            public DateTime ConnectTime { get { return DateTime.FromFileTimeUtc(ConnectTimeUTC); } }
            public DateTime DisconnectTime { get { return DateTime.FromFileTimeUtc(DisconnectTimeUTC); } }
            public DateTime LastInputTime { get { return DateTime.FromFileTimeUtc(LastInputTimeUTC); } }
            public DateTime LogonTime { get { return DateTime.FromFileTimeUtc(LogonTimeUTC); } }
            public DateTime CurrentTime { get { return DateTime.FromFileTimeUtc(CurrentTimeUTC); } }
        }

        #endregion
        #region Public Parameter

        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public int SessionID { get; set; }
        public string SessionType { get; set; }
        public string SessionState { get; set; }

        public int ProtocolType { get; set; }

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

                    IntPtr protocolTypePtr = IntPtr.Zero;

                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSUserName, out userNamePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSDomainName, out domainNamePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSWinStationName, out sessionTypePtr, out bytes);

                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSClientProtocolType, out protocolTypePtr, out bytes);

                    list.Add(new UserLogonSession()
                    {
                        UserName = Marshal.PtrToStringAnsi(userNamePtr),
                        UserDomain = Marshal.PtrToStringAnsi(domainNamePtr),
                        SessionID = si.SessionID,
                        SessionType = Marshal.PtrToStringAnsi(sessionTypePtr),
                        SessionState = si.State.ToString(),
                        ProtocolType = Marshal.ReadInt32(protocolTypePtr)
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
