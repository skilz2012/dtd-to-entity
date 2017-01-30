using System;
using DtdTool;
using Xunit;

namespace Tests
{
    public class DtdParserTests
    {
        [Fact]
        public void TestDtdParser() 
        {
            string dtd = @"<!ELEMENT NEWSPAPER (ARTICLE+)>
<!ELEMENT ARTICLE (HEADLINE,BYLINE)>
<!ELEMENT HEADLINE (#PCDATA)>
<!ELEMENT BYLINE (#PCDATA)>";
            DtdParser parser = new DtdParser(dtd);
            var result = parser.Parse();
            Assert.Equal(result.Count, 2);
            Assert.Equal(result[0].Name, "NEWSPAPER");
            Assert.Equal(result[0].Fields["ARTICLE+"], "List<ARTICLE> ARTICLE");
            Assert.Equal(result[1].Name, "ARTICLE");
            Assert.Equal(result[1].Fields["HEADLINE"], "string HEADLINE");
            Assert.Equal(result[1].Fields["BYLINE"], "string BYLINE");
        }
    }
}
