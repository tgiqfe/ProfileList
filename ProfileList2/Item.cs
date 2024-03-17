using ProfileList2.Lib.ScriptLanguage;
using System.Diagnostics;

namespace ProfileList2
{
    public class Item
    {
        public static string WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static LanguageCollection LanguageCollection = LanguageCollection.Load(Path.Combine(WorkingDirectory, "languages.json"));

        public static Setting Setting = null;
    }
}
