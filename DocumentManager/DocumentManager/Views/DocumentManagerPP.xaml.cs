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
using System.Windows.Data;

namespace DocumentManager.Views
{
    public partial class DocumentManagerPP : Page
    {
        private DocumentManagerViewModel documentManagerViewModel;

        public DocumentManagerPP()
        {
            InitializeComponent();
            documentManagerViewModel = new DocumentManagerViewModel();
            documentManagerViewModel.GroupID =2;
            this.DataContext = documentManagerViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            documentManagerViewModel.SetCallBackLoaded(FinishLoaded);
            documentManagerViewModel.LoadData();
        }

        private void FinishLoaded()
        {
            foreach (CollectionViewGroup group in documentManagerViewModel.TaxpayerView.Groups)
            {
                TaxpayerGrid.CollapseRowGroup(group, true);
            }
        }

    }
}
