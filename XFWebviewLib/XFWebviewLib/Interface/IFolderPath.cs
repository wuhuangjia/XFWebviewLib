using System;
namespace XFWebviewLib.Interface
{
    public interface IFolderPath
    {
        string GetPath(Environment.SpecialFolder SpecialFloder, string FloderName);
        string GetTempDirectory();
    }
}
