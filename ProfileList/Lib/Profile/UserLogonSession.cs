using System.Runtime.InteropServices;
using System.Text;

//  [WTS_INFO_CLASS]
//  https://learn.microsoft.com/ja-jp/windows/win32/api/wtsapi32/ne-wtsapi32-wts_info_class
//  [WTSINFOA]
//  https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ns-wtsapi32-wtsinfoa

namespace ProfileList.Lib.Profile
{
    /// <summary>
    /// ユーザーログオン情報を管理
    /// </summary>
    public class UserLogonSession
    {
        #region Static Parameter

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern nint WTSOpenServer(string pServerName);

        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(nint hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern int WTSEnumerateSessions(
                nint hServer,
                int Reserved,
                int Version,
                ref nint ppSessionInfo,
                ref int pCount);

        [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
        public static extern void WTSFreeMemory(nint memory);

        [DllImport("Wtsapi32.dll")]
        static extern bool WTSQuerySessionInformation(
            nint hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out nint ppBuffer, out uint pBytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSDisconnectSession(nint hServer, int sessionId, bool bWait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSLogoffSession(nint hServer, int SessionId, bool bWait);

        [StructLayout(LayoutKind.Sequential)]
        struct WTS_SESSION_INFO
        {
            public int SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pWinStationName;
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

        public enum WTS_CONNECTSTATE_CLASS
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

        public struct WTSINFOA
        {
            public const int WINSTATIONNAME_LENGTH = 32;
            public const int DOMAIN_LENGTH = 17;
            public const int USERNAME_LENGTH = 20;
            public WTS_CONNECTSTATE_CLASS State;
            public int SessionId;
            public int IncomingBytes;
            public int OutgoingBytes;
            public int IncomingFrames;
            public int OutgoingFrames;
            public int IncomingCompressedBytes;
            public int OutgoingCompressedBytes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = WINSTATIONNAME_LENGTH)]
            public byte[] WinStationNameRaw;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = DOMAIN_LENGTH)]
            public byte[] DomainRaw;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = USERNAME_LENGTH + 1)]
            public byte[] UserNameRaw;

            public string WinStationName { get { return Encoding.ASCII.GetString(WinStationNameRaw); } }
            public string Domain { get { return Encoding.ASCII.GetString(DomainRaw); } }
            public string UserName { get { return Encoding.ASCII.GetString(UserNameRaw); } }

            public long ConnectTimeUTC;
            public long DisconnectTimeUTC;
            public long LastInputTimeUTC;
            public long LogonTimeUTC;
            public long CurrentTimeUTC;

            public DateTime ConnectTime { get { return DateTime.FromFileTime(ConnectTimeUTC); } }
            public DateTime DisconnectTime { get { return DateTime.FromFileTime(DisconnectTimeUTC); } }
            public DateTime LastInputTime { get { return DateTime.FromFileTime(LastInputTimeUTC); } }
            public DateTime LogonTime { get { return DateTime.FromFileTime(LogonTimeUTC); } }
            public DateTime CurrentTime { get { return DateTime.FromFileTime(CurrentTimeUTC); } }
        }

        #endregion
        #region Public Parameter

        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public int SessionID { get; set; }
        public string SessionType { get; set; }
        public string SessionState { get; set; }
        public int ProtocolType { get; set; }
        public DateTime LogonTime { get; set; }

        #endregion

        /// <summary>
        /// 現在ログオン中セッションを全取得
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UserLogonSession> GetLoggedOnSession()
        {
            List<UserLogonSession> list = new List<UserLogonSession>();

            nint serverHandle = WTSOpenServer(Environment.MachineName);
            nint buffer = nint.Zero;
            int count = 0;
            int retVal = WTSEnumerateSessions(serverHandle, 0, 1, ref buffer, ref count);
            int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            nint current = buffer;
            uint bytes = 0;

            if (retVal != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure(current, typeof(WTS_SESSION_INFO));
                    current += dataSize;

                    nint userNamePtr = nint.Zero;
                    nint domainNamePtr = nint.Zero;
                    nint sessionTypePtr = nint.Zero;
                    nint protocolTypePtr = nint.Zero;
                    nint wtsinfoPtr = nint.Zero;

                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSUserName, out userNamePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSDomainName, out domainNamePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSWinStationName, out sessionTypePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSClientProtocolType, out protocolTypePtr, out bytes);
                    WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSSessionInfo, out wtsinfoPtr, out bytes);

                    var wtsinfo = (WTSINFOA)Marshal.PtrToStructure(wtsinfoPtr, typeof(WTSINFOA));
                    var userName = Marshal.PtrToStringAnsi(userNamePtr);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        list.Add(new UserLogonSession()
                        {
                            UserName = userName,
                            UserDomain = Marshal.PtrToStringAnsi(domainNamePtr),
                            SessionID = si.SessionID,
                            SessionType = Marshal.PtrToStringAnsi(sessionTypePtr),
                            SessionState = si.State.ToString(),
                            ProtocolType = Marshal.ReadInt32(protocolTypePtr),
                            LogonTime = wtsinfo.LogonTime,
                        });
                    }

                    WTSFreeMemory(userNamePtr);
                    WTSFreeMemory(domainNamePtr);
                    WTSFreeMemory(sessionTypePtr);
                    WTSFreeMemory(protocolTypePtr);
                    WTSFreeMemory(wtsinfoPtr);
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
            nint serverHandle = WTSOpenServer(Environment.MachineName);
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
            nint serverHandle = WTSOpenServer(Environment.MachineName);
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
