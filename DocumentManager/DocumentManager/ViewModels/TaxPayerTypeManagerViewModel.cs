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
using Microsoft.Windows.Data.DomainServices;
using DocumentManager.Model.Entities;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Views;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class TaxPayerTypeManagerViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.taxpayertype> taxPayerTypeView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayertype> taxPayerTypeLoader;
        private EntityList<DocumentManager.Web.Model.taxpayertype> taxPayerTypeSource;
        private TaxPayerTypeEntity selectTaxPayerTypeEntity;
        private TaxPayerTypeEntity addTaxPayerTypeEntity;

        public ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }
        
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        public TaxPayerTypeEntity SelectTaxPayerTypeEntity
        {
            get
            {
                return selectTaxPayerTypeEntity;
            }
            set
            {
                if (selectTaxPayerTypeEntity != value)
                {
                    selectTaxPayerTypeEntity = value;
                    UpdateChanged("SelectTaxPayerTypeEntity");
                    (OnModifyTaxPayerType as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnAddTaxPayerType { get; private set; }
        public ICommand OnModifyTaxPayerType { get; private set; }
        public ICommand OnDoubleClickList { get; private set; }

        public TaxPayerTypeManagerViewModel()
        {
            TaxPayerTypeList = new ObservableCollection<TaxPayerTypeEntity>();
            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();
            OnAddTaxPayerType = new DelegateCommand(onAddTaxPayerType);
            OnModifyTaxPayerType = new DelegateCommand(onModifyTaxPayerType, canModifyTaxPayerType);
            OnDoubleClickList = new DelegateCommand(onDoubleClickList);
        }

        public void LoadData()
        {
            IsBusy = true;

            this.taxPayerTypeSource = new EntityList<DocumentManager.Web.Model.taxpayertype>(this.documentManagerContext.taxpayertypes);
            this.taxPayerTypeLoader = new DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayertype>(
                this.LoadTaxPayerTypeEntities,
                this.loadOperation_Completed);
            this.taxPayerTypeView = new DomainCollectionView<DocumentManager.Web.Model.taxpayertype>(this.taxPayerTypeLoader, this.taxPayerTypeSource);

            using (this.taxPayerTypeView.DeferRefresh())
            {
                this.taxPayerTypeView.MoveToFirstPage();
            }
        }

        private LoadOperation<DocumentManager.Web.Model.taxpayertype> LoadTaxPayerTypeEntities()
        {
            this.IsBusy = true;
            EntityQuery<DocumentManager.Web.Model.taxpayertype> lQuery = documentManagerContext.GetTaxpayertypeQuery();
            return documentManagerContext.Load(lQuery.SortAndPageBy(this.taxPayerTypeView));
        }

        private void loadOperation_Completed(LoadOperation<DocumentManager.Web.Model.taxpayertype> sender)
        {
            TaxPayerTypeList.Clear();
            this.taxPayerTypeSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.taxpayertype taxpayertype in sender.Entities)
            {
                TaxPayerTypeEntity taxPayerTypeEntity = new TaxPayerTypeEntity();
                taxPayerTypeEntity.TaxPayerType = taxpayertype;
                taxPayerTypeEntity.Update();
                TaxPayerTypeList.Add(taxPayerTypeEntity);
            }
            UpdateChanged("TaxPayerTypeList");
            IsBusy = false;
        }

        private void onAddTaxPayerType()
        {
            addTaxPayerTypeEntity = new TaxPayerTypeEntity();
            DocumentManager.Web.Model.taxpayertype taxpayertype = new DocumentManager.Web.Model.taxpayertype();
            addTaxPayerTypeEntity.TaxPayerType = taxpayertype;
            addTaxPayerTypeEntity.Update();
            TaxPayerTypeWindow lTaxPayerTypeWindow = new TaxPayerTypeWindow(TaxPayerTypeWindowType.ADD, addTaxPayerTypeEntity);
            lTaxPayerTypeWindow.Closed += AddTaxPayerType_Closed;
            lTaxPayerTypeWindow.Show();
        }

        private void onModifyTaxPayerType()
        {
            TaxPayerTypeWindow lTaxPayerTypeWindow = new TaxPayerTypeWindow(TaxPayerTypeWindowType.MODIFY, SelectTaxPayerTypeEntity);
            lTaxPayerTypeWindow.Closed += TaxPayerTypeWindow_Closed;
            lTaxPayerTypeWindow.Show();
        }

        private bool canModifyTaxPayerType(object aObject)
        {
            return SelectTaxPayerTypeEntity != null;
        }

        private void onDoubleClickList()
        {
            TaxPayerTypeWindow lTaxPayerTypeWindow = new TaxPayerTypeWindow(TaxPayerTypeWindowType.MODIFY, SelectTaxPayerTypeEntity);
            lTaxPayerTypeWindow.Closed += TaxPayerTypeWindow_Closed;
            lTaxPayerTypeWindow.Show();
        }

        private void AddTaxPayerType_Closed(object sender, EventArgs e)
        {
            TaxPayerTypeWindow lTaxPayerTypeWindow = sender as TaxPayerTypeWindow;
            if (lTaxPayerTypeWindow.DialogResult == true)
            {
                IsBusy = true;
                TaxPayerTypeList.Add(addTaxPayerTypeEntity);
                documentManagerContext.taxpayertypes.Add(addTaxPayerTypeEntity.TaxPayerType);
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        private void TaxPayerTypeWindow_Closed(object sender, EventArgs e)
        {
            TaxPayerTypeWindow lTaxPayerTypeWindow = sender as TaxPayerTypeWindow;
            if (lTaxPayerTypeWindow.DialogResult == true)
            {
                IsBusy = true;
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        void SubOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
                if (addTaxPayerTypeEntity != null)
                {
                    TaxPayerTypeList.Remove(addTaxPayerTypeEntity);
                    addTaxPayerTypeEntity = null;
                }
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
                addTaxPayerTypeEntity = null;
                LoadData();
            }
            IsBusy = false;
        }
    }
}
