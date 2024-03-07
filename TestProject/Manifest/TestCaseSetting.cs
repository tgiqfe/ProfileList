﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using TestProject.Lib;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    /// <summary>
    /// テストケースのTopクラス
    /// </summary>
    internal class TestCaseSetting
    {
        public TestCase TestCase { get; set; }

        /// <summary>
        /// 初期設定
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// ファイルから読み込んで、TestCaseSettingクラスを生成
        /// </summary>
        /// <returns></returns>
        public static TestCaseSetting Load()
        {
            string path = Path.Combine(Item.WorkingDirectory, "testcase.yml");
            TestCaseSetting tc = null;
            try
            {
                string content = File.ReadAllText(path);
                tc = new Deserializer().Deserialize<TestCaseSetting>(content);
            }
            catch { }
            if(tc == null)
            {
                tc = new();
                tc.Init();
            }
            return tc;
        }

        /// <summary>
        /// TestCaseSettingクラスをファイルに保存
        /// </summary>
        public void Save()
        {
            string path = Path.Combine(Item.WorkingDirectory, "testcase.yml");
            string content = new SerializerBuilder().
                WithEventEmitter(x => new MultilineScalarFlowStyleEmitter(x)).
                Build().Serialize(this);
            File.WriteAllText(path, content);
        }
    }
}
