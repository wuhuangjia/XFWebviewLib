using System;
using System.Collections.Generic;
using System.Text;
using XFWebviewLib.iOS.Service;
using XFWebviewLib.Interface;
using SQLite;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection))]
namespace XFWebviewLib.iOS.Service
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public SQLiteConnection DbConnection(string dbName)
        {
            string personalFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder = Path.Combine(personalFolder, "..", "Library");
            var path = Path.Combine(libraryFolder, dbName);
            return new SQLiteConnection(path);
        }
    }
}
