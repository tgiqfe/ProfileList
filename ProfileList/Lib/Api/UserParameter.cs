using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class UserParameter : ApiParameter
    {
        /// <summary>
        /// 対象のユーザー名。ログオフや切断、ログオン時に使用する。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 対象ユーザーのパスワード。ログオン時に使用する。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 対象ユーザーのドメイン名。ログオフや切断、ログオン時に使用する。
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// ユーザー情報を取得時、ユーザー情報を更新する。
        /// </summary>
        public bool? Refresh { get; set; }
    }
}
