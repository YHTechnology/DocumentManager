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
using DocumentManager.Views;
using System.ServiceModel.DomainServices.Client.ApplicationServices;

namespace DocumentManager.ViewModels
{
    public class MainPageViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.User user;

        public DocumentManager.Web.User User
        {
            get
            {
                return user;
            }
            set
            {
                if (value != user)
                {
                    user = value;

                    UpdateChanged("UserName");
                }

            }
        }

        public string UserName
        {
            get
            {
                return "当前用户:" + user.UserName;
            }
        }

        public Visibility DocManager
        {
            get
            {
                bool hasRight = false;
                User.RightDictionary.TryGetValue(100001, out hasRight);
                if(hasRight)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }

            }
        }

        public Visibility DocSearch
        {
            get
            {
                bool hasRight = false;
                User.RightDictionary.TryGetValue(100002, out hasRight);
                if(hasRight)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }

            }
        }

        public Visibility ProxyBook
        {
            get
            {
                bool hasRight = false;
                User.RightDictionary.TryGetValue(100003, out hasRight);
                if(hasRight)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }

            }
        }

        public Visibility SystemManager
        {
            get
            {
                bool hasRight = false;
                User.RightDictionary.TryGetValue(100004, out hasRight);
                if(hasRight)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }

            }
        }

        public ICommand OnModifyPassword { get; private set; }
        public ICommand OnLogout { get; private set; }

        public MainPageViewModel()
        {
            OnModifyPassword = new DelegateCommand(onModifyPassword);
            OnLogout = new DelegateCommand(onLogout);
        }

        private void onModifyPassword()
        {
            ModifyPasswordWindow lModifyPasswordWindow = new ModifyPasswordWindow();
            lModifyPasswordWindow.Show();
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
    }
}
