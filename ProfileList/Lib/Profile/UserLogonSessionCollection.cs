namespace ProfileList.Lib.Profile
{
    public class UserLogonSessionCollection
    {
        public IEnumerable<UserLogonSession> Sessions { get; set; }

        public UserLogonSessionCollection()
        {
            this.Sessions = UserLogonSession.GetLoggedOnSession();
        }

        /*
        /// <summary>
        /// RDPセッションの切断
        /// </summary>
        public dynamic Disconnect(string username = null)
        {
            this.Sessions = UserLogonSession.GetLoggedOnSession();
            if (username == null)
            {
                //  ユーザー指定無し。全RDPセッションを切断
                var list = this.Sessions.
                    Where(x => x.ProtocolType == 2).
                    ToList();
                list.ForEach(x => x.Disconnect());
                return new
                {
                    disconnect = list.Select(x => $"{x.UserDomain}\\{x.UserName}")
                };
            }
            else
            {
                //  ユーザー指定有り。指定ユーザーのRDPセッションを切断
                var list = this.Sessions.
                    Where(x => x.ProtocolType == 2).
                    Where(x => $"{x.UserDomain}\\{x.UserName}" == username || x.UserName == username).
                    ToList();
                list.ForEach(x => x.Disconnect());
                return new
                {
                    disconnect = list.Select(x => $"{x.UserDomain}\\{x.UserName}")
                };
            }
        }
        */
    }
}
