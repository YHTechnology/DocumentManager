using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DocumentManager.Model.Entities;

namespace DocumentManager.ViewModels
{
    public class AddProjectWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        public TaxPayerEntity OldTaxPayerEntity { get; set; }
        public TaxPayerEntity NewTaxPayerEntity { get; set; }

        public string Title { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public AddProjectWindowViewModel(ChildWindow aChildWindow
            , TaxPayerEntity aOldTaxPayerEntity
            , TaxPayerEntity aNewTaxPayerEntity)
        {
            childWindow = aChildWindow;
            OldTaxPayerEntity = aOldTaxPayerEntity;
            NewTaxPayerEntity = aNewTaxPayerEntity;

            NewTaxPayerEntity.TaxPayerCode = "";
            NewTaxPayerEntity.TaxPayerName = "";
            NewTaxPayerEntity.TaxPayerRegyear = "";
            NewTaxPayerEntity.TaxPayerProject = "";
            NewTaxPayerEntity.TaxPayerProjectFinish = false;
            NewTaxPayerEntity.TaxPayerFtk = false;

            Title = "添加项目 纳税人：" + OldTaxPayerEntity.TaxPayerName;

            OnOK = new DelegateCommand(onOK);
            OnCancel = new DelegateCommand(onCancel);
        }

        public void onOK()
        {
            if (NewTaxPayerEntity.Validate())
            {
                //NewTaxPayerEntity.TaxPayerCode = OldTaxPayerEntity.TaxPayerCode;
                NewTaxPayerEntity.TaxPayerName = OldTaxPayerEntity.TaxPayerName;
                NewTaxPayerEntity.TaxPayerTypeId = OldTaxPayerEntity.TaxPayerTypeId;
                NewTaxPayerEntity.TaxPayerGroupId = OldTaxPayerEntity.TaxPayerGroupId;

                NewTaxPayerEntity.DUpdate();
                NewTaxPayerEntity.RaisALL();

                childWindow.DialogResult = true;
            }
        }

        public void onCancel()
        {
            childWindow.DialogResult = false;
        }
    }
}
