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

namespace XFWebviewLib.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region fields
        appfunc _appfunc;
        AppFunDAO _appfunc_db;
        private readonly IEventAggregator _eventAggregator;
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


        #endregion

        #region Command
        private DelegateCommand _Navitotest2;
        public DelegateCommand Navitotest2Command =>
            _Navitotest2 ?? (_Navitotest2 = new DelegateCommand(ExecuteNavitotest2Command));

        async void ExecuteNavitotest2Command()
        {
            try
            {
                await NavigationService.NavigateAsync("test2Page");

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            Title = "Main Page";
            AppFuncObj = appfunc_db.ReadByName("首頁");
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<WebViewtoVmEvent>().Subscribe(x =>
            {
                if (!string.IsNullOrEmpty(x.Message))
                {
                    try
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await NavigationService.NavigateAsync("test2Page");
                        });

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            });
        }

        public async void DownloadAppFuncFileAsync(string FloderName, string MutiFileName)
        {
            var listfile = new List<string>(MutiFileName.Split(','));

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(FloderName, CreationCollisionOption.OpenIfExists);

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

        public async void InitAppfuncHtmlAsync()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(AppFuncObj.appfunc_id, CreationCollisionOption.OpenIfExists);

            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                Baseurl = DependencyService.Get<IFloderPath>().GetTempDirectory();
                var flist = folder.GetFilesAsync();
                string tmppath = Baseurl;
                IFolder targetfloder = await FileSystem.Current.GetFolderFromPathAsync(tmppath);
                flist.Result.ToList().ForEach( f =>
                {
                    PCLStorageExtensions.CopyFileTo(f, targetfloder);
                });
            }
            else
            {
                Baseurl = $"file://{DependencyService.Get<IFloderPath>().GetPath(Environment.SpecialFolder.Personal, AppFuncObj.appfunc_id)}/";
            }
            PageTemplate = await Utilities.ReadFileAsync(AppFuncObj.appfunc_id, AppFuncObj.appfunc_url);

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
            //DownloadAppFuncFileAsync(AppFuncObj.appfunc_id, AppFuncObj.appfunc_files);
        }


    }
}
