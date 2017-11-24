﻿using Prism.Commands;
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

namespace XFWebviewLib.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region fields
        ContentTemplateDAO db;

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
        #endregion

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
            //1.從後端下載樣板所需要的外部資源，例如：css js 圖檔等，並存到自訂的資料夾 geometry_tmp
            //2.ios 的 wkwebview 需要再將所下載的外部檔案於執行時複製到 tmp 資料夾

            SaveCss();
            //ReadCss();
            db = new ContentTemplateDAO();

            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                Baseurl = DependencyService.Get<IFloderPath>().GetTempDirectory();
            }
            else
            {
                Baseurl = $"file://{DependencyService.Get<IFloderPath>().GetPath(Environment.SpecialFolder.Personal, "MySubFolder")}/";
            }

            var htmltemplateObj = db.ReadByPK("1");
            if (htmltemplateObj != null)
            {
                PageTemplate = htmltemplateObj.htmltemplate_content;
            }
        }

        async void  SaveCss()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("MySubFolder",CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync("style.css",CreationCollisionOption.ReplaceExisting);
            var strcss = "html,body {color:green;}";
            await file.WriteAllTextAsync(strcss);
            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                string tmppath = DependencyService.Get<IFloderPath>().GetTempDirectory();
                IFolder targetfloder = await FileSystem.Current.GetFolderFromPathAsync(tmppath);
                PCLStorageExtensions.CopyFileTo(file, targetfloder);
            }


        }

        async void ReadCss()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile sourceFile = await FileSystem.Current.GetFileFromPathAsync(Path.Combine(rootFolder.Path, "MySubFolder", "style.css"));
            string tmppath = DependencyService.Get<IFloderPath>().GetTempDirectory();
            IFolder targetfloder = await FileSystem.Current.GetFolderFromPathAsync(tmppath);
            PCLStorageExtensions.CopyFileTo(sourceFile, targetfloder);
        }
    }
}
