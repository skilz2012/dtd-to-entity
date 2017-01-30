using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtdTool
{
    public class DtdParser
    {
        private string _dtdContent;

        public DtdParser(string dtdContent)
        {    
            _dtdContent = dtdContent;
        }

        public List<DtdEntity> Parse()
        {
            List<DtdEntity> allEntity = new List<DtdEntity>();

            Regex regex = new Regex(@"<!ELEMENT\s+(\w+)\s+\((\s*.*?,\s*)*(\s*.*?\s*)\)>");
            MatchCollection matches = regex.Matches(_dtdContent);

            foreach (Match m in matches)
            {
                if (m.Groups.Count < 3)
                {
                    throw new Exception("Regex match failed for string:" + m.Value);
                }

                DtdEntity entity = new DtdEntity();
                entity.Name = m.Groups[1].Value.Trim();
                entity.Fields = new Dictionary<string, string>();

                foreach (Capture c in m.Groups[2].Captures)
                {
                    entity.Fields.Add(c.Value.Replace(',', ' ').Trim(), "");
                }

                if (m.Groups.Count >= 4)
                {
                    foreach (Capture c in m.Groups[3].Captures)
                    {
                        entity.Fields.Add(c.Value.Replace(',', ' ').Trim(), "");
                    }
                }

                if(entity.Fields.ContainsKey("#PCDATA"))
                {
                    continue;
                }

                allEntity.Add(entity);
            }

            for (int i = 0; i < allEntity.Count; i++)
            {
                DtdEntity entity = allEntity[i];

                string[] fields = new string[entity.Fields.Count];
                entity.Fields.Keys.CopyTo(fields, 0);

                for (int j = 0; j < fields.Length; j++)
                {
                    string key = fields[j];
                    string typeName = key;
                    string fieldName = key;

                    if (key.EndsWith("?") || key.EndsWith("*") || key.EndsWith("+"))
                    {
                        typeName = key.Substring(0, fieldName.Length - 1);
                        fieldName = key.Substring(0, fieldName.Length - 1);
                    }
                    if (!allEntity.Exists(c => c.Name == typeName))
                    {
                        typeName = "string";
                    }

                    if (key.EndsWith("*") || key.EndsWith("+"))
                    {
                        entity.Fields[key] = $"List<{typeName}> {fieldName}";
                    }
                    else
                    {
                        entity.Fields[key] = $"{typeName} {fieldName}";
                    }
                }
            }

            return allEntity;
        }
    }
}
