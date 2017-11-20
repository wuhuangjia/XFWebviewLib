using System;
using System.Collections.Generic;
using System.Text;
using XFWebviewLib.iOS.Service;
using XFWebviewLib.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(MyDocumentPath))]
namespace XFWebviewLib.iOS.Service
{
    class MyDocumentPath : IMyDocumentPath
    {
        public string GetDocumentPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
