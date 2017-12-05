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
using XFWebviewLib.Interface;

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

        public SplashPageViewModel(INavigationService navigationService, IFolderPath floderpath)
         : base(navigationService, floderpath)
        {
        }


        public override async void OnNavigatingTo(NavigationParameters parameters)
        {
            #region 1.檢查是否可以連上桃園
            try
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

                    }
            }
            catch (Exception ex)
            {
            }

            #endregion
        }
        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            try
            {
                using (UserDialogs.Instance.Loading("與伺服器連線中...", null, null, true, MaskType.Black))
                {
                    //進入主選單
                    var navpara = new NavigationParameters();
                    navpara.Add("TempSyncAppList", TempSyncAppList);
                    await Task.Delay(1);
                    await NavigationService.NavigateAsync("app:///NavigationPage/MainPage", navpara);
                }
            }
            catch (Exception ex)
            {
                var toastConfig = new ToastConfig("目前主機無法連線，請稍後再試");
                toastConfig.SetDuration(3000);
                UserDialogs.Instance.Toast(toastConfig);
            }

        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {

        }
    }
}
