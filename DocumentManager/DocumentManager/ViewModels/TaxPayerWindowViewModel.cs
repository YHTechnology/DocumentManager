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
using System.Collections.ObjectModel;

namespace DocumentManager.ViewModels
{
    public enum TaxPayerWindowType : uint 
    {
        ADD = 0,
        MODIFY = 1,
    }

    public class TaxPayerWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private TaxPayerWindowType taxPayerWindowType;

        public TaxPayerEntity TaxPayerEntity { get; set; }
        public ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }
        public TaxPayerTypeEntity SelectTaxPayerTypeEntity { get; set; } 


        public string Title { get; set; }
        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }
        public int GroupID { get; set; }

        public bool IsNormal
        {
            get
            {
                if (GroupID == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public TaxPayerWindowViewModel(ChildWindow aChildWindow
            , TaxPayerWindowType aTaxPayerWindowType
            , TaxPayerEntity aTaxPayerEntity
            , ObservableCollection<TaxPayerTypeEntity> aTaxPayerTypeEntityList
            , int aGroupID)
        {
            childWindow = aChildWindow;
            taxPayerWindowType = aTaxPayerWindowType;
            TaxPayerEntity = aTaxPayerEntity;
            TaxPayerTypeList = aTaxPayerTypeEntityList;
            GroupID = aGroupID;

            string lGroup = "";
            switch (GroupID)
            {
                case 0:
                    lGroup = "固定户";
                    break;
                case 1:
                    lGroup = "建安代开";
                    break;
                case 2:
                    lGroup = "普票代开";
                    break;
                case 3:
                    lGroup = "专票代开";
                    break;
            }

            if (taxPayerWindowType == TaxPayerWindowType.ADD)
            {
                Title = "添加纳税人（" + lGroup + ")";
            }
            else
            {
                Title = "修改纳税人（" + lGroup + ") 名称：" + TaxPayerEntity.TaxPayerName;
                SelectTaxPayerTypeEntity = TaxPayerEntity.TaxPayerTypeEntity;
            }

            OnOK = new DelegateCommand(onOK);
            OnCancel = new DelegateCommand(onCancel);
        }

        public void onOK()
        {
            if (TaxPayerEntity.Validate())
            {
                TaxPayerEntity.TaxPayerGroupId = GroupID;
                TaxPayerEntity.TaxPayerTypeEntity = SelectTaxPayerTypeEntity;
                TaxPayerEntity.DUpdate();
                TaxPayerEntity.RaisALL();
                childWindow.DialogResult = true;
            }
        }

        public void onCancel()
        {
            TaxPayerEntity.Update();
            TaxPayerEntity.RaisALL();
            childWindow.DialogResult = false;
        }
    }
}
