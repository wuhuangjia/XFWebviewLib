using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XFWebviewLib.Interface;
using XFWebviewLib.Droid.Service;

[assembly: Xamarin.Forms.Dependency(typeof(MyDocumentPath))]
namespace XFWebviewLib.Droid.Service
{
    class MyDocumentPath : IMyDocumentPath
    {
        public string GetDocumentPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        }
    }
}