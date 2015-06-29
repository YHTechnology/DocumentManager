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
using Microsoft.Windows.Data.DomainServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Views;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class TaxPayerManagerViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer> taxPayerLoader;
        private EntityList<DocumentManager.Web.Model.taxpayer> taxPayerSource;
        private TaxPayerEntity selectTaxPayerEntity;
        private TaxPayerEntity addTaxPayerEntity;
        private ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }
        private Dictionary<int, TaxPayerTypeEntity> TaxPayerTypeEntityDictionary { get; set; }

        public ObservableCollection<TaxPayerEntity> TaxPayerList { get; set; }
        public int GroupID { get; set; }
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        public TaxPayerEntity SelectTaxPayerEntity
        {
            get
            {
                return selectTaxPayerEntity;
            }
            set
            {
                if (selectTaxPayerEntity != value)
                {
                    selectTaxPayerEntity = value;
                    UpdateChanged("SelectTaxPayerEntity");
                    (OnModifyTaxPayer as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        
        public ICommand OnAddTaxPayer { get; private set; }
        public ICommand OnModifyTaxPayer { get; private set; }
        public ICommand OnDoubleClickList { get; private set; }

        public TaxPayerManagerViewModel()
        {
            TaxPayerList = new ObservableCollection<TaxPayerEntity>();
            TaxPayerTypeList = new ObservableCollection<TaxPayerTypeEntity>();
            TaxPayerTypeEntityDictionary = new Dictionary<int, TaxPayerTypeEntity>();

            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();
            OnAddTaxPayer = new DelegateCommand(onAddTaxPayer);
            OnModifyTaxPayer = new DelegateCommand(onModifyTaxPayer, canModifyTaxPayer);
            OnDoubleClickList = new DelegateCommand(onDoubleClickList);
        }

        public void LoadData()
        {
            IsBusy = true;
            LoadOperation<DocumentManager.Web.Model.taxpayertype> loadOperationTaxPayerType =
                documentManagerContext.Load<DocumentManager.Web.Model.taxpayertype>(documentManagerContext.GetTaxpayertypeQuery());
            loadOperationTaxPayerType.Completed += loadOperationTaxPayerType_Completed;
        }

        private void loadOperationTaxPayerType_Completed(object sender, EventArgs e)
        {
            TaxPayerTypeList.Clear();
            TaxPayerTypeEntityDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.taxpayertype taxpayertype in loadOperation.Entities)
            {
                TaxPayerTypeEntity lTaxPayerTypeEntity = new TaxPayerTypeEntity();
                lTaxPayerTypeEntity.TaxPayerType = taxpayertype;
                lTaxPayerTypeEntity.Update();
                TaxPayerTypeList.Add(lTaxPayerTypeEntity);
                TaxPayerTypeEntityDictionary.Add(lTaxPayerTypeEntity.TaxPayerTypeId, lTaxPayerTypeEntity);
            }

            taxPayerSource = new EntityList<Web.Model.taxpayer>(documentManagerContext.taxpayers);
            taxPayerLoader = new DomainCollectionViewLoader<Web.Model.taxpayer>(
                LoadTaxPayerEntities,
                loadOperation_Completed);
            taxPayerView = new DomainCollectionView<Web.Model.taxpayer>(taxPayerLoader, taxPayerSource);

            using (this.taxPayerView.DeferRefresh())
            {
                this.taxPayerView.MoveToFirstPage();
            }
        }

        private LoadOperation<Web.Model.taxpayer> LoadTaxPayerEntities()
        {
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuery = documentManagerContext.GetTaxpayerQuery();
            return documentManagerContext.Load(lQuery.SortAndPageBy(this.taxPayerView));
        }

        private void loadOperation_Completed(LoadOperation<DocumentManager.Web.Model.taxpayer> sender)
        {
            TaxPayerList.Clear();
            taxPayerSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.taxpayer taxpayer in sender.Entities)
            {
                TaxPayerEntity taxPayerEntity = new TaxPayerEntity();
                taxPayerEntity.TaxPayer = taxpayer;
                taxPayerEntity.Update();

                if (taxPayerEntity.TaxPayerTypeId.HasValue)
                {
                    TaxPayerTypeEntity taxPayerTypeEntity;
                    if (TaxPayerTypeEntityDictionary.TryGetValue(taxPayerEntity.TaxPayerTypeId.Value, out taxPayerTypeEntity))
                    {
                        taxPayerEntity.TaxPayerTypeEntity = taxPayerTypeEntity;
                    }
                }

                
                TaxPayerList.Add(taxPayerEntity);
            }
            UpdateChanged("TaxPayerList");
            IsBusy = false;
        }

        private void onAddTaxPayer()
        {
            addTaxPayerEntity = new TaxPayerEntity();
            DocumentManager.Web.Model.taxpayer taxpayer = new DocumentManager.Web.Model.taxpayer();
            addTaxPayerEntity.TaxPayer = taxpayer;
            addTaxPayerEntity.Update();
            TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.ADD, addTaxPayerEntity, TaxPayerTypeList, GroupID);
            lTaxPayerWindow.Closed += AddTaxPayer_Closed;
            lTaxPayerWindow.Show();
        }

        private void onModifyTaxPayer()
        {
            TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
            lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
            lTaxPayerWindow.Show();
        }

        private bool canModifyTaxPayer(object aObject)
        {
            return SelectTaxPayerEntity != null;
        }

        private void onDoubleClickList()
        {
            TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
            lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
            lTaxPayerWindow.Show();
        }

        private void TaxPayerWindow_Closed(object sender, EventArgs e)
        {
            TaxPayerWindow lTaxPayerWindow = sender as TaxPayerWindow;
            if (lTaxPayerWindow.DialogResult == true)
            {
                IsBusy = true;
                Log.ModifyLog(documentManagerContext, SelectTaxPayerEntity.ToString());
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        private void AddTaxPayer_Closed(object sender, EventArgs e)
        {
            TaxPayerWindow lTaxPayerWindow = sender as TaxPayerWindow;
            if (lTaxPayerWindow.DialogResult == true)
            {
                IsBusy = true;
                TaxPayerList.Add(addTaxPayerEntity);
                documentManagerContext.taxpayers.Add(addTaxPayerEntity.TaxPayer);
                Log.AddLog(documentManagerContext, addTaxPayerEntity.ToString());
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
                if (addTaxPayerEntity != null)
                {
                    TaxPayerList.Remove(addTaxPayerEntity);
                    addTaxPayerEntity = null;
                }
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
                addTaxPayerEntity = null;
                LoadData();
            }
            IsBusy = false;
        }
    }
}
