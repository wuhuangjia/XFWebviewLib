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
    public class AppFunDAO
    {
        public string DBPath { get; set; }
        private SQLiteConnection db;
        private static object collisionLock = new object();
        public ObservableCollection<appfunc> appfuncs { get; set; }

        public AppFunDAO()
        {
            db = DependencyService.Get<IDatabaseConnection>().DbConnection(AppData.DBName);
            DBPath = db.DatabasePath;
            db.CreateTable<appfunc>();
            this.appfuncs = new ObservableCollection<appfunc>(db.Table<appfunc>());
            // If the table is empty, initialize the collection
            if (!db.Table<appfunc>().Any())
            {
                appfunc o = new appfunc();
                //o.appfunc_id = Guid.NewGuid().ToString();
                o.appfunc_id = "1";
                o.appfunc_pid = string.Empty;
                o.appfunc_type = "webview";
                o.appfunc_name = "首頁";
                o.appfunc_url = "mainpage.html";
                o.appfunc_order = 0;
                o.appfunc_isactive = "1";
                o.appfunc_files = "mainpage.html,style.css";
                o.update_date = DateTime.Now;
                o.create_date = DateTime.Now;
                this.Create(o);
            }
        }

        public int Create(appfunc entity)
        {
            lock (collisionLock)
            {
                return db.Insert(entity);
            }
        }

        public int Update(appfunc entity)
        {
            lock (collisionLock)
            {
                return db.Update(entity);
            }
        }

        public IEnumerable<appfunc> ReadAll()
        {
            // Use locks to avoid database collitions
            lock (collisionLock)
            {
                var query = from x in db.Table<appfunc>()
                            select x;
                return query.AsEnumerable();
            }
        }

        public appfunc ReadByPK(string PK)
        {
            lock (collisionLock)
            {
                return db.Table<appfunc>().FirstOrDefault(x => x.appfunc_id == PK);
            }
        }

        public appfunc ReadByName(string appfunc_name)
        {
            lock (collisionLock)
            {
                return db.Table<appfunc>().FirstOrDefault(x => x.appfunc_name == appfunc_name);
            }
        }

        public int DeleteByPK(string PK)
        {
            lock (collisionLock)
            {
                return db.Delete<appfunc>(PK);
            }
        }

    }
}
