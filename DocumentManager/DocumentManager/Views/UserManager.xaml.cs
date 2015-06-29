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
using System.Windows.Navigation;
using DocumentManager.ViewModels;

namespace DocumentManager.Views
{
    public partial class UserManager : Page
    {
        public UserManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.UserManagerViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserManagerViewModel lUserManagerViewModel = this.DataContext as UserManagerViewModel;
            lUserManagerViewModel.LoadData();
        }

    }
}
