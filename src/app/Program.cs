using System;
using System.IO;
using DtdTool;

namespace ConsoleApplication
{
    public class Program
    {
        private static string _dtdFilePath;
        private static string _classDir;

        public static void Main(string[] args)
        {
            if(!CheckArgs(args))
            {
                return;
            }

            try
            {
                string dtdContent = File.ReadAllText(_dtdFilePath);
                DtdParser parser = new DtdParser(dtdContent);
                var dtdEntityList = parser.Parse();
                if(dtdEntityList == null || dtdEntityList.Count == 0)
                {
                    Console.WriteLine("0 entity was parsed.");
                    return;
                }

                if(!Directory.Exists(_classDir))
                {
                    Directory.CreateDirectory(_classDir);
                }

                foreach (var entity in dtdEntityList)
                {
                    string classFilePath = Path.Combine(_classDir, $"{entity.Name}.cs");
                    File.WriteAllText(classFilePath, entity.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Convert failed.");
                Console.WriteLine($"{ex.Message}");
            }

            Console.WriteLine($"Convert successfully. All class files were saved into {_classDir}.");
        }

        private static bool CheckArgs(string[] args)
        {
            if(args == null || args.Length < 4)
            {
                Console.WriteLine("-i input dtd file path\r\n-o output class file directory");
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if(args[i].ToUpper() == "-O")
                {
                    if(i + 1 >= args.Length)
                    {
                        Console.WriteLine("Output directory should be specified.");
                        return false;
                    }
                    else
                    {
                        _classDir = args[++i];
                    }
                }

                if(args[i].ToUpper() == "-I")
                {
                    if(i + 1 >= args.Length)
                    {
                        Console.WriteLine("Input dtd file should be specified.");
                        return false;
                    }
                    else
                    {
                        _dtdFilePath = args[++i];
                    }
                }
            }

            if(string.IsNullOrWhiteSpace(_classDir))
            {
                Console.WriteLine("Output directory should be specified.");
                return false;
            }

            if(string.IsNullOrWhiteSpace(_dtdFilePath))
            {
                Console.WriteLine("Input dtd file should be specified.");
                return false;
            }

            return true;
        }
    }
}
