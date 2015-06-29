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
using DocumentManager.Model.SearchEntities;
using System.Collections.ObjectModel;
using DocumentManager.Model.Entities;

namespace DocumentManager.Views
{
    public partial class AddSearchWindow : ChildWindow
    {
        private AddSearchWindowViewModel AddSearchWindowViewModel;

        public AddSearchWindow(int aGroupID, bool aIsFirst, ObservableCollection<TaxPayerTypeEntity> aTaxPayerTypeList)
        {
            InitializeComponent();
            AddSearchWindowViewModel = new AddSearchWindowViewModel(this, aGroupID, aIsFirst, aTaxPayerTypeList);
            this.DataContext = AddSearchWindowViewModel;
        }

        public TaxPayerSearchEntity GetTaxPayerSearchEntity()
        {
            return AddSearchWindowViewModel.GetTaxPayerSearchEntity();
        }

        public void SetTaxPayerSearchEntity(TaxPayerSearchEntity aTaxPayerSearchEntity)
        {
            AddSearchWindowViewModel.SetTacPayerSearchEntity(aTaxPayerSearchEntity);
        }
    }
}

