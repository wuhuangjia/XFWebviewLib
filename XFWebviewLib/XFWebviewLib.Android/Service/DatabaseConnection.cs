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
using SQLite;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection))]
namespace XFWebviewLib.Droid.Service
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public SQLiteConnection DbConnection(string dbName)
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return new SQLiteConnection(path);
        }
    }
}