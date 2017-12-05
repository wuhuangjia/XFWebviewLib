using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFWebviewLib.DAO;
using PCLStorage;
using Xamarin.Forms;
using XFWebviewLib.Interface;
using System.IO;
using XFWebviewLib.Helper;
using Plugin.DeviceInfo;
using System.Threading.Tasks;
using System.Net.Http;
using XFWebviewLib.Infrastructure;
using Acr.UserDialogs;
using XFWebviewLib.Model;
using Prism.Events;
using Xam.Plugin.WebView.Abstractions;
using Xam.Plugin.WebView.Abstractions.Enumerations;

namespace XFWebviewLib.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region fields
        appfunc _appfunc;
        syncapp _syncapp;
        SyncAppDAO _syncapp_db;
        AppFunDAO _appfunc_db;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region Propertys

        public AppFunDAO appfunc_db
        {
            get
            {
                if (_appfunc_db == null)
                {
                    _appfunc_db = new AppFunDAO();
                }
                return _appfunc_db;
            }
            set { _appfunc_db = value; }
        }

        public SyncAppDAO syncapp_db
        {
            get
            {
                if (_syncapp_db == null)
                {
                    _syncapp_db = new SyncAppDAO();
                }
                return _syncapp_db;
            }
            set { _syncapp_db = value; }
        }

        public string NaviUrl { get; set; }
        public WebViewContentType wvContentType { get; set; }
        public string wvSource { get; set; }
        public string wvBaseurl { get; set; }
        #endregion

        #region Command
        private DelegateCommand _Navitotest2;
        public DelegateCommand Navitotest2Command =>
            _Navitotest2 ?? (_Navitotest2 = new DelegateCommand(ExecuteNavitotest2Command));

        async void ExecuteNavitotest2Command()
        {
            try
            {
                await NavigationService.NavigateAsync(NaviUrl);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        public MainPageViewModel(INavigationService navigationService, IFolderPath floderpath)
            : base(navigationService, floderpath)
        {
            Title = "Main Page";
            _syncapp = syncapp_db.ReadByTableName("appfunc", "");
            _appfunc = appfunc_db.ReadByName("首頁");
        }



        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            using (UserDialogs.Instance.Loading("與伺服器連線中...", null, null, true, MaskType.Black))
            {
                InitAppfuncHtmlAsync();
            }
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("TempSyncAppList"))
            {
                TempSyncAppList = (List<syncapp>)parameters["TempSyncAppList"];
            }
        }

        public async void InitAppfuncHtmlAsync()
        {
            if (_syncapp != null)
            {
                var remote = TempSyncAppList.FirstOrDefault(x => x.syncapp_table == _syncapp.syncapp_table && x.syncapp_filter == _syncapp.syncapp_filter);
                if (remote.update_date > _syncapp.update_date)
                {
                    //如果遠端的更新日期大於本地端，則由網路取得並且緩存一份在本地
                    //切換webview為internet
                    wvContentType = WebViewContentType.Internet;
                    wvSource = $"{AppData.WebBaseUrl}files/appfunc_id/{_appfunc.appfunc_id}/{_appfunc.appfunc_url}";
                    DownloadAppFuncFileAsync(_appfunc.appfunc_id, _appfunc.appfunc_files, wvSource);
                    _syncapp.update_date = remote.update_date;
                    syncapp_db.Update(_syncapp);
                }
                else
                {
                    //讀取本地緩存頁面
                    wvContentType = WebViewContentType.StringData;
                    wvBaseurl = await GetBaseurlAsync(_appfunc.appfunc_id, _appfunc.appfunc_url);
                    wvSource = await GetAppfuncHtmlAsync(_appfunc.appfunc_id, _appfunc.appfunc_url);
                }
            }
            else
            {
                //_syncapp如果是null，表示這個功能還沒有緩存過，必須從網路取得

                //切換webview為internet
                wvContentType = WebViewContentType.Internet;
                wvSource = $"{AppData.WebBaseUrl}files/appfunc_id/{_appfunc.appfunc_id}/{_appfunc.appfunc_url}";
                var NoFileNameWebUrl = $"{AppData.WebBaseUrl}files/appfunc_id/{_appfunc.appfunc_id}/";
                DownloadAppFuncFileAsync(_appfunc.appfunc_id, _appfunc.appfunc_files, NoFileNameWebUrl);
                var remote = TempSyncAppList.FirstOrDefault(x => x.syncapp_table == "appfunc");
                syncapp_db.Create(remote);

            }

        }

    }
}
