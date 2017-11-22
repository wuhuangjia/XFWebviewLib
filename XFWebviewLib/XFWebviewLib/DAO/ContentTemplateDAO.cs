using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms;
using XFWebviewLib.Interface;
using XFWebviewLib.Model;
using System.Linq;
using System.Collections.ObjectModel;
using XFWebviewLib.Infrastructure;

namespace XFWebviewLib.DAO
{
    public class ContentTemplateDAO
    {
        public string DBPath { get; set; }
        private SQLiteConnection db;
        private static object collisionLock = new object();
        public ObservableCollection<htmltemplate> htmltemplates { get; set; }

        public ContentTemplateDAO()
        {
            db = DependencyService.Get<IDatabaseConnection>().DbConnection(AppData.DBName);
            DBPath = db.DatabasePath;
            db.CreateTable<htmltemplate>();
            this.htmltemplates = new ObservableCollection<htmltemplate>(db.Table<htmltemplate>());
            // If the table is empty, initialize the collection
            if (!db.Table<htmltemplate>().Any())
            {
                AddNewTemplate();
            }
        }

        private void AddNewTemplate()
        {
            this.htmltemplates.Add(new htmltemplate
            {
                htmltemplate_id = Guid.NewGuid().ToString(),
                htmltemplate_subject = "test",
            });
        }
    }
}
