using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFWebviewLib.Views
{
	public partial class MainPage : ContentPage
	{
        public MainPage()
        {
            InitializeComponent();
            this.hybridWebView.Uri = new Uri("http://quasar-framework.org/quasar-play/android/index.html#/showcase");
        }
	}
}