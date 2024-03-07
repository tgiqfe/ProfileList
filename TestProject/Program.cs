﻿
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
    Console.WriteLine(x.toCurlCommand($"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}{x.Address}")));



setting.TestCase.ActionList.ForEach(x =>
{
    var server = $"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}";

    //var url = $"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}{x.Address}";
    x.Send(server, x.Address).Wait();
});


setting.Save();



Console.ReadLine();
