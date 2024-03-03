namespace ProfileList.Lib.Profile
{
    /// <summary>
    /// 対象フォルダー配下のファイル数、ディレクトリ数、サイズをカウントするクラス
    /// </summary>
    public class FileSystemCount
    {
        public int FileCount { get; set; }
        public int DirectoryCount { get; set; }
        public long Size { get; set; }
        public string SizeText { get { return GetSize(); } }

        public bool IsFinished { get; set; }

        private string _targetPath = null;

        public FileSystemCount(string targetPath, bool countStart = false)
        {
            _targetPath = targetPath;

            if (string.IsNullOrEmpty(targetPath) || !Directory.Exists(targetPath))
            {
                IsFinished = true;
            }

            if (countStart)
            {
                CountStartAsync();
            }
        }

        public async void CountStartAsync()
        {
            if (IsFinished) return;
            await Task.Run(() =>
            {
                CountUp(new DirectoryInfo(_targetPath));
                IsFinished = true;
            });
        }

        public void CountStart()
        {
            if (IsFinished) return;
            CountUp(new DirectoryInfo(_targetPath));
            IsFinished = true;
        }

        private void CountUp(DirectoryInfo di)
        {
            DirectoryCount++;
            if ((di.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                return;
            }
            try
            {
                var files = di.GetFiles();
                FileCount += files.Length;
                Size += files.Select(x => x.Length).Sum();
                foreach (var subDir in di.GetDirectories())
                {
                    CountUp(subDir);
                }
            }
            catch { }
        }

        public string GetSize()
        {
            if (Size < 1024)
            {
                return $"{Size}B";
            }
            else if (Size < 1024 * 1024)
            {
                var size = Math.Round((double)Size / 1024, 2, MidpointRounding.AwayFromZero);
                return $"{size}KB";
            }
            else if (Size < 1024 * 1024 * 1024)
            {
                var size = Math.Round((double)Size / 1024 / 1024, 2, MidpointRounding.AwayFromZero);
                return $"{size}MB";
            }
            else
            {
                var size = Math.Round((double)Size / 1024 / 1024 / 1024, 2, MidpointRounding.AwayFromZero);
                return $"{size}GB";
            }
        }
    }
}
