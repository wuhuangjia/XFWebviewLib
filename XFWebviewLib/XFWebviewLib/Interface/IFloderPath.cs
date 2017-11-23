using System;
namespace XFWebviewLib.Interface
{
    public interface IFloderPath
    {
        string GetPath(Environment.SpecialFolder SpecialFloder, string FloderName);
        string GetTempDirectory();
    }
}
