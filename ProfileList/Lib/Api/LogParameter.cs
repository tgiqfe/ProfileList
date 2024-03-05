using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class LogParameter : ApiParameter
    {
        /// <summary>
        /// ログ出力時に表示する行数
        /// ※LineとRequestの両方が無指定の場合は、Line = 10 とする
        /// </summary>
        public int? Line { get; set; }

        /// <summary>
        /// ログ出力時に表示するリクエスト番号
        /// ※LineとRequestの両方を指定した場合は、Requestを優先する
        /// </summary>
        public int? Request { get; set; }

        /// <summary>
        /// 全ログを出力する。
        /// ログファイルは日付ごとに生成されるので、その日のログ全てを出力。
        /// </summary>
        public bool? All { get; set; }
    }
}
