#region FileHeader
/*
                                     \\\///
                                    / _  _ \
                                  (| (.)(.) |)
+-------------------------------.OOOo--()--oOOO.----------------------------------+
 Copyright (C) 2015 Newegg. All rights reserved.

 Class Name:              DTD2Entity
 Author:                  jy86
 Date:                    2015 4/14/2015 8:45:11 AM
 CLR Version:             4.0.30319.18408
 Namespace:               Newegg.OZZO.ConnectShip.Entity
 Description:             Provide operations to transform DTD to entity

 Revision History
 Date                     User    Version    Description
 --------------------------------------------------------------------------------- 
 4/14/2015 8:45:11 AM    jy86    1.0        Create.
+--------------------------------.oooO--------------------------------------------+
                                  (   )   Oooo.
                                   \ (    (   )
                                    \_)    ) /
                                          (_/
*/
#endregion

namespace Newegg.OZZO.ConnectShip.Entity
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provide operations to transform DTD to entity
    /// </summary>
    public class DTD2Entity
    {
        public string DTDPath { get; set; }
        public string EntityDir { get; set; }
        public string Namespace { get; set; }
        public string BaseClassName { get; set; }

        public DTD2Entity(string dtdPth, string entityDir, string nameSpace, string baseClassName)
        {
            DTDPath = dtdPth;
            EntityDir = entityDir;
            Namespace = nameSpace;
            BaseClassName = baseClassName;
        }

        public void Transform()
        {
            string dtdContent = File.ReadAllText(DTDPath);

            List<EntityClass> allEntityClasses = ParseEntityClass(dtdContent);

            CreateClassFiles(allEntityClasses);
        }

        private List<EntityClass> ParseEntityClass(string dtdContent)
        {
            List<EntityClass> allEntityClasses = new List<EntityClass>();

            //Regex regex = new Regex(@"<!ELEMENT\s+(\w+)\s+\((\s*\w+[\?\*\+]*,\s*)*(\s*\w+[\?\*\+]*\s*)\)>");
            Regex regex = new Regex(@"<!ELEMENT\s+(\w+)\s+\((\s*.*?,\s*)*(\s*.*?\s*)\)>");
            MatchCollection matches = regex.Matches(dtdContent);

            foreach (Match m in matches)
            {
                if (m.Groups.Count < 3)
                {
                    throw new Exception("Regex match failed for string:" + m.Value);
                }

                EntityClass entityClass = new EntityClass();
                entityClass.ClassName = m.Groups[1].Value.Trim();
                entityClass.ClassFields = new Dictionary<string, string>();

                foreach (Capture c in m.Groups[2].Captures)
                {
                    entityClass.ClassFields.Add(c.Value.Replace(',', ' ').Trim(), "");
                }

                if (m.Groups.Count >= 4)
                {
                    foreach (Capture c in m.Groups[3].Captures)
                    {
                        entityClass.ClassFields.Add(c.Value.Replace(',', ' ').Trim(), "");
                    }
                }

                allEntityClasses.Add(entityClass);
            }

            for (int i = 0; i < allEntityClasses.Count; i++)
            {
                EntityClass entity = allEntityClasses[i];

                string[] fields = new string[entity.ClassFields.Count];
                entity.ClassFields.Keys.CopyTo(fields, 0);

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
                    if (!allEntityClasses.Exists(c => c.ClassName == typeName))
                    {
                        typeName = "string";
                    }

                    if (key.EndsWith("*") || key.EndsWith("+"))
                    {
                        entity.ClassFields[key] = string.Format(@"        [XmlElement(ElementName = ""{1}"")]
        public List<{0}> {1};", typeName, fieldName);
                    }
                    else
                    {
                        entity.ClassFields[key] = string.Format("        public {0} {1};", typeName, fieldName);
                    }
                }
            }

            return allEntityClasses;
        }

        private void CreateClassFiles(List<EntityClass> allEntityClasses)
        {
            if(!Directory.Exists(EntityDir))
            {
                Directory.CreateDirectory(EntityDir);
            }

            foreach(EntityClass entity in allEntityClasses)
            {
                StringBuilder classFileContent = new StringBuilder();
                classFileContent.AppendFormat(@"
/*Auto generated by DTD2Entity utility.
  Author: John.Z.Yang                  */
namespace {0}
{{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Newegg.OZZO.ConnectShip.Entity;

    /// <summary>
    /// {1} Entity
    /// </summary>
    public class {1} : {2}
    {{", Namespace, entity.ClassName, BaseClassName);

                foreach (string field in entity.ClassFields.Values)
                {
                    classFileContent.AppendFormat(@"
{0}", field);
                }

                classFileContent.Append(@"
    }
}
");
                string classFileName = EntityDir + (EntityDir.EndsWith("\\") ? "" : "\\") + entity.ClassName + ".cs";
                File.WriteAllText(classFileName, classFileContent.ToString());
            }
        }
    }

    public class EntityClass
    {
        public string ClassName { get; set; }
        public Dictionary<string, string> ClassFields { get; set; }
    }
}
