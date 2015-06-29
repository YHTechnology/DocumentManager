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
using DocumentManager.Model.Entities;

namespace DocumentManager.Views
{
    public partial class LinkFileWindow : ChildWindow
    {
        public LinkFileViewModel linkFileViewModel;
        public LinkFileWindow(Dictionary<int, FileTypeEntity> aFileTypeDictionary)
        {
            InitializeComponent();
            linkFileViewModel = new LinkFileViewModel(this, aFileTypeDictionary);
            this.DataContext = linkFileViewModel;
        }

        private void ChildWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            linkFileViewModel.LoadData();
        }
    }
}

