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
    public partial class FileTypeManager : Page
    {
        public FileTypeManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.FileTypeManagerViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            FileTypeManagerViewModel lFileTypeManagerViewModel = this.DataContext as FileTypeManagerViewModel;
            lFileTypeManagerViewModel.LoadData();
        }

    }
}
