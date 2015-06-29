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
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using DocumentManager.Views;

namespace DocumentManager.ViewModels
{
    public class LogonUserViewModel : NotifyPropertyChanged
    {
        public string UserName
        {
            get
            {
                App app = Application.Current as App;
                return app.MainPageViewModel.User.UserName;
            }
        }

        public ICommand OnLogout { get; private set; }
        public ICommand OnModifyPassword { get; private set; }

        public LogonUserViewModel()
        {
            OnLogout = new DelegateCommand(onLogout);
            OnModifyPassword = new DelegateCommand(onModifyPassword);
        }

        private void onLogout()
        {
            WebContext.Current.Authentication.Logout(Logout_Complete, null);
        }

        private void Logout_Complete(LogoutOperation aLogoutOperation)
        {
            if (!aLogoutOperation.User.Identity.IsAuthenticated)
            {
                App app = Application.Current as App;
                app.SuccessLogout();
            }
        }

        private void onModifyPassword()
        {
            ModifyPasswordWindow lModifyPasswordWindow = new ModifyPasswordWindow();
            lModifyPasswordWindow.Show();
        }
    }
}
