using System.Diagnostics;

namespace ProfileList2.Lib.ScriptLanguage
{
    public class DefaultLanguageSetting
    {
        public static List<Language> Generate()
        {
            return new()
            {
                Exe(),
                Cmd(),
                //PowerShell(),
                Pwsh(),
                WScript(),
                Go(),
                NodeJS(),
                Python(),
            };
        }

        /// <summary>
        /// 実行ファイル (exeファイル)
        /// </summary>
        /// <returns></returns>
        private static Language Exe()
        {
            return new()
            {
                Name = "exe",
                Description = "実行ファイル",
                Extensions = new[] { ".exe" },
                Command = null,
            };
        }

        /// <summary>
        /// Windowsバッチファイル
        /// </summary>
        /// <returns></returns>
        private static Language Cmd()
        {
            return new()
            {
                Name = "cmd",
                Description = "Windowsバッチファイル",
                Alias = new[] { "dos", "bat", "batch" },
                Extensions = new[] { ".cmd", ".bat" },
                Encoding = "Shift_JIS",
                Command = "cmd",
                ArgsPrefix = "/c \"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// PowerShell (Windows標準版)
        /// </summary>
        /// <returns></returns>
        private static Language PowerShell()
        {
            return new()
            {
                Name = "PowerShell",
                Alias = new[] { "pshell", "pwsh" },
                Description = "PowerShell (Windows標準版)",
                Extensions = new[] { ".ps1" },
                Encoding = "Shift_JIS",
                Command = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe",
                ArgsPrefix = "-ExecutionPolicy Unrestricted -File \"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// PowerShell 7 (Cross-Platform版)
        /// </summary>
        /// <returns></returns>
        private static Language Pwsh()
        {
            return new()
            {
                Name = "Pwsh7",
                Alias = new[] { "pshell", "pwsh" },
                Description = "PowerShell (Cross-Platfform版)",
                Extensions = new[] { ".ps1" },
                Encoding = "UTF-8",
                Command = WhereCommand("pwsh") ?? @"C:\Program Files\PowerShell\7\pwsh.exe",
                ArgsPrefix = "-ExecutionPolicy Unrestricted -File \"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// Windows Script Host (VBS, JScript)
        /// </summary>
        /// <returns></returns>
        private static Language WScript()
        {
            return new()
            {
                Name = "WScript",
                Alias = new[] { "vbs", "vbscript", "js", "jscript" },
                Description = "Windows Script Host (VBS, JScript)",
                Extensions = new[] { ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh" },
                Encoding = "Shift_JIS",
                Command = @"C:\Windows\System32\wscript.exe",
                ArgsPrefix = "//nologo \"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// Golang
        /// </summary>
        /// <returns></returns>
        private static Language Go()
        {
            return new()
            {
                Name = "Go",
                Alias = new[] { "golang" },
                Description = "Golang",
                Extensions = new[] { ".go" },
                Encoding = "UTF-8",
                Command = WhereCommand("go") ?? @"C:\Program Files\Go\bin\go.exe",
                ArgsPrefix = "\"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// Node.js
        /// </summary>
        /// <returns></returns>
        private static Language NodeJS()
        {
            return new()
            {
                Name = "Node.js",
                Alias = new[] { "nodejs", "node" },
                Description = "Node.js",
                Extensions = new[] { ".js", ".jsx", ".ts", ".tsx" },
                Encoding = "UTF-8",
                Command = WhereCommand("node") ?? @"C:\Program Files\nodejs\node.exe",
                ArgsPrefix = "\"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// Python
        /// とりあえずPython3.9系のパスをデフォルトセット
        /// </summary>
        /// <returns></returns>
        private static Language Python()
        {
            return new()
            {
                Name = "Python",
                Alias = new[] { "py" },
                Description = "Python",
                Extensions = new[] { ".py", ".pyw" },
                Encoding = "UTF-8",
                Command = WhereCommand("python", "python3") ?? @"C:\Program Files\Python39\python.exe",
                ArgsPrefix = "\"",
                ArgsMidWithoutArgs = "\"",
                ArgsMidWithArgs = "\" ",
                ArgsSuffix = "",
            };
        }

        /// <summary>
        /// whereコマンドを実行して、コマンドのパスを取得する
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        private static string WhereCommand(params string[] commands)
        {
            foreach (string command in commands)
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = "where.exe";
                    proc.StartInfo.Arguments = command;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();
                    string output_psCommandPath = proc.StandardOutput.ReadLine();
                    proc.WaitForExit();
                    return proc.ExitCode == 0 ? output_psCommandPath.Trim() : null;
                }
            }
            return null;
        }
    }
}
