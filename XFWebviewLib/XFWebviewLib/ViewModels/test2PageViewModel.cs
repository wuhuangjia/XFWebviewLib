using Acr.UserDialogs;
using PCLStorage;
using Plugin.DeviceInfo;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using XFWebviewLib.DAO;
using XFWebviewLib.Helper;
using XFWebviewLib.Infrastructure;
using XFWebviewLib.Interface;
using XFWebviewLib.Model;

namespace XFWebviewLib.ViewModels
{
	public class test2PageViewModel : ViewModelBase
	{
        #region fields
        appfunc _appfunc;
        AppFunDAO _appfunc_db;
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
                await NavigationService.NavigateAsync("test2Page").ConfigureAwait(false);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        public test2PageViewModel(INavigationService navigationService, IFolderPath floderpath)
            : base(navigationService, floderpath)
        {
            Title = "Main Page";
            AppFuncObj = appfunc_db.ReadByName("首頁");

        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            using (UserDialogs.Instance.Loading("與伺服器連線中...", null, null, true, MaskType.Black))
            {

                //InitAppfuncHtmlAsync();
            }
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            //DownloadAppFuncFileAsync(AppFuncObj.appfunc_id, AppFuncObj.appfunc_files);
        }
    }
}
