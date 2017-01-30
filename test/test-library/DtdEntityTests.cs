using System;
using System.Collections.Generic;
using DtdTool;
using Xunit;

namespace Tests
{
    public class DtdEntityTests
    {
        [Fact]
        public void TestDtdEntityToString() 
        {
            DtdEntity entity = new DtdEntity
            {
                Name = "TestEntity",
                Fields = new Dictionary<string, string>()
            };
            entity.Fields.Add("TestString", "string TestString");
            entity.Fields.Add("TestList", "List<int> TestList");

            string entityString = entity.ToString();
            string expectString = @"using System;
using System.Collections.Generic;

public class TestEntity
{
    public string TestString { get; set; }
    public List<int> TestList { get; set; }
}
";
            Assert.Equal(entityString, expectString);
        }
    }
}
