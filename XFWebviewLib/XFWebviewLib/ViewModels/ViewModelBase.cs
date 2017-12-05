using PCLStorage;
using Plugin.Connectivity;
using Plugin.DeviceInfo;
using Plugin.Geolocator;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XFWebviewLib.Helper;
using XFWebviewLib.Infrastructure;
using XFWebviewLib.Interface;
using XFWebviewLib.Model;

namespace XFWebviewLib.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        #region Fields
        #endregion

        #region Propertys
        protected INavigationService NavigationService { get; private set; }
        protected IFolderPath FolderPath { get; private set; }
        protected string Title { get; set; }
        protected bool IsServerAvailable { get; set; }

        protected bool IsSync { get; set; }

        private List<syncapp> _tempsyncapplist;

        protected List<syncapp> TempSyncAppList
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

        public ViewModelBase(INavigationService navigationService, IFolderPath folderpath)
        {
            NavigationService = navigationService;
            FolderPath = folderpath;
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

        #region 取得Webview本地的Baseurl
        public async Task<string> GetBaseurlAsync(string appfunc_id, string appfunc_url)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder appfuncfolder = await rootFolder.CreateFolderAsync("appfunc_id", CreationCollisionOption.OpenIfExists);
            IFolder folder = await appfuncfolder.CreateFolderAsync(appfunc_id, CreationCollisionOption.OpenIfExists);
            string Baseurl = string.Empty;
            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                Baseurl = FolderPath.GetTempDirectory();
                var flist = folder.GetFilesAsync();
                string tmppath = Baseurl;
                IFolder targetfloder = await FileSystem.Current.GetFolderFromPathAsync(tmppath);
                flist.Result.ToList().ForEach(f =>
                {
                    PCLStorageExtensions.CopyFileTo(f, targetfloder);
                });
            }
            else
            {
                //Baseurl = $"file://{FolderPath.GetPath(Environment.SpecialFolder.Personal, appfunc_id)}/";
                Baseurl = $"file://{folder.Path}/";
            }
            return Baseurl;
        }

        #endregion

        #region 取得Webview讀取本地的內容
        public async Task<string> GetAppfuncHtmlAsync(string appfunc_id, string appfunc_url)
        {
            string wvSource = string.Empty;
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder appfuncfolder = await rootFolder.CreateFolderAsync("appfunc_id", CreationCollisionOption.OpenIfExists);
            IFolder folder = await appfuncfolder.CreateFolderAsync(appfunc_id, CreationCollisionOption.OpenIfExists);
            wvSource = await Utilities.ReadFileAsync(folder.Path, appfunc_url);
            return wvSource;
        }
        #endregion


        #region 將遠端網頁內容緩存
        public async void DownloadAppFuncFileAsync(string appfunc_id, string MutiFileName, string NoFileNameWebUrl)
        {
            var listfile = new List<string>(MutiFileName.Split(','));

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder appfuncfolder = await rootFolder.CreateFolderAsync("appfunc_id", CreationCollisionOption.OpenIfExists);
            IFolder folder = await appfuncfolder.CreateFolderAsync(appfunc_id, CreationCollisionOption.OpenIfExists);

            listfile.ForEach(async filename =>
            {
                IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (var fooFileStream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    using (HttpClientHandler handle = new HttpClientHandler())
                    {
                        // 建立 HttpClient 物件
                        using (HttpClient client = new HttpClient(handle))
                        {
                            // 取得指定 URL 的 Stream
                            var url = $"{NoFileNameWebUrl}/{filename}";

                            using (var fooStream = await client.GetStreamAsync(url))
                            {
                                // 將網路的檔案 Stream 複製到本機檔案上
                                fooStream.CopyTo(fooFileStream);
                            }
                        }
                    }
                }
            });

        }

        #endregion
    }
}
