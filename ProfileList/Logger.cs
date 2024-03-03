namespace ProfileList
{
    public class Logger
    {
        #region class parameter

        /// <summary>
        /// ログファイルの名前
        /// </summary>
        private string _logFile = null;
        public string LogFile
        {
            get
            {
                _logFile ??= $"ProfileList_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log";
                return _logFile;
            }
        }

        /// <summary>
        /// ログファイルへの絶対パス
        /// </summary>
        public string LogPath { get; private set; }

        /// <summary>
        /// ログファイルへの出力を一時停止
        /// </summary>
        private bool _pause = false;
        public bool Pause
        {
            get { return _pause; }
            set
            {
                if(_pause == value) { return; }
                _pause = value;
                if (!_pause)
                {
                    File.AppendAllLines(LogPath, _tempStoreLogs);
                    _tempStoreLogs.Clear();
                }
            }
        }

        /// <summary>
        /// 一時停止中にログを保存するリスト
        /// </summary>
        private List<string> _tempStoreLogs = null;

        #endregion


        public Logger(string logDirectory)
        {
            string parent = Path.Combine(Item.WorkingDirectory, Item.Setting.LogDirectory);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            this.LogPath = Path.Combine(parent, LogFile);
            this.Pause = false;
            _tempStoreLogs = new();
        }

        /// <summary>
        /// ログをファイルに書き込み。
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            string log = $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}] {message}{Environment.NewLine}";

            if (this.Pause)
            {
                _tempStoreLogs.Add(log);
            }
            else
            {
                File.AppendAllText(LogPath, log);
            }
        }



    }
}
