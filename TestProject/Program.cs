
using System.Text.Json;
using TestProject.Manifest;

TestFileCollection collection = new();
collection.Files = new()
{
    new TestFile()
    {
        FileName = "01_ProfileList_[Api_Profile_List].yml",
        TestCase = new()
        {
            List = new()
            {
                new TestCase()
                {
                    ServerProtocol = "http",
                    ServerAddress = "localhost",
                    ServerPort = 5000,
                    ActionList = new()
                    {
                        new TestAction()
                        {
                            Address = "/api/profile/list",
                            Method = TestAction.METHOD_GET,
                            TestResultList = new()
                            {
                                new TestResult()
                                {
                                    TestType = "key",
                                    TestCode = "/profiles/[0]/userName",
                                    Expected = "User",
                                }
                            }
                        }
                    },
                }
            }
        }
    }
};


/*
collection.TestCaseSetting.List.Add(new TestCase()
{
    FileName = "01_ProfileList_[Api_Profile_List].yml",
    ServerProtocol = "http",
    ServerAddress = "localhost",
    ServerPort = 5000,
    ActionList = new List<TestAction>()
    {
        new TestAction()
        {
            Address = "/api/profile/list",
            Method = TestAction.METHOD_GET,
            TestResultList = new()
            {
                new TestResult()
                {
                    TestType = "key",
                    TestCode = "/profiles/[0]/userName",
                    Expected = "User",
                }
            }
        }
    },
});
*/

string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);

collection.Save();


Console.ReadLine();
