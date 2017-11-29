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
        }

        #endregion
    }
}