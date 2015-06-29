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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DocumentManager.Model.Entities;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Model.SearchEntities;

namespace DocumentManager.ViewModels
{
    public class DocumentSearchZPViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer> taxPayerLoader;
        private EntityList<DocumentManager.Web.Model.taxpayer> taxPayerSource;
        private TaxPayerEntity selectTaxPayerEntity;
        private ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }
        private Dictionary<int, TaxPayerTypeEntity> TaxPayerTypeEntityDictionary { get; set; }
        private TaxPayerEntity addTaxPayerEntity;
        public ObservableCollection<TaxPayerEntity> TaxPayerList { get; set; }

        private Dictionary<int, FileTypeEntity> FileTypeDictionary { get; set; }
        private ObservableCollection<FileTypeEntity> FileTypeList { get; set; }
        private Dictionary<int, UserEntity> UserEntityDictionary { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        private bool showExpander = false;
        public bool ShowExpander
        {
            get { return showExpander; }
            set { if (showExpander != value) { showExpander = value; UpdateChanged("ShowExpander"); } }
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
                    DocumentViewModel.TaxPayerEntity = value;
                    UpdateChanged("SelectTaxPayerEntity");
                    //(OnModifyTaxPayer as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string FilterString { get; set; }

        public ICommand OnAddTaxPayer { get; private set; }
        public ICommand OnModifyTaxPayer { get; private set; }
        public ICommand OnRefresh { get; private set; }
        public ICommand OnDoubleClickList { get; private set; }

        public DocumentViewModel DocumentViewModel { get; set; }

        public int GroupID { get; set; }

        public TaxPayerSearch TaxPayerSearch { get; set; }

        public string SearchInfo
        {
            get
            {
                int lCount = TaxPayerList.Count;
                string lRet = "查询 共 " + lCount.ToString() + "条记录";
                return lRet;
            }
        }

        public DocumentSearchZPViewModel()
        {
            TaxPayerList = new ObservableCollection<TaxPayerEntity>();
            TaxPayerTypeList = new ObservableCollection<TaxPayerTypeEntity>();
            TaxPayerTypeEntityDictionary = new Dictionary<int, TaxPayerTypeEntity>();
            FileTypeList = new ObservableCollection<FileTypeEntity>();
            FileTypeDictionary = new Dictionary<int, FileTypeEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();

            DocumentViewModel = new DocumentViewModel();
            DocumentViewModel.BeginLoadings += BeginLoading;
            DocumentViewModel.FinishLoadings += FinishLoading;

            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();
            OnRefresh = new DelegateCommand(onRefresh);
            OnDoubleClickList = new DelegateCommand(onDoubleClickList);

            TaxPayerSearch = new TaxPayerSearch();

            TaxPayerSearch.GroupID = 3;
            TaxPayerSearch.TaxPayerTypeList = TaxPayerTypeList;
        }

        private void BeginLoading(object sender, EventArgs e)
        {
            IsBusy = true;
        }

        private void FinishLoading(object sender, EventArgs e)
        {
            IsBusy = false;
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

            LoadOperation<DocumentManager.Web.Model.filetype> loadOperationFileType =
                documentManagerContext.Load<DocumentManager.Web.Model.filetype>(documentManagerContext.GetFiletypeQuery());
            loadOperationFileType.Completed += loadOperationFileType_Completed;
        }

        private void loadOperationFileType_Completed(object sender, EventArgs e)
        {
            FileTypeList.Clear();
            FileTypeDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.filetype filetype in loadOperation.Entities)
            {
                FileTypeEntity lFileTypeEntity = new FileTypeEntity();
                lFileTypeEntity.FileType = filetype;
                lFileTypeEntity.Update();
                FileTypeList.Add(lFileTypeEntity);
                FileTypeDictionary.Add(lFileTypeEntity.FileTypeId, lFileTypeEntity);
            }

            DocumentViewModel.FileTypeEntityDictionary = FileTypeDictionary;
            DocumentViewModel.FileTypeEntityList = FileTypeList;

            LoadOperation<DocumentManager.Web.Model.user> loadOperationUser =
                documentManagerContext.Load<DocumentManager.Web.Model.user>(documentManagerContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;
        }

        private void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.user user in loadOperation.Entities)
            {
                UserEntity lUserEntity = new UserEntity();
                lUserEntity.User = user;
                lUserEntity.Update();
                UserEntityDictionary.Add(lUserEntity.UserId, lUserEntity);
            }
            DocumentViewModel.UserEntityDictionary = UserEntityDictionary;

            taxPayerSource = new EntityList<Web.Model.taxpayer>(documentManagerContext.taxpayers);
            taxPayerLoader = new DomainCollectionViewLoader<Web.Model.taxpayer>(
                LoadTaxPayerEntities,
                loadOperation_Completed);
            taxPayerView = new DomainCollectionView<Web.Model.taxpayer>(taxPayerLoader, taxPayerSource);

            TaxPayerSearch.taxPayerView = taxPayerView;

            using (this.taxPayerView.DeferRefresh())
            {
                this.taxPayerView.MoveToFirstPage();
            }
        }

        private LoadOperation<Web.Model.taxpayer> LoadTaxPayerEntities()
        {
            IsBusy = false;
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuery = documentManagerContext.GetTaxpayerQuery();
            lQuery = lQuery.Where(c => (c.taxpayer_group_id == 3));


            lQuery = TaxPayerSearch.CreateQuery(lQuery);

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
            UpdateChanged("SearchInfo");
            IsBusy = false;
        }

        private void onRefresh()
        {
            using (this.taxPayerView.DeferRefresh())
            {
                this.taxPayerView.MoveToFirstPage();
            }
        }

        private void onDoubleClickList()
        {
            ShowExpander = !ShowExpander; 
        }
    }
}