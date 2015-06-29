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
using DocumentManager.Model;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class LogonViewModel : NotifyPropertyChanged
    {
        public LoginInfo LoginInfo { get; set; }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        public Action<object> FinishLogon { get; set; }

        public ICommand OnLogin { get; private set; }

        public void onLogin()
        {
            if (LoginInfo.Validate())
            {
                IsBusy = true;
                WebContext.Current.Authentication.Login(
                                new LoginParameters(LoginInfo.UserName
                                                    , Cryptography.MD5CryptoServiceProvider.GetMd5String(LoginInfo.Password)
                                                    , LoginInfo.IsRemember, "")
                                , LoginOperation_Completed
                                , null);
            }
        }

        private void LoginOperation_Completed(LoginOperation loginOperation)
        {
            IsBusy = false;
            App app = Application.Current as App;


            if (loginOperation.LoginSuccess)
            {
                DocumentManager.Web.User lUser = loginOperation.User.Identity as DocumentManager.Web.User;

                if (lUser.ExpireDay == -1100)
                {
                    NotifyWindow notifyWindow = new NotifyWindow("注册错误", "注册信息丢失, 请与 管理员 联系");
                    notifyWindow.Show();
                    return;
                }
                else if (lUser.ExpireDay < 0)
                {
                    NotifyWindow notifyWindow = new NotifyWindow("应用程序已经过期", "应用程序已经过期" + lUser.ExpireDay.ToString() + "天, 请与 管理员 联系");
                    notifyWindow.Show();
                    return;
                }
                else if (lUser.ExpireDay < 30)
                {
                    NotifyWindow notifyWindow = new NotifyWindow("过期", "还有" + lUser.ExpireDay + "天， 应用程序即将过期，请与 管理员 联系");
                    notifyWindow.Show();
                }

                app.MainPageViewModel.User = lUser;
                app.SuccessLogon();
            }
            else
            {

                NotifyWindow notifyWindow = new NotifyWindow("用户名或密码错误", "用户名或密码错误");
                notifyWindow.Show();
                return;
            }
        }

        public LogonViewModel()
        {
            LoginInfo = new LoginInfo();
            OnLogin = new DelegateCommand(onLogin);
        }

    }
}
