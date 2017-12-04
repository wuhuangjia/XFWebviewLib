using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Model
{
    public class syncapp
    {
        [PrimaryKey]
        public string syncapp_id { get; set; }
        public string syncapp_table { get; set; }
        public string syncapp_filter { get; set; }
        public DateTime create_date { get; set; }
        public string create_user { get; set; }
        public DateTime update_date { get; set; }
        public string update_user { get; set; }

    }
}
