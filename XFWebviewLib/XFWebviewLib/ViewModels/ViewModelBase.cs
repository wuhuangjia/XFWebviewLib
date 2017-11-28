using Plugin.Connectivity;
using Plugin.Geolocator;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XFWebviewLib.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;

        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public virtual void Destroy()
        {
            
        }

        #region 檢查網路狀態
        protected bool IsConnectAvailable()
        {
            var pluginCrossConnectivity = CrossConnectivity.Current;
            return pluginCrossConnectivity.IsConnected;
        }
        #endregion

        #region 檢查伺服器狀態
        public async Task<bool> IsHostConnectAvailable(string url)
        {
            var connectivity = CrossConnectivity.Current;
            if (!connectivity.IsConnected)
                return false;

            var reachable = await connectivity.IsRemoteReachable(url);

            return reachable;
        }
        #endregion


        #region 檢查是否可以定位
        protected bool IsLocationAvailable()
        {
            var pluginCrossGeolocator = CrossGeolocator.Current;
            if (pluginCrossGeolocator.IsGeolocationAvailable && pluginCrossGeolocator.IsGeolocationEnabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

    }
}
