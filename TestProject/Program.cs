

using TestProject.Manifest;


bool isCreate = false;

TestCaseSetting setting = null;
if (isCreate)
{
    setting = TestCaseSetting.Load();
}
else
{
    setting = new()
    {
        TestCase = new TestCase
        {
            Server_Protocol = "http",
            Server_Address = "localhost",
            Server_Port = 5000,
            ActionList = new List<TestAction>
            {
                new TestAction
                {
                    Address = "/api/server/network",
                    Method = "POST",
                    BodpyParameters = new Dictionary<string, string>
                    {
                        { "refresh", "true" }
                    },
                    TestResults = new List<TestResult>
                    {
                        new TestResult
                        {
                            TestType = "key",
                            TestCode = "/networkInterface/[0]/name",
                            Expected = "Wi-Fi"
                        }
                    }
                }
            }
        }
    };
}

setting.TestCase.ActionList.ForEach(x =>
    Console.WriteLine(x.toCurlCommand($"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}{x.Address}")));



setting.TestCase.ActionList.ForEach(x =>
{
    var url = $"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}{x.Address}";
    x.Send(url).Wait();

    //string json = x.GetJsonResponseBody();
    //Console.WriteLine(json);

    /*
    x.GetNodeValue("networkInterface").ToList().ForEach(y =>
    {
        Console.WriteLine(y);
    });
    */
    //var ntw = x.GetNodeValue("/networkInterface/[0]/name");
    //Console.WriteLine(ntw);

    x.TestStart();
});


setting.Save();



Console.ReadLine();
