namespace ProfileList
{
    public class Logger
    {
        public string LogFile { get; private set; }

        public string LogPath { get; private set; }

        public Logger(string logDirectory)
        {
            this.LogFile = $"ProfileList_{DateTime.Now.ToString("yyyyMMdd")}.log";
            string parent = Path.Combine(Item.Setting.LogDirectory, LogFile);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            this.LogPath = Path.Combine(parent, LogFile);
        }

        public void WriteLine(string message)
        {
            string log = $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}] {message}{Environment.NewLine}";
            File.AppendAllText(LogPath, log);
        }

        public IEnumerable<string> Print(int line = 10)
        {
            string text = File.ReadAllText(this.LogPath);
            return text.Split(Environment.NewLine).
                Where(x => x != "").
                TakeLast(line);
        }
    }
}
