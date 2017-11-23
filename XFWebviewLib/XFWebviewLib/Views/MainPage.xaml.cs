using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFWebviewLib.ViewModels;

namespace XFWebviewLib.Views
{
	public partial class MainPage : ContentPage
	{
        MainPageViewModel _MainPageViewModel;
        public MainPage()
        {
            InitializeComponent();
            _MainPageViewModel = this.BindingContext as MainPageViewModel;
            var htmlSource = new HtmlWebViewSource();
            htmlSource.BaseUrl = _MainPageViewModel.Baseurl;
            htmlSource.Html = _MainPageViewModel.PageTemplate;
            this.hybridWebView.Source = htmlSource;
        }
	}
}