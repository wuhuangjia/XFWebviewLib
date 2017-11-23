using System;
using System.IO;
using XFWebviewLib.Droid.Service;
using XFWebviewLib.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(FloderPath))]
namespace XFWebviewLib.Droid.Service
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
            throw new NotImplementedException();
        }
    }
}
