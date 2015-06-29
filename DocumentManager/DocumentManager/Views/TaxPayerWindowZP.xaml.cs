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
using System.Collections.ObjectModel;

namespace DocumentManager.Views
{
    public partial class TaxPayerWindowZP : ChildWindow
    {
        public TaxPayerWindowZP(TaxPayerWindowType aTaxPayerWindowType
            , TaxPayerEntity aTaxPayerEntity
            , ObservableCollection<TaxPayerTypeEntity> aTaxPayerTypeEntityList
            , int aGroupID)
        {
            InitializeComponent();
            this.DataContext = new TaxPayerWindowViewModel(this, aTaxPayerWindowType, aTaxPayerEntity, aTaxPayerTypeEntityList, aGroupID);
        }
    }
}

