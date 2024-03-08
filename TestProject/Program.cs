
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
            ServerProtocol = "http",
            ServerAddress = "localhost",
            ServerPort = 5000,
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
                            Expected = "イーサネット"
                        },
                        new TestResult
                        {
                            TestType = "log",
                            TestCode = "line=1",
                            Expected = "[POST]Get Network Info."
                        },
                        new TestResult
                        {
                            TestType = "log",
                            TestCode = "line=10",
                            Expected = "[POST]Get Network Info."
                        }
                    }
                }
            }
        }
    };
}

setting.TestCase.ActionList.ForEach(x =>
    Console.WriteLine(x.toCurlCommand($"{setting.TestCase.ServerProtocol}://{setting.TestCase.ServerAddress}:{setting.TestCase.ServerPort}{x.Address}")));



setting.TestCase.ActionList.ForEach(x =>
{
    var server = $"{setting.TestCase.ServerProtocol}://{setting.TestCase.ServerAddress}:{setting.TestCase.ServerPort}";
    x.Send(server, x.Address).Wait();
});


setting.Save();



Console.ReadLine();
