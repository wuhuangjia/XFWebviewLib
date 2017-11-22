using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Interface
{
    public interface IDatabaseConnection
    {
        SQLiteConnection DbConnection(string dbName);
    }
}
