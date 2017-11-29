using Acr.UserDialogs;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFWebviewLib.Helper
{
    public static class Utilities
    {
        #region 將點擊圖片加上放大縮小動畫，預設 CubicInOut + BounceOut，須放置於 Code Behiend
        /// <summary>
        /// 取得動畫的效果
        /// </summary>
        /// <param name="easingName">動畫名稱</param>
        /// <returns></returns>
        private static Easing GetEasing(string easingName)
        {
            switch (easingName)
            {
                case "BounceIn": return Easing.BounceIn;
                case "BounceOut": return Easing.BounceOut;
                case "CubicInOut": return Easing.CubicInOut;
                case "CubicOut": return Easing.CubicOut;
                case "Linear": return Easing.Linear;
                case "SinIn": return Easing.SinIn;
                case "SinInOut": return Easing.SinInOut;
                case "SinOut": return Easing.SinOut;
                case "SpringIn": return Easing.SpringIn;
                case "SpringOut": return Easing.SpringOut;
                default: throw new ArgumentException(easingName + " is not valid");
            }
        }
        /// <summary>
        /// 將點擊圖片加上放大縮小動畫，預設CubicInOut+BounceOut須放置於Code Behiend
        /// </summary>
        /// <param name="Element">要加上動畫的元素</param>
        /// <param name="EasingName1">開始特效名稱</param>
        /// <param name="EasingName2">結束特效名稱</param>
        public async static void ImageTappedScaleToAnimation(VisualElement Element, string EasingName1 = "CubicInOut", string EasingName2 = "BounceOut")
        {
            if (!Element.AnimationIsRunning("ScaleTo"))
            {
                await Element.ScaleTo(1.2, 250, GetEasing(EasingName1));
                await Element.ScaleTo(1, 500, GetEasing(EasingName2));
            }
        }

        #endregion

        #region 各式提醒，載入，alert等
        //使用方式可參考：https://github.com/aritchie/userdialogs/blob/master/src/Samples/Samples/ViewModels/ToastsViewModel.cs
        #region Tost
        /// <summary>
        /// 於Android底部顯示snackbar
        /// </summary>
        /// <param name="Msg">訊息內容</param>
        /// <param name="Sec">顯示時間(秒)</param>
        public static void ShowTost(string Msg, int Sec = 3)
        {
            var toastConfig = new ToastConfig(Msg);
            toastConfig.SetDuration(Sec * 1000);
            UserDialogs.Instance.Toast(toastConfig);
        }

        #endregion

        #region AlertAsync
        /// <summary>
        /// 顯示 Alert
        /// </summary>
        /// <param name="Title">標題</param>
        /// <param name="Msg">內容</param>
        /// <param name="okTxt">確認鍵文字</param>
        public async static void ShowAlertAsync(string Title, string Msg, string okTxt)
        {
            await UserDialogs.Instance.AlertAsync(Msg, Title, okTxt);
        }

        #endregion
        #endregion

        #region 傳入資料夾及檔名，回傳字串
        public static async Task<string> ReadFileAsync(string FloderName, string FileName)
        {
            string result = string.Empty;
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(FloderName, CreationCollisionOption.OpenIfExists);
            var e = await folder.CheckExistsAsync(FileName);
            if (e == ExistenceCheckResult.FileExists) 
            {
                IFile file = await folder.GetFileAsync(FileName);
                result = await file.ReadAllTextAsync();
            }

            return result;
        }
        #endregion

    }
}
