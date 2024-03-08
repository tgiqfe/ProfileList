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
            bool isCreate = true;

            TestCaseSetting setting = null;
            if (isCreate)
            {
                setting = TestCaseSetting.Load2();
            }
            else
            {
                setting = new()
                {
                    TestCaseList = new List<TestCase>() { new TestCase()
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
                    }}
                };
            }
        }
    }
}
