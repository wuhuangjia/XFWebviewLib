using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Infrastructure
{
    public static class AppData
    {
        public static string WebBaseUrl = "https://manage-scenedraw.azurewebsites.net/";
        public static string APIKey { get; set; } = "ea03fcb8c47822bce772cf6c07d0ebbb";
        public static string DBName { get; set; } = "XFWebviewLib.db3";
        public static string SyncAppJsonUrl { get; set; } = "https://manage-scenedraw.azurewebsites.net/files/syncapp_id/syncapp.txt"; 
    }
}
