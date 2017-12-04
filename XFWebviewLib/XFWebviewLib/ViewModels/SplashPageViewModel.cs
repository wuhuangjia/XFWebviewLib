using Acr.UserDialogs;
using PCLStorage;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XFWebviewLib.Helper;
using XFWebviewLib.Infrastructure;
using XFWebviewLib.Model;
using Newtonsoft.Json;
using XFWebviewLib.DAO;

namespace XFWebviewLib.ViewModels
{
	public class SplashPageViewModel : ViewModelBase
	{
        #region fields
        SyncAppDAO _syncapp_db;

        #endregion

        #region Propertys
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

        #endregion

        public SplashPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }


        public override void OnNavigatingTo(NavigationParameters parameters)
        {
        }
        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            #region 1.檢查是否可以連上桃園
            try
            {
                using (UserDialogs.Instance.Loading("與伺服器連線中...", null, null, true, MaskType.Black))
                {
                    IsServerAvailable = await IsHostConnectAvailable(AppData.WebBaseUrl);
                    if (IsServerAvailable)
                    {
                        #region 2.同步資料
                        //讀取遠端syncapp
                        List<syncapp> listremotesync = new List<syncapp>();
                        string SyncAppJson = await Utilities.GetStringUseGetByUrlAsync(AppData.SyncAppJsonUrl);
                        listremotesync = JsonConvert.DeserializeObject<List<syncapp>>(SyncAppJson);

                        //讀取本機syncapp
                        List<syncapp> listlocalesync = new List<syncapp>();
                        listlocalesync = syncapp_db.ReadAll().ToList();

                        //比對差異
                        listremotesync.ForEach(remote =>
                        {
                            if (listlocalesync.Contains<syncapp>(remote))
                            {
                                var local = listlocalesync.FirstOrDefault(x => x.syncapp_id == remote.syncapp_id);
                                if (remote.update_date > local.update_date)
                                {
                                    //暫存要同步的內容list
                                    //預先將更新日期調整為跟遠端的相同
                                    local.update_date = remote.update_date;
                                    TempSyncAppList.Add(local);
                                }
                            }
                            else
                            {
                                TempSyncAppList.Add(remote);
                            }

                        });
                        #endregion

                        await NavigationService.NavigateAsync("app:///NavigationPage/MainPage");
                    }
                }
            }
            catch (Exception ex)
            {
                var toastConfig = new ToastConfig("目前主機無法連線，請稍後再試");
                toastConfig.SetDuration(3000);
                UserDialogs.Instance.Toast(toastConfig);
            }

            #endregion

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
                            //var url = $"{AppData.WebBaseUrl}files/appfunc_id/{FloderName}/{filename}";
                            var url = "https://card.tycg.gov.tw/ap_mobile/index.aspx";
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

    }
}
