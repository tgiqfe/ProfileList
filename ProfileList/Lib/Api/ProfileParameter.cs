using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class ProfileParameter : ApiParameter
    {
        /// <summary>
        /// プロファイルに関する処理の前に、プロファイル情報を更新する。
        /// </summary>
        public bool? Refresh { get; set; }

        /// <summary>
        /// 対象のユーザー名。プロファイル削除時に指定する。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// プロファイル削除時に、全ユーザーのプロファイルを削除する。
        /// </summary>
        public bool? All { get; set; }
    }
}
