using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Model
{
    public class appfunc
    {
        [PrimaryKey]
        public string appfunc_id { get; set; }
        public string appfunc_pid { get; set; }
        public string appfunc_type { get; set; }
        public string appfunc_name { get; set; }
        public string appfunc_url { get; set; }
        public int appfunc_order { get; set; }
        public string appfunc_isactive { get; set; }
        public string appfunc_files { get; set; }
        public DateTime create_date { get; set; }
        public string create_user { get; set; }
        public DateTime update_date { get; set; }
        public string update_user { get; set; }
    }
}
