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
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("MySubFolder", CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync("style.css", CreationCollisionOption.ReplaceExisting);
            var strcss = "html,body {color:green;}";
            await file.WriteAllTextAsync(strcss);

            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                string tmppath = DependencyService.Get<IFloderPath>().GetTempDirectory();
                IFolder targetfloder = await FileSystem.Current.GetFolderFromPathAsync(tmppath);
                PCLStorageExtensions.CopyFileTo(file, targetfloder);
            }

            this.hybridWebView.LoadContent(_MainPageViewModel.PageTemplate, _MainPageViewModel.Baseurl);
        }

        #endregion
    }
}