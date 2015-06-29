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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using DocumentManager.ViewModels;

namespace DocumentManager
{
    public partial class Home : Page
    {
        HomeViewModel homeViewModel;

        public Home()
        {
            InitializeComponent();
            homeViewModel = new HomeViewModel();
            this.DataContext = homeViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            homeViewModel.LoadData();
        }
    }
}