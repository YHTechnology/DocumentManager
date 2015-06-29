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
using DocumentManager.Model.Entities;
using DocumentManager.ViewModels;

namespace DocumentManager.Views
{
    public partial class AddProjectWindow : ChildWindow
    {
        public AddProjectWindow(TaxPayerEntity aTaxPayerEntity, TaxPayerEntity aNewTaxPayerEntity)
        {
            InitializeComponent();
            this.DataContext = new AddProjectWindowViewModel(this, aTaxPayerEntity, aNewTaxPayerEntity);
        }
    }
}

