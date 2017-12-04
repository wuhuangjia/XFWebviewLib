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
    public class SyncAppDAO
    {
        public string DBPath { get; set; }
        private SQLiteConnection db;
        private static object collisionLock = new object();
        public ObservableCollection<syncapp> syncapps { get; set; }

        public SyncAppDAO()
        {
            db = DependencyService.Get<IDatabaseConnection>().DbConnection(AppData.DBName);
            DBPath = db.DatabasePath;
            db.CreateTable<syncapp>();
        }

        private int Create(syncapp entity)
        {
            lock (collisionLock)
            {
                return db.Insert(entity);
            }
        }

        private int Update(syncapp entity)
        {
            lock (collisionLock)
            {
                return db.Update(entity);
            }
        }

        public IEnumerable<syncapp> ReadAll()
        {
            // Use locks to avoid database collitions
            lock (collisionLock)
            {
                var query = from x in db.Table<syncapp>()
                            select x;
                return query.AsEnumerable();
            }
        }

        public syncapp ReadByPK(string PK)
        {
            lock (collisionLock)
            {
                return db.Table<syncapp>().FirstOrDefault(x => x.syncapp_id == PK);
            }
        }

        public int DeleteByPK(string PK)
        {
            lock (collisionLock)
            {
                return db.Delete<syncapp>(PK);
            }
        }

    }
}
