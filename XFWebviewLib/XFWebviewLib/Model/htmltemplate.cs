using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Model
{
    public class htmltemplate
    {
        [PrimaryKey]
        public string htmltemplate_id { get; set; }
        public string appfunc_id { get; set; }
        public string htmltemplate_type { get; set; }
        public string htmltemplate_subject { get; set; }
        public string htmltemplate_content { get; set; }
        public DateTime create_date { get; set; }
        public string create_user { get; set; }
        public DateTime update_date { get; set; }
        public string update_user { get; set; }
    }
}
