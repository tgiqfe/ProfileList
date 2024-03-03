using System.Linq;

namespace ProfileList.Lib.Api
{
    public class Log
    {
        public static IEnumerable<string> Print(LogParameter parameter = null)
        {
            try
            {
                string text = File.ReadAllText(Item.Logger.LogPath);

                if (parameter?.AllPrint ?? false)
                {
                    Item.Logger.WriteLine("Print all log.");
                    return text.Split(Environment.NewLine).
                        Where(x => x != "");
                }
                else
                {
                    var outputLine = (int)((parameter?.Line ?? 0) > 0 ? parameter.Line : 10);
                    Item.Logger.WriteLine($"Print {outputLine} lastline log.");
                    return text.Split(Environment.NewLine).
                        Where(x => x != "").
                        TakeLast(outputLine);
                }
            }
            catch
            {
                return new string[] { "" };
            }
        }
    }
}
