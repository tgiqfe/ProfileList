using System.Linq;

namespace ProfileList.Lib.Api
{
    public class Log
    {
        /// <summary>
        /// ログの出力
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static IEnumerable<string> Print(LogParameter parameter = null)
        {
            try
            {
                string text = File.ReadAllText(Item.Logger.LogPath);

                if (parameter?.All ?? false)
                {
                    //  全ログを出力
                    Item.Logger.WriteLine("Print all log.");
                    return text.Split(Environment.NewLine).
                        Where(x => x != "");
                }
                else if (parameter?.Request > 0)
                {
                    //  ログの最後から指定したRequest単位で出力
                    Item.Logger.WriteLine($"Print {parameter.Request} request log.");

                    var texts = text.Split(Environment.NewLine).Where(x => x != "").ToArray();
                    int count = 0;
                    int position = texts.Length - 1;
                    for (; position >= 0 && count < parameter.Request; position--)
                    {
                        //                       ↓22文字スキップ
                        //  [2024/01/01 00:00:00] Log message body.
                        var logmessage = texts[position].Substring(22);
                        if (logmessage.StartsWith("[GET]") ||
                            logmessage.StartsWith("[POST]") ||
                            logmessage.StartsWith("[DELETE]") ||
                            logmessage.StartsWith("[PUT]") ||
                            logmessage.StartsWith("Start Service."))
                        {
                            count++;
                        }
                    }
                    return texts.Skip(position + 1);
                }
                else
                {
                    //  ログの最後から行数指定して出力
                    //  無指定の場合は10行
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
