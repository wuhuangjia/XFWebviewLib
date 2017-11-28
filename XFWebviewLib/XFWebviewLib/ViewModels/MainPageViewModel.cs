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

namespace XFWebviewLib.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region fields
        AppFunDAO db;
        appfunc _appfunc;
        #endregion

        #region Propertys
        public string PageTemplate
        {
            get;
            set;
        }

        public string Baseurl
        {
            get;
            set;
        }

        public appfunc AppFuncObj
        {
            get
            {
                if (_appfunc == null)
                {
                    _appfunc = new appfunc();
                }
                return _appfunc;
            }
            set
            {
                _appfunc = value;
            }
        }

        #endregion

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
        }

        public async void DownloadAppFuncFileAsync(string FloderName, string MutiFileName)
        {
            var listfile = new List<string>(MutiFileName.Split(','));

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(FloderName, CreationCollisionOption.OpenIfExists);
            listfile.ForEach(async filename =>
            {
                IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

                using (var fooFileStream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    using (HttpClientHandler handle = new HttpClientHandler())
                    {
                        // 建立 HttpClient 物件
                        using (HttpClient client = new HttpClient(handle))
                        {
                            // 取得指定 URL 的 Stream
                            var url = $"{AppData.WebBaseUrl}files/appfunc_id/{FloderName}/{filename}";
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

        public async void InitAppfuncHtmlAsync(string FloderName, string PageFileName, string MutiFileName)
        {
            var listfile = new List<string>(MutiFileName.Split(','));

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(FloderName, CreationCollisionOption.OpenIfExists);

            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                listfile.ForEach(async filename =>
                {
                    IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                    string tmppath = DependencyService.Get<IFloderPath>().GetTempDirectory();
                    IFolder targetfloder = await FileSystem.Current.GetFolderFromPathAsync(tmppath);
                    PCLStorageExtensions.CopyFileTo(file, targetfloder);
                });
            }

        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {           

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            //讀取資料庫
            db = new AppFunDAO();
            //取出首頁
            AppFuncObj = db.ReadByName("首頁");
            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                Baseurl = DependencyService.Get<IFloderPath>().GetTempDirectory();
            }
            else
            {
                Baseurl = $"file://{DependencyService.Get<IFloderPath>().GetPath(Environment.SpecialFolder.Personal, AppFuncObj.appfunc_id)}/";

            }

        }

    }
}
