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
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Model.Entities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DocumentManager.Views;
using DocumentManager.Controls;
using System.Windows.Data;

namespace DocumentManager.ViewModels
{
    public delegate void FinishLoadedTaxpayer();

    public class DocumentManagerViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer> taxPayerLoader;
        private EntityList<DocumentManager.Web.Model.taxpayer> taxPayerSource;

        private DomainCollectionView<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentLoader;
        private EntityList<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentSource;

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
                    (OnModifyTaxPayer as DelegateCommand).RaiseCanExecuteChanged();
                    (OnAddProject as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteTaxPayer as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string FilterString { get; set; }

        public ICommand OnAddTaxPayer { get; private set; }
        public ICommand OnAddProject { get; private set; }
        public ICommand OnModifyTaxPayer { get; private set; }
        public ICommand OnDeleteTaxPayer { get; private set; }

        public ICommand OnRefresh { get; private set; }
        public ICommand OnDoubleClickList { get; private set; }

        public DocumentViewModel DocumentViewModel { get; set; }

        public int GroupID { get; set; }

        private PagedCollectionView taxpayerView;
        public PagedCollectionView TaxpayerView
        {
            get
            {
                return taxpayerView;
            }
            set
            {
                if (taxpayerView != value)
                {
                    taxpayerView = value;
                    UpdateChanged("TaxpayerView");
                }
            }
        }

        private FinishLoadedTaxpayer finishLoadedTaxpayer;

        public DocumentManagerViewModel()
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
            OnAddTaxPayer = new DelegateCommand(onAddTaxPayer);
            OnAddProject = new DelegateCommand(onAddProject, canAddProject);
            OnModifyTaxPayer = new DelegateCommand(onModifyTaxPayer, canModifyTaxPayer);
            OnDeleteTaxPayer = new DelegateCommand(onDeleteTaxPayer, canDeleteTacPayer);

            OnRefresh = new DelegateCommand(onRefresh);
            OnDoubleClickList = new DelegateCommand(onDoubleClickList);

            taxPayerDocumentSource = new EntityList<Web.Model.taxpayerdocument>(documentManagerContext.taxpayerdocuments);
            taxPayerDocumentLoader = new DomainCollectionViewLoader<Web.Model.taxpayerdocument>(
                LoadTaxPayerDocument,
                LoadTaxPayerDocument_Complete
                );
            taxPayerDocumentView = new DomainCollectionView<Web.Model.taxpayerdocument>(taxPayerDocumentLoader, taxPayerDocumentSource);

        }

        public void SetCallBackLoaded(FinishLoadedTaxpayer afinishLoaded)
        {
            finishLoadedTaxpayer = afinishLoaded;
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

            using (this.taxPayerView.DeferRefresh())
            {
                this.taxPayerView.MoveToFirstPage();
            }
        }

        private LoadOperation<Web.Model.taxpayer> LoadTaxPayerEntities()
        {
            IsBusy = false;
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuery = documentManagerContext.GetTaxpayerQuery();
            lQuery = lQuery.Where(c => (c.taxpayer_group_id == GroupID));
            if (!String.IsNullOrEmpty(FilterString))
            {
                lQuery = lQuery.Where(c => (c.taxpayer_code.Contains(FilterString)) || (c.taxpayer_name.Contains(FilterString)));
            }
            if (GroupID != 1)
            {
                string lYear = DateTime.Now.Year.ToString();
                lQuery = lQuery.Where(c => (c.taxpayer_regyear == lYear || c.taxpayer_project_finish == false));
            }
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

            PagedCollectionView lPagedCollectionView = new PagedCollectionView(TaxPayerList);
            lPagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("TaxPayerName"));
            TaxpayerView = lPagedCollectionView;
            TaxpayerView.Refresh();
            UpdateChanged("TaxpayerView");
            UpdateChanged("TaxPayerList");
            IsBusy = false;

            finishLoadedTaxpayer();
        }

        private void onAddTaxPayer()
        {
            addTaxPayerEntity = new TaxPayerEntity();
            DocumentManager.Web.Model.taxpayer taxpayer = new DocumentManager.Web.Model.taxpayer();
            addTaxPayerEntity.TaxPayer = taxpayer;
            addTaxPayerEntity.Update();
            switch (GroupID)
            {
                case 0:
                    {
                        TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.ADD, addTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += AddTaxPayer_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;
                case 1:
                    {
                        TaxPayerWindowJA lTaxPayerWindow = new TaxPayerWindowJA(TaxPayerWindowType.ADD, addTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += AddTaxPayer_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;
                case 2:
                    {
                        TaxPayerWindowPP lTaxPayerWindow = new TaxPayerWindowPP(TaxPayerWindowType.ADD, addTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += AddTaxPayer_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;
                case 3:
                    {
                        TaxPayerWindowZP lTaxPayerWindow = new TaxPayerWindowZP(TaxPayerWindowType.ADD, addTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += AddTaxPayer_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;

            }
            //TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.ADD, addTaxPayerEntity, TaxPayerTypeList, GroupID);
            //lTaxPayerWindow.Closed += AddTaxPayer_Closed;
            //lTaxPayerWindow.Show();
        }

        private void onAddProject()
        {
            addTaxPayerEntity = new TaxPayerEntity();
            DocumentManager.Web.Model.taxpayer taxpayer = new DocumentManager.Web.Model.taxpayer();
            addTaxPayerEntity.TaxPayer = taxpayer;
            addTaxPayerEntity.Update();
            AddProjectWindow lAddProjectWindow = new AddProjectWindow(SelectTaxPayerEntity, addTaxPayerEntity);
            lAddProjectWindow.Closed += new EventHandler(AddProjectWindow_Closed);
            lAddProjectWindow.Show();
           
        }

        void AddProjectWindow_Closed(object sender, EventArgs e)
        {
            ChildWindow lTaxPayerWindow = sender as ChildWindow;
            if (lTaxPayerWindow.DialogResult == true)
            {
                IsBusy = true;
                TaxPayerList.Add(addTaxPayerEntity);
                documentManagerContext.taxpayers.Add(addTaxPayerEntity.TaxPayer);               
                Log.AddLog(documentManagerContext, addTaxPayerEntity.ToString());

                 SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperationAddProject_Completed;
            }
        }

        private void AutoLinkDocumentFiles(TaxPayerEntity aTaxPayerEntity, TaxPayerEntity aSourceTaxpayerEntity)
        {
            using (taxPayerDocumentView.DeferRefresh())
            {
                taxPayerDocumentView.MoveToFirstPage();
            }
        }

        private LoadOperation<Web.Model.taxpayerdocument> LoadTaxPayerDocument()
        {
            EntityQuery<Web.Model.taxpayerdocument> lQuery = documentManagerContext.GetTaxpayerdocumentQuery();
            if (SelectTaxPayerEntity != null)
            {
                lQuery = lQuery.Where(c => c.taxpayer_id == SelectTaxPayerEntity.TaxPayerId);
            }
            else
            {
                lQuery = lQuery.Where(c => c.taxpayer_id == -100);
            }
            return documentManagerContext.Load(lQuery.SortAndPageBy(taxPayerDocumentView));
        }

        private void LoadTaxPayerDocument_Complete(LoadOperation<Web.Model.taxpayerdocument> sender)
        {
            taxPayerDocumentSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.taxpayerdocument taxpayerdocument in sender.Entities)
            {
                TaxPayerDocumentEntity taxPayerDocumentEntity = new TaxPayerDocumentEntity();
                taxPayerDocumentEntity.TaxPayerDocument = taxpayerdocument;
                taxPayerDocumentEntity.Update();

                if (taxPayerDocumentEntity.TaxPayerDocumentTypeId == 2) // 拨款报告
                {
                    continue;
                }

                if (taxPayerDocumentEntity.TaxPayerDocumentTypeId == 7) // 税票及发票
                {
                    continue;
                }

                if (taxPayerDocumentEntity.TaxPayerDocumentTypeId == 5) // 合同
                {
                    continue;
                }

                if (taxPayerDocumentEntity.TaxPayerDocumentTypeId == 6) // 代开申请
                {
                    continue;
                }

                if (taxPayerDocumentEntity.TaxPayerDocumentTypeId == 3) // 申报表
                {
                    continue;
                }

                App lApp = Application.Current as App;

                TaxPayerDocumentEntity lTaxPayerDocumentEntity = new TaxPayerDocumentEntity();
                lTaxPayerDocumentEntity.TaxPayerDocument = new Web.Model.taxpayerdocument();

                lTaxPayerDocumentEntity.TaxPayerId = addTaxPayerEntity.TaxPayerId;
                lTaxPayerDocumentEntity.TaxPayerDocumentName = taxPayerDocumentEntity.TaxPayerDocumentName;
                lTaxPayerDocumentEntity.TaxPayerDocumentTypeId = taxPayerDocumentEntity.TaxPayerDocumentTypeId;
                lTaxPayerDocumentEntity.TaxPayerDocumentDescript = taxPayerDocumentEntity.TaxPayerDocumentDescript;
                lTaxPayerDocumentEntity.TaxPayerUpdateUserId = lApp.MainPageViewModel.User.UserID;
                lTaxPayerDocumentEntity.TaxPayerUpdateTime = taxPayerDocumentEntity.TaxPayerUpdateTime;
                lTaxPayerDocumentEntity.TaxPayerDocumentBytes = taxPayerDocumentEntity.TaxPayerDocumentBytes;
                lTaxPayerDocumentEntity.TaxPayerIsLink = true;
                if (taxPayerDocumentEntity.TaxPayerIsLink.HasValue && taxPayerDocumentEntity.TaxPayerIsLink.Value)
                {
                    lTaxPayerDocumentEntity.TaxPayerLinkId = taxPayerDocumentEntity.TaxPayerLinkId;
                }
                else
                {
                    lTaxPayerDocumentEntity.TaxPayerLinkId = SelectTaxPayerEntity.TaxPayerId;
                }
                
                lTaxPayerDocumentEntity.TaxPayerDocument = new Web.Model.taxpayerdocument();
                lTaxPayerDocumentEntity.DUpdate();
                documentManagerContext.taxpayerdocuments.Add(lTaxPayerDocumentEntity.TaxPayerDocument);
            }
            SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
            lSubmitOperation.Completed += SubOperation_Completed;
        }

        private bool canAddProject(object aObject)
        {
            return SelectTaxPayerEntity != null && GroupID != 0;
        }

        private void onModifyTaxPayer()
        {
            switch (GroupID)
            {
                case 0:
                    {
                        TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;
                case 1:
                    {
                        TaxPayerWindowJA lTaxPayerWindow = new TaxPayerWindowJA(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;
                case 2:
                    {
                        TaxPayerWindowPP lTaxPayerWindow = new TaxPayerWindowPP(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;
                case 3:
                    {
                        TaxPayerWindowZP lTaxPayerWindow = new TaxPayerWindowZP(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
                        lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
                        lTaxPayerWindow.Show();
                    }
                    break;

            }

            //TaxPayerWindow lTaxPayerWindow = new TaxPayerWindow(TaxPayerWindowType.MODIFY, SelectTaxPayerEntity, TaxPayerTypeList, GroupID);
            //lTaxPayerWindow.Closed += TaxPayerWindow_Closed;
            //lTaxPayerWindow.Show();
        }

        private bool canModifyTaxPayer(object aObject)
        {
            return SelectTaxPayerEntity != null;
        }

        private void onDeleteTaxPayer()
        {
            ConfirmWindow lConfirmWindow = new ConfirmWindow("删除纳税人", "是否确认删除纳税人: " + SelectTaxPayerEntity.TaxPayerName + "? 与之关联的台账和档案将丢失！");
            lConfirmWindow.Closed += new EventHandler(DeleteTaxPayerConfirm_Closed);
            lConfirmWindow.Show();
        }

        void DeleteTaxPayerConfirm_Closed(object sender, EventArgs e)
        {
            ConfirmWindow lConfirmWindow = sender as ConfirmWindow;
            if (lConfirmWindow.DialogResult == true)
            {
                documentManagerContext.taxpayers.Remove(SelectTaxPayerEntity.TaxPayer);
                Log.DeleteLog(documentManagerContext, SelectTaxPayerEntity.ToString());
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        private bool canDeleteTacPayer(object aObject)
        {
            return SelectTaxPayerEntity != null;
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

        private void TaxPayerWindow_Closed(object sender, EventArgs e)
        {
            ChildWindow lTaxPayerWindow = sender as ChildWindow;
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
            ChildWindow lTaxPayerWindow = sender as ChildWindow;
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

        void SubOperationAddProject_Completed(object sender, EventArgs e)
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
                IsBusy = false;
            }
            else
            {
                addTaxPayerEntity.Update();
                AutoLinkDocumentFiles(addTaxPayerEntity, SelectTaxPayerEntity);
            }

        }
    }
}
