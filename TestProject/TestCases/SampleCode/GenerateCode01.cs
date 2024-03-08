using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Manifest;

namespace TestProject.TestCases.SampleCode
{
    internal class GenerateCode01
    {
        public static void Generate01()
        {
            TestCaseSetting setting = null;

            setting = new()
            {
                List = new List<TestCase>() { new TestCase()
                {
                    ServerProtocol = "http",
                    ServerAddress = "localhost",
                    ServerPort = 5000,
                    Actions = new List<TestAction>
                    {
                        new TestAction
                        {
                            Address = "/api/server/network",
                            Method = "POST",
                            BodpyParameters = new Dictionary<string, string>
                            {
                                { "refresh", "true" }
                            },
                            Results = new List<TestResult>
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
                }}
            };
        }
    }
}
