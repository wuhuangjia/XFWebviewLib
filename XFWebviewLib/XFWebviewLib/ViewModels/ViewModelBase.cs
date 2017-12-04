using Plugin.Connectivity;
using Plugin.Geolocator;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XFWebviewLib.Helper;
using XFWebviewLib.Model;

namespace XFWebviewLib.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        #region Fields
        protected INavigationService NavigationService { get; private set; }
        #endregion

        #region Propertys
        public string Title { get; set; }
        public bool IsServerAvailable { get; set; }

        public bool IsSync { get; set; }

        private List<syncapp> _tempsyncapplist;

        public List<syncapp> TempSyncAppList
        {
            get {
                if(_tempsyncapplist == null)
                {
                    _tempsyncapplist = new List<syncapp>();
                }
                return _tempsyncapplist; }
            set { _tempsyncapplist = value; }
        }

        #endregion

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

        #region 檢查頁面是否過期

        #endregion

    }
}
