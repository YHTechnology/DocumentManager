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
    public enum TaxPayerTypeWindowType : uint
    {
        ADD = 0,
        MODIFY = 1
    }

    public class TaxPayerTypeWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private TaxPayerTypeWindowType taxPayerTypeWindowType;

        public TaxPayerTypeEntity TaxPayerTypeEntity { get; set; }

        public string Title { get; set; }
        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public TaxPayerTypeWindowViewModel(ChildWindow aChildWindow, TaxPayerTypeWindowType aTaxPayerTypeWindowType, TaxPayerTypeEntity aTaxPayerTypeEntity)
        {
            childWindow = aChildWindow;
            taxPayerTypeWindowType = aTaxPayerTypeWindowType;
            TaxPayerTypeEntity = aTaxPayerTypeEntity;

            if (aTaxPayerTypeWindowType == TaxPayerTypeWindowType.ADD)
            {
                Title = "添加纳税人类型";
            }
            else
            {
                Title = "修改纳税人类型";
            }

            OnOK = new DelegateCommand(onOK);
            OnCancel = new DelegateCommand(onCancel);
        }

        public void onOK()
        {
            if (TaxPayerTypeEntity.Validate())
            {
                TaxPayerTypeEntity.DUpdate();
                TaxPayerTypeEntity.RaisALL();
                childWindow.DialogResult = true;
            }
        }

        public void onCancel()
        {
            TaxPayerTypeEntity.Update();
            TaxPayerTypeEntity.RaisALL();
            childWindow.DialogResult = false;
        }
    }
}
