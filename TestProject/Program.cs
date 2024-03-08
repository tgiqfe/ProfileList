
using System.Text.Json;
using TestProject.Manifest;

TestFileCollection collection = new();

/*
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
                    Actions = new()
                    {
                        new TestAction()
                        {
                            Address = "/api/profile/list",
                            Method = TestAction.METHOD_GET,
                            Results = new()
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
*/

//string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true });
//Console.WriteLine(json);

collection.Save();


Console.ReadLine();
