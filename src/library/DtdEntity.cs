using System.Collections.Generic;
using System.Text;

namespace DtdTool
{
    public class DtdEntity
    {
        // Entity name
        public string Name { get; set; }

        // Fields name, type
        public Dictionary<string, string> Fields { get; set; }

        public override string ToString()
        {
            StringBuilder classBuilder = new StringBuilder();

            classBuilder.AppendLine($@"using System;
using System.Collections.Generic;

public class {Name}
{{");

            foreach(string field in Fields.Values)
            {
                classBuilder.AppendLine($"    public {field} {{ get; set; }}");
            }

            classBuilder.AppendLine("}");

            return classBuilder.ToString();
        }
    }
}