using System;
using System.IO;
using XFWebviewLib.Interface;
using XFWebviewLib.iOS.Service;

[assembly: Xamarin.Forms.Dependency(typeof(FolderPath))]
namespace XFWebviewLib.iOS.Service
{
    public class FolderPath : IFolderPath
    {
        public FolderPath()
        {
        }
       
        public string GetPath(Environment.SpecialFolder SpecialFolder, string FolderName)
        {
            string personalFolder = System.Environment.GetFolderPath(SpecialFolder);
            var path = Path.Combine(personalFolder, FolderName);
            return path;
        }

        public string GetTempDirectory()
        {
            var tmp = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("Documents", "tmp")}/";
            return tmp;
        }
    }
}
