namespace ProfileList.Lib.Profile
{
    public class UserLogonSessionCollection
    {
        public IEnumerable<UserLogonSession> Sessions { get; set; }

        public UserLogonSessionCollection()
        {
            this.Sessions = UserLogonSession.GetLoggedOnSession();
        }
    }
}
