using System;
using System.IO;
using XFWebviewLib.Interface;
using XFWebviewLib.iOS.Service;

[assembly: Xamarin.Forms.Dependency(typeof(FloderPath))]
namespace XFWebviewLib.iOS.Service
{
    public class FloderPath : IFloderPath
    {
        public FloderPath()
        {
        }
       
        public string GetPath(Environment.SpecialFolder SpecialFloder, string FloderName)
        {
            string personalFolder = System.Environment.GetFolderPath(SpecialFloder);
            var path = Path.Combine(personalFolder, FloderName);
            return path;
        }

        public string GetTempDirectory()
        {
            var tmp = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("Documents", "tmp")}/";
            return tmp;
        }
    }
}
