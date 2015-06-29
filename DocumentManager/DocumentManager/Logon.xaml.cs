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

namespace DocumentManager
{
    public partial class Logon : UserControl
    {
        public Logon()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.LogonViewModel;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogonViewModel lLogonViewModel = this.DataContext as LogonViewModel;
                lLogonViewModel.LoginInfo.UserName = username.Text;
                lLogonViewModel.LoginInfo.Password = password.Password;
                lLogonViewModel.onLogin();
            }
        }
    }
}
