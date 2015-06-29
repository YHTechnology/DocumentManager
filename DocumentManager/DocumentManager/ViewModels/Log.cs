using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DocumentManager.Web;

namespace DocumentManager.ViewModels
{
    public class Log
    {
        public static void AddLog(DocumentManagerDomainContext documentManagerContext, string aString)
        {
            App lApp = Application.Current as App;

            string lUser = lApp.MainPageViewModel.User.UserName;
            string logString = lUser + " add " + aString;

            DocumentManager.Web.Model.systemlog lSystemlog = new DocumentManager.Web.Model.systemlog();
            lSystemlog.system_log = logString;
            documentManagerContext.systemlogs.Add(lSystemlog);
        }

        public static void ModifyLog(DocumentManagerDomainContext documentManagerContext, string aString)
        {
            App lApp = Application.Current as App;

            string lUser = lApp.MainPageViewModel.User.UserName;
            string logString = lUser + " modify " + aString;

            DocumentManager.Web.Model.systemlog lSystemlog = new DocumentManager.Web.Model.systemlog();
            lSystemlog.system_log = logString;
            documentManagerContext.systemlogs.Add(lSystemlog);
        }

        public static void DeleteLog(DocumentManagerDomainContext documentManagerContext, string aString)
        {
            App lApp = Application.Current as App;

            string lUser = lApp.MainPageViewModel.User.UserName;
            string logString = lUser + " delete " + aString;

            DocumentManager.Web.Model.systemlog lSystemlog = new DocumentManager.Web.Model.systemlog();
            lSystemlog.system_log = logString;
            documentManagerContext.systemlogs.Add(lSystemlog);
        }
    }
}
