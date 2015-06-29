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
    public partial class DocumentSearchJA : Page
    {
        private DocumentSearchJAViewModel DocumentSearchViewModel;

        public DocumentSearchJA()
        {
            InitializeComponent();
            DocumentSearchViewModel = new DocumentSearchJAViewModel();
            DocumentSearchViewModel.GroupID = 1;
            this.DataContext = DocumentSearchViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DocumentSearchViewModel.LoadData();
        }

    }
}
