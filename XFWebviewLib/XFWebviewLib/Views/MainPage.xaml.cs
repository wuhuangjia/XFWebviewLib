using PCLStorage;
using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFWebviewLib.Helper;
using XFWebviewLib.Interface;
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
        }

        #region Overrides of Page
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //_MainPageViewModel. DownloadAppFuncFileAsync(_MainPageViewModel.AppFuncObj.appfunc_id, _MainPageViewModel.AppFuncObj.appfunc_files);
            _MainPageViewModel. InitAppfuncHtmlAsync(_MainPageViewModel.AppFuncObj.appfunc_id, _MainPageViewModel.AppFuncObj.appfunc_url, _MainPageViewModel.AppFuncObj.appfunc_files);

            this.hybridWebView.BaseUrl= _MainPageViewModel.Baseurl;
            this.hybridWebView.Source = _MainPageViewModel.AppFuncObj.appfunc_url;
        }

        #endregion
    }
}