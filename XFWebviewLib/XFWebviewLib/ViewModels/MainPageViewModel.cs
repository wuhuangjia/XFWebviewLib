using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFWebviewLib.DAO;

namespace XFWebviewLib.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public string dbpath { get; set; }
        ContentTemplateDAO db;
        public MainPageViewModel(INavigationService navigationService) 
            : base (navigationService)
        {
            Title = "Main Page";
            db = new ContentTemplateDAO();
            dbpath = $"路徑: {db.DBPath}";
        }
    }
}
