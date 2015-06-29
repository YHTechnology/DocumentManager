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
using System.Collections.Generic;
using Microsoft.Windows.Data.DomainServices;
using System.ServiceModel.DomainServices.Client;

namespace DocumentManager.ViewModels
{
    public class LinkFileViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private ChildWindow childWindow;
        public ObservableCollection<TaxPayerEntity> TaxPayerEntityList { get; set; }
        public ObservableCollection<TaxPayerEntity> TaxPayerEntityLinkList { get; set; }
        public ObservableCollection<TaxPayerDocumentEntity> TaxPayerDocumentEntityList { get; set; }

        private Dictionary<int, FileTypeEntity> FileTypeEntityDictionary { get; set; }

        private DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer> taxPayerLoader;
        private EntityList<DocumentManager.Web.Model.taxpayer> taxPayerSource;

        private DomainCollectionView<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentLoader;
        private EntityList<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentSource;

        public String FilterContext { get; set; }


        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public ICommand OnReflash { get; private set; }
        public ICommand OnAddToTaxPayer { get; private set; }
        public ICommand OnRemoveTaxPayer { get; private set; }

        public int GroupID { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        private TaxPayerEntity selectTaxPayerEntity;
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
                    if (null != selectTaxPayerEntity)
                    {
                        //LoadData();
                        using (taxPayerDocumentView.DeferRefresh())
                        {
                            taxPayerDocumentView.MoveToFirstPage();
                        }
                    }
                    UpdateChanged("SelectTaxPayerEntity");
                }
            }
        }

        public TaxPayerEntity SelectLinkTaxPayerEntity { get; set; }

        private TaxPayerDocumentEntity selectTaxPayerDocumentEntity;
        public TaxPayerDocumentEntity SelectTaxPayerDocumentEntity
        {
            get { return selectTaxPayerDocumentEntity; }
            set
            {
                if (selectTaxPayerDocumentEntity != value)
                {
                    selectTaxPayerDocumentEntity = value;
                }
                (OnOK as DelegateCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<TaxPayerDocumentEntity> SelectTaxPayerDocumentEntitis;

        public LinkFileViewModel(ChildWindow aChildWindow, Dictionary<int, FileTypeEntity> aFileTypeDictionary)
        {
            documentManagerContext = new Web.DocumentManagerDomainContext();
            childWindow = aChildWindow;
            SelectTaxPayerDocumentEntitis = new ObservableCollection<TaxPayerDocumentEntity>();
            TaxPayerEntityList = new ObservableCollection<TaxPayerEntity>();
            TaxPayerEntityLinkList = new ObservableCollection<TaxPayerEntity>();
            TaxPayerDocumentEntityList = new ObservableCollection<TaxPayerDocumentEntity>();
            FileTypeEntityDictionary = aFileTypeDictionary;
            GroupID = 1;
            OnOK = new DelegateCommand(OnOKCommand, CanOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
            OnReflash = new DelegateCommand(OnReflashCommand);
            OnAddToTaxPayer = new DelegateCommand(OnAddToTaxPayerCommand);
            OnRemoveTaxPayer = new DelegateCommand(OnRemoveTaxPayerCommand);

            taxPayerSource = new EntityList<DocumentManager.Web.Model.taxpayer>(documentManagerContext.taxpayers);
            taxPayerLoader = new DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer>(
                LoadTaxPayerEntities,
                LoadOperationTaxPayerCompleted
                );
            taxPayerView = new DomainCollectionView<DocumentManager.Web.Model.taxpayer>(taxPayerLoader, taxPayerSource);

            taxPayerDocumentSource = new EntityList<DocumentManager.Web.Model.taxpayerdocument>(documentManagerContext.taxpayerdocuments);
            taxPayerDocumentLoader = new DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayerdocument>(
                LoadTaxPayerDocumentEntities,
                LoadOperationTaxPayerDocumentCompleted
                );
            taxPayerDocumentView = new DomainCollectionView<DocumentManager.Web.Model.taxpayerdocument>(taxPayerDocumentLoader, taxPayerDocumentSource);
        }

        public void LoadData()
        {
            IsBusy = true;
            using (taxPayerView.DeferRefresh())
            {
                taxPayerView.MoveToFirstPage();
            }
        }

        private LoadOperation<DocumentManager.Web.Model.taxpayer> LoadTaxPayerEntities()
        {
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuery = documentManagerContext.GetTaxpayerQuery();
            if (FilterContext != null)
            {
                lQuery = lQuery.Where(c => (c.taxpayer_group_id == GroupID) && (c.taxpayer_code.ToLower().Contains(FilterContext.ToLower()) || c.taxpayer_name.ToLower().Contains(FilterContext.ToLower())));
            }
            else
            {
                lQuery = lQuery.Where(c => (c.taxpayer_group_id == GroupID));
            }
            return documentManagerContext.Load(lQuery.SortAndPageBy(this.taxPayerView));
        }

        private void LoadOperationTaxPayerCompleted(LoadOperation<DocumentManager.Web.Model.taxpayer> aLoadOperation)
        {
            TaxPayerEntityList.Clear();
            foreach (DocumentManager.Web.Model.taxpayer taxpayer in aLoadOperation.Entities)
            {
                TaxPayerEntity taxPayerEntity = new TaxPayerEntity();
                taxPayerEntity.TaxPayer = taxpayer;
                taxPayerEntity.Update();
                TaxPayerEntityList.Add(taxPayerEntity);
            }
            UpdateChanged("TaxPayerEntityList");
            IsBusy = false;
        }

        private LoadOperation<DocumentManager.Web.Model.taxpayerdocument> LoadTaxPayerDocumentEntities()
        {
            IsBusy = true;
            EntityQuery<DocumentManager.Web.Model.taxpayerdocument> lQuery = documentManagerContext.GetTaxpayerdocumentQuery();
            if (SelectTaxPayerEntity != null)
            {
                lQuery = lQuery.Where(c => c.taxpayer_id == SelectTaxPayerEntity.TaxPayerId);
            }
            else
            {
                lQuery = lQuery.Where(c => c.taxpayer_id == -1);
            }
            return documentManagerContext.Load(lQuery.SortAndPageBy(this.taxPayerDocumentView));
        }

        private void LoadOperationTaxPayerDocumentCompleted(LoadOperation<DocumentManager.Web.Model.taxpayerdocument> aLoadOperation)
        {
            TaxPayerDocumentEntityList.Clear();
            foreach (DocumentManager.Web.Model.taxpayerdocument taxpayerdocument in aLoadOperation.Entities)
            {
                TaxPayerDocumentEntity taxPayerDocumentEntity = new TaxPayerDocumentEntity();
                taxPayerDocumentEntity.TaxPayerDocument = taxpayerdocument;
                taxPayerDocumentEntity.Update();

                FileTypeEntity lFileTypeEntity;
                if (FileTypeEntityDictionary != null
                    && taxPayerDocumentEntity.TaxPayerDocumentTypeId.HasValue
                    && FileTypeEntityDictionary.TryGetValue(taxPayerDocumentEntity.TaxPayerDocumentTypeId.Value, out lFileTypeEntity))
                {
                    taxPayerDocumentEntity.FileTypeName = lFileTypeEntity.FileTypeName;
                }

                TaxPayerDocumentEntityList.Add(taxPayerDocumentEntity);
            }
            UpdateChanged("TaxPayerDocumentEntityList");
            IsBusy = false;
        }

        private void OnOKCommand()
        {
            childWindow.DialogResult = true;
        }

        private bool CanOKCommand(Object aObject)
        {
            return SelectTaxPayerDocumentEntity != null;
        }

        private void OnCancelCommand()
        {
            childWindow.DialogResult = false;
        }

        private void OnReflashCommand()
        {
            IsBusy = true;
            using (taxPayerView.DeferRefresh())
            {
                taxPayerView.MoveToFirstPage();
            }
        }

        private void OnAddToTaxPayerCommand()
        {
            if (SelectTaxPayerEntity != null)
            {
                bool lIsAdded = false;
                foreach (TaxPayerEntity taxPayerEntity in TaxPayerEntityLinkList)
                {
                    if (taxPayerEntity.TaxPayerId == SelectTaxPayerEntity.TaxPayerId)
                    {
                        lIsAdded = true;
                        break;
                    }
                }

                if (!lIsAdded)
                {
                    TaxPayerEntityLinkList.Add(SelectTaxPayerEntity);
                    UpdateChanged("TaxPayerEntityLinkList");
                }
            }
        }

        private void OnRemoveTaxPayerCommand()
        {
            if (SelectLinkTaxPayerEntity != null)
            {
                TaxPayerEntityLinkList.Remove(SelectLinkTaxPayerEntity);
                UpdateChanged("TaxPayerEntityLinkList");
            }
        }
    }
}
