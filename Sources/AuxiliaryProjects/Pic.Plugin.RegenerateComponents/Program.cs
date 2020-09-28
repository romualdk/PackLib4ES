#region Using directives
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlTypes;
using System.Drawing;

using System.CodeDom.Compiler;
using System.Runtime.Serialization.Formatters;
#endregion

namespace Pic.Plugin.RegenerateComponents
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderDlls = ConfigurationManager.AppSettings.Get("FolderComponentFiles");
            string folderSources = ConfigurationManager.AppSettings.Get("FolderComponentSources");

            IComponentSearchMethod sm = new ComponentSearchDirectory(folderDlls);

            foreach (string filePath in Directory.GetFiles(folderSources, "*.cs", SearchOption.AllDirectories))
            {
                string sAuthor = string.Empty;
                string sName = string.Empty;
                string sDescription = string.Empty;
                string sVersion = string.Empty;
                string sGuid = string.Empty;
                bool hasThumbnail = false;
                string sDate = string.Empty;

                var fileLines = File.ReadAllLines(filePath);
                int lineIndex = 0;
                string str = fileLines[lineIndex++];
                if (str.Contains("GUID")) sGuid = str.Substring(str.IndexOf(':') + 2).Trim();
                str = fileLines[lineIndex++];
                if (str.Contains("Name")) sName = str.Substring(str.IndexOf(':') + 2).Trim();
                str = fileLines[lineIndex++];
                if (str.Contains("Description")) sDescription = str.Substring(str.IndexOf(':') + 2).Trim();
                str = fileLines[lineIndex++];
                if (str.Contains("Author")) sAuthor = str.Substring(str.IndexOf(':') + 2).Trim();
                str = fileLines[lineIndex++];
                if (str.Contains("Version")) sVersion = str.Substring(str.IndexOf(':') + 2).Trim();
                str = fileLines[lineIndex++];
                if (str.Contains("Thumbnail")) hasThumbnail = str.Substring(str.IndexOf(':') + 2).Trim() == "true";
                str = fileLines[lineIndex++];
                if (str.Contains("Date")) sDate = str.Substring(str.IndexOf(':') + 2).Trim();

                lineIndex +=1;
                
                StringBuilder sb = new StringBuilder();
                for (int i = lineIndex; i < fileLines.Count(); ++i)
                    sb.AppendLine( fileLines[i] );

                try
                {
                    string outName = Path.GetFileName(Path.ChangeExtension(filePath, "dll"));
                    {
                        Console.Write($"Compiling {outName}");

                        // instantiate PluginGenerator
                        PluginGenerator generator = new PluginGenerator
                        {
                            AssemblyCompany = sAuthor,
                            AssemblyDescription = sDescription,
                            AssemblyVersion = sVersion,
                            DrawingName = sName,
                            DrawingDescription = sDescription,
                            Guid = Guid.Parse(sGuid),
                            DrawingCode = sb.ToString(),
                            OutputName = outName,
                            OutputDirectory = folderDlls
                        };
                        string filePathBmp = Path.ChangeExtension(filePath, "bmp");
                        if (File.Exists(filePathBmp))
                        {
                            // set thumbnail path in generator
                            generator.ThumbnailPath = filePathBmp;
                        }
                        CompilerResults res = generator.Build();
                        if (res.Errors.Count > 0)
                        {
                            Console.WriteLine($" -> Failed");
                            for (int i = 0; i < res.Errors.Count; ++i)
                                Console.WriteLine($"{res.Errors[i].Line} : {res.Errors[i].ErrorText}");
                        }
                        else
                            Console.WriteLine($" -> Done");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
