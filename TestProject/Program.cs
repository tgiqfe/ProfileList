

using TestProject.Manifest;


bool isCreate = true;

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
            Description = "Test case description",
            ActionList = new List<TestAction>
            {
                new TestAction
                {
                    Address = "/api/profile/list",
                    Method = "GET",
                    ContentType = "application/json",
                    BodpyParameters = new Dictionary<string, string>
                    {
                        { "refresh", "true" }
                    }
                }
            }
        }
    };
}

setting.TestCase.ActionList.ForEach(x =>
    Console.WriteLine(x.toCurlCommand($"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}{x.Address}")));

setting.Save();

setting.TestCase.ActionList.ForEach(x =>
{
    var url = $"{setting.TestCase.Server_Protocol}://{setting.TestCase.Server_Address}:{setting.TestCase.Server_Port}{x.Address}";
    x.Send(url).Wait();

    string json = x.GetJsonResponseBody();
    Console.WriteLine(json);

});





Console.ReadLine();
