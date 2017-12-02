using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Model
{
    public class WebViewtoVmEventArgs : EventArgs
    {
        public string Message { get; set; }

        public WebViewtoVmEventArgs(string message)
        {
            Message = message;
        }
    }
}
