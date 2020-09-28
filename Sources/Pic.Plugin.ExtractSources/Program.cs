#region Using directives
using System;
using System.Configuration;
using System.IO;
#endregion

namespace Pic.Plugin.ExtractSources
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderDlls = ConfigurationManager.AppSettings.Get("FolderComponentFiles");
            string folderSources = ConfigurationManager.AppSettings.Get("FolderComponentSources");

            IComponentSearchMethod sm = new ComponentSearchDirectory(folderDlls);

            foreach (string filePath in Directory.GetFiles(folderDlls, "*.dll", SearchOption.AllDirectories))
            {
                var sw = File.CreateText(
                    Path.Combine(
                        folderSources,
                        Path.GetFileName(Path.ChangeExtension(filePath, "cs")))
                        );
                using (ComponentLoader cl = new ComponentLoader() { SearchMethod = sm })
                {
                    var component = cl.LoadComponent(filePath);
                    sw.WriteLine("// GUID        : " + component.Guid.ToString());
                    sw.WriteLine("// Name        : " + component.Name);
                    sw.WriteLine("// Description : " + component.Description);
                    sw.WriteLine("// Author      : " + component.Author);
                    sw.WriteLine("// Version     : " + component.Version);
                    sw.WriteLine("// Thumbnail   : " + (component.HasEmbeddedThumbnail ? "true" : "false"));
                    sw.WriteLine("// Date        : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    sw.WriteLine("// ### ");
                    sw.Write(component.SourceCode);

                    if (component.HasEmbeddedThumbnail)
                    {
                        // get thumbnail image
                        var bmp = component.Thumbnail;
                        // save thumbnail image as bmp
                        string bitmapPath = Path.Combine( folderSources, Path.GetFileName(Path.ChangeExtension(filePath, "bmp")));
                        bmp.Save(bitmapPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
                sw.Close();

                Console.Write(".");
            }
            Console.WriteLine();
        }
    }
}
