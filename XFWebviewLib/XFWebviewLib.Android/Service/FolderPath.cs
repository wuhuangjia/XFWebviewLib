using System;
using System.IO;
using XFWebviewLib.Droid.Service;
using XFWebviewLib.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(FolderPath))]
namespace XFWebviewLib.Droid.Service
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
            throw new NotImplementedException();
        }
    }
}
