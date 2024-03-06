namespace ProfileList.Lib.Profile
{
    public class UserLogonSessionCollection
    {
        public IEnumerable<UserLogonSession> Sessions { get; set; }

        public UserLogonSessionCollection()
        {
            this.Sessions = UserLogonSession.GetLoggedOnSession();
        }

        #region Summary

        /// <summary>
        /// セッション数
        /// </summary>
        /// <returns></returns>
        public int GetSummary_SessionCount()
        {
            return this.Sessions.Count();
        }

        /// <summary>
        /// ログオン中のユーザー
        /// プロトコルタイプ: 0⇒コンソール, 2⇒RDP, 1⇒Unknown
        /// </summary>
        /// <returns></returns>
        public string GetSummary_Users()
        {
            return string.Join(", ", this.Sessions.Select(x => $"{x.UserName}({x.ProtocolType})"));
        }

        #endregion
    }
}
