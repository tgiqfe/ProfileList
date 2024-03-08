namespace TestProject.Manifest
{
    /// <summary>
    /// テストケースの設定
    /// </summary>
    internal class TestCase
    {
        /// <summary>
        /// テスト先のサーバーのプロトコル
        /// http or https
        /// </summary>
        public string ServerProtocol { get; set; }

        /// <summary>
        /// テスト先のサーバーのアドレス
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// テスト先のサーバーのポート
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// テスト動作のリスト
        /// </summary>
        public List<TestAction> Actions { get; set; }
    }
}
