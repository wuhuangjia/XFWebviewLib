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
                htmltemplate template = new htmltemplate();
                template.htmltemplate_id = "1";
                template.htmltemplate_subject = "test";
                template.htmltemplate_type = "Home";
                template.htmltemplate_content = @"<html>
                <head>
                    <title>WebView</title>
                <link rel=""stylesheet"" type=""text/css"" href=""style.css""/>
                  </head>
                  <body>
                  <h1>Xamarin.Forms</h1>
                  <p>This is a local Html page</p>
                     <img src=""XamarinLogo.png""/>
                      </body>
                      </html>";
                template.update_date = DateTime.Now;
                template.create_date = DateTime.Now;
                this.Create(template);
            }
        }

        private int Create(htmltemplate template)
        {
            lock (collisionLock)
            {
                return db.Insert(template);
            }
        }

        private int Update(htmltemplate template)
        {
            lock (collisionLock)
            {
                return db.Update(template);
            }
        }

        public IEnumerable<htmltemplate> ReadAll()
        {
            // Use locks to avoid database collitions
            lock (collisionLock)
            {
                var query = from x in db.Table<htmltemplate>()
                            select x;
                return query.AsEnumerable();
            }
        }

        public htmltemplate ReadByPK(string PK)
        {
            lock (collisionLock)
            {
                return db.Table<htmltemplate>().FirstOrDefault(x => x.htmltemplate_id == PK);
            }
        }

        public int DeleteByPK(string PK)
        {
            lock (collisionLock)
            {
                return db.Delete<htmltemplate>(PK);
            }
        }

    }
}
