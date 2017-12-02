using PCLStorage;
using Plugin.DeviceInfo;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFWebviewLib.Helper;
using XFWebviewLib.Interface;
using XFWebviewLib.Model;
using XFWebviewLib.ViewModels;

namespace XFWebviewLib.Views
{
	public partial class MainPage : ContentPage
	{
        MainPageViewModel _MainPageViewModel;
        private readonly IEventAggregator _ea;
        public MainPage(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _MainPageViewModel = this.BindingContext as MainPageViewModel;
            _ea = eventAggregator;
            this.hybridWebView.AddLocalCallback("navitotest2", navitotest2);
        }

        void  navitotest2(string obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _MainPageViewModel.Navitotest2Command.CheckBeginExecute();
            });

        }
    }
}