using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DocumentManager.ViewModels;
using System.ServiceModel.DomainServices.Client.ApplicationServices;

namespace DocumentManager
{
    public partial class App : Application
    {
        public LogonViewModel LogonViewModel { get; set; }
        public MainPageViewModel MainPageViewModel { get; set; }
        public LogonUserViewModel LogonUserViewModel { get; set; }
        public UserManagerViewModel UserManagerViewModel { get; set; }
        public FileTypeManagerViewModel FileTypeManagerViewModel { get; set; }
        public TaxPayerTypeManagerViewModel TaxPayerTypeManagerViewModel { get; set; }
        public TaxPayerManagerViewModel TaxPayerManagerViewModel { get; set; }
        public DocumentManagerViewModel DocumentManagerViewModel { get; set; }
        
        public App()
        {
            LogonViewModel = new LogonViewModel();
            MainPageViewModel = new MainPageViewModel();
            LogonUserViewModel = new LogonUserViewModel();
            UserManagerViewModel = new UserManagerViewModel();
            FileTypeManagerViewModel = new FileTypeManagerViewModel();
            TaxPayerTypeManagerViewModel = new TaxPayerTypeManagerViewModel();
            TaxPayerManagerViewModel = new TaxPayerManagerViewModel();
            DocumentManagerViewModel = new DocumentManagerViewModel();

            this.Startup += this.Application_Startup;
            this.UnhandledException += this.Application_UnhandledException;
            this.CheckAndDownloadUpdateCompleted +=new CheckAndDownloadUpdateCompletedEventHandler(App_CheckAndDownloadUpdateCompleted);
            InitializeComponent();
            WebContext webContext = new WebContext();
            webContext.Authentication = new FormsAuthentication();
            this.ApplicationLifetimeObjects.Add(webContext);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (this.IsRunningOutOfBrowser)
            {
                CheckAndDownloadUpdateAsync();
            }
            WebContext.Current.Authentication.LoadUser(this.LoadUser_Completed, null);
            this.RootVisual = new Logon();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
                errorWin.Show();
            }
        }

        private void App_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)   
        {   
            if (e.UpdateAvailable) 
            {
                MessageBox.Show("有新版本，已下载安装. 请重新启动!");   
            }   
        }  

        public void SuccessLogon()
        {
            UserControl lCurrent = RootVisual as UserControl;
            lCurrent.Content = new MainPage();
            //this.RootVisual = MainPage;
        }

        public void SuccessLogout()
        {
            LogonViewModel.LoginInfo.UserName = "";
            LogonViewModel.LoginInfo.Password = "";

            UserControl lCurrent = RootVisual as UserControl;
            lCurrent.Content = new Logon();
            //this.RootVisual = Logon;
        }

        private void LoadUser_Completed(LoadUserOperation operation)
        {
            if (!operation.User.Identity.IsAuthenticated)
            {
                this.RootVisual = new Logon();
            }
            else
            {
                DocumentManager.Web.User lUser = operation.User.Identity as DocumentManager.Web.User;
                MainPageViewModel.User = lUser;
                this.RootVisual = new MainPage();
            }
        }
    }
}