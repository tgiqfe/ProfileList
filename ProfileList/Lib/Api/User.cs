using ProfileList.Lib.Profile;

namespace ProfileList.Lib.Api
{
    public class User
    {
        /// <summary>
        /// ユーザープロファイルを取得
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static UserProfileCollection Profile(UserParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, UserProfileCollectionl.");
                Item.UserProfileCollection = new();
            }
            return Item.UserProfileCollection;
        }

        /// <summary>
        /// ユーザーのログオンセッションを取得
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static UserLogonSessionCollection Session(UserParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, UserLogonSessionCollection.");
                Item.UserLogonSessionCollection = new();
            }
            return Item.UserLogonSessionCollection;
        }

        /// <summary>
        /// ユーザーのログオフ
        /// </summary>
        /// <param name="parameter"></param>
        public static dynamic Logoff(UserParameter parameter = null)
        {
            string username = parameter == null ?
                null :
                string.IsNullOrEmpty(parameter.DomainName) ?
                    parameter.UserName :
                    $"{parameter.DomainName}\\{parameter.UserName}";
            Item.Logger.WriteLine("Logoff target user: " + (username ?? "All"));

            Item.UserLogonSessionCollection = new();
            List<UserLogonSession> targetList = null;
            if (string.IsNullOrEmpty(username))
            {
                //  ユーザー指定無し。全RDPセッションをログオフ
                Item.Logger.WriteLine("Logoff all session.");
                targetList = Item.UserLogonSessionCollection.Sessions.
                    ToList();
                targetList.ForEach(x => x.Disconnect());
            }
            else
            {
                //  ユーザー指定有り。指定ユーザーのRDPセッションをログオフ
                Item.Logger.WriteLine($"Logoff session. [{username}]");
                targetList = Item.UserLogonSessionCollection.Sessions.
                    Where(x => $"{x.UserDomain}\\{x.UserName}" == username || x.UserName == username).
                    ToList();
                targetList.ForEach(x => x.Disconnect());
            }

            return new
            {
                disconnect = targetList.Select(x => $"{x.UserDomain}\\{x.UserName}")
            };
        }

        /// <summary>
        /// RDPセッションの切断
        /// </summary>
        /// <param name="parameter"></param>
        public static dynamic Disconnect(UserParameter parameter = null)
        {
            string username = parameter == null ?
                null :
                string.IsNullOrEmpty(parameter.DomainName) ?
                    parameter.UserName :
                    $"{parameter.DomainName}\\{parameter.UserName}";
            Item.Logger.WriteLine("Disconnect target user: " + (username ?? "All"));

            Item.UserLogonSessionCollection = new();
            List<UserLogonSession> targetList = null;
            if (string.IsNullOrEmpty(username))
            {
                //  ユーザー指定無し。全RDPセッションを切断
                Item.Logger.WriteLine("Disconnect all RDP session.");
                targetList = Item.UserLogonSessionCollection.Sessions.
                    Where(x => x.ProtocolType == 2).
                    ToList();
                targetList.ForEach(x => x.Disconnect());
            }
            else
            {
                //  ユーザー指定有り。指定ユーザーのRDPセッションを切断
                Item.Logger.WriteLine($"Disconnect RDP session. [{username}]");
                targetList = Item.UserLogonSessionCollection.Sessions.
                    Where(x => x.ProtocolType == 2).
                    Where(x => $"{x.UserDomain}\\{x.UserName}" == username || x.UserName == username).
                    ToList();
                targetList.ForEach(x => x.Disconnect());
            }

            return new
            {
                disconnect = targetList.Select(x => $"{x.UserDomain}\\{x.UserName}")
            };
        }
    }
}
