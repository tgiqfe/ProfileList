using ProfileList.Lib.Profile;

namespace ProfileList.Lib.Api
{
    public class User
    {
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
        /// ユーザーのログオン
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static dynamic Logon(UserParameter parameter)
        {
            //  受け取ったパラメータの確認
            if (parameter == null)
            {
                return new
                {
                    Result = "NG",
                    Message = "Parameter is null.",
                };
            }
            string username = parameter.UserName ?? "";
            string password = parameter.Password ?? "";
            string domainname = parameter.DomainName ?? "";
            string tempDomainName = string.IsNullOrEmpty(domainname) ? "." : domainname;
            Item.Logger.WriteLine($"Logon user: {tempDomainName}\\{username}");

            //  ログオン可否確認
            var ret_logon = ConsoleLogon.CheckLogonUser(username, password, domainname);
            if (!ret_logon)
            {
                Item.Logger.WriteLine("Logon check NG.");
                return new
                {
                    Result = "NG",
                    Message = "Logon check NG.",
                };
            }
            Item.Logger.WriteLine("Logon check OK.");

            //  エージェントの稼働確認
            var ret_agent = ConsoleLogon.CheckRunningAgent();
            if (!ret_agent)
            {
                Item.Logger.WriteLine("Remote Logon Agent is stopped.");
                return new
                {
                    Result = "NG",
                    Message = "Remote Logon Agent is stopped.",
                };
            }
            Item.Logger.WriteLine("Remote Logon Agent is running.");

            //  ログオン開始
            var ret_enter = ConsoleLogon.Enter(username, password, domainname);
            if (!ret_enter)
            {
                Item.Logger.WriteLine("Logon failed?, unknown error.");
                return new
                {
                    Result = "NG",
                    Message = "Logon failed?, unknown error.",
                };
            }
            Item.Logger.WriteLine("Logon success.");

            return new
            {
                Result = "OK",
                Message = "Logon success.",
            };
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
                targetList.ForEach(x => x.Logoff());
            }
            else
            {
                //  ユーザー指定有り。指定ユーザーのRDPセッションをログオフ
                Item.Logger.WriteLine($"Logoff session. [{username}]");
                targetList = Item.UserLogonSessionCollection.Sessions.
                    Where(x => $"{x.UserDomain}\\{x.UserName}" == username || x.UserName == username).
                    ToList();
                targetList.ForEach(x => x.Logoff());
            }

            return new
            {
                Logoff = targetList.Select(x => $"{x.UserDomain}\\{x.UserName}")
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
                Disconnect = targetList.Select(x => $"{x.UserDomain}\\{x.UserName}")
            };
        }
    }
}
