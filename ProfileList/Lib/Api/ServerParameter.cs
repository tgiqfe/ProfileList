using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class ServerParameter : ApiParameter
    {
        /// <summary>
        /// サーバ情報取得時に、サーバ情報を更新する。
        /// </summary>
        public bool? Refresh { get; set; }

        /// <summary>
        /// ネットワーク情報取得時に、全ネットワーク情報を取得する。
        /// 未指定の場合は、メインで使用していると思われる1つだけを返す。
        /// </summary>
        public bool? All { get; set; }
    }
}
