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
using System.Collections.Generic;
using DocumentManager.Views;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class DocumentViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentLoader;
        private EntityList<DocumentManager.Web.Model.taxpayerdocument> taxPayerDocumentSource;
        private TaxPayerEntity taxPayerEntity;
        private TaxPayerDocumentEntity selectTaxPayerDocumentEntity;

        public ObservableCollection<TaxPayerDocumentEntity> TaxPayerDocumentList { get; set; }
        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }
        public Dictionary<int, FileTypeEntity> FileTypeEntityDictionary { get; set; }
        public Dictionary<int, UserEntity> UserEntityDictionary { get; set; }

        public TaxPayerEntity TaxPayerEntity
        {
            set
            {
                if (taxPayerEntity != value)
                {
                    taxPayerEntity = value;
                    //if (taxPayerEntity != null)
                    {
                        using (taxPayerDocumentView.DeferRefresh())
                        {
                            taxPayerDocumentView.MoveToFirstPage();
                        }
                    }
                    (OnAddSignalDocument as DelegateCommand).RaiseCanExecuteChanged();
                    (OnAddMultiDocument as DelegateCommand).RaiseCanExecuteChanged();
                    (OnMergeImageDocument as DelegateCommand).RaiseCanExecuteChanged();
                    (OnLinkDocument as DelegateCommand).RaiseCanExecuteChanged();
                }

            }
        }

        public TaxPayerDocumentEntity SelectTaxPayerDocumentEntity
        {
            get
            {
                return selectTaxPayerDocumentEntity;
            }
            set
            {
                if (selectTaxPayerDocumentEntity != value)
                {
                    selectTaxPayerDocumentEntity = value;
                    (OnDeleteDocument as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnAddSignalDocument { get; private set; }
        public ICommand OnAddMultiDocument { get; private set; }
        public ICommand OnMergeImageDocument { get; private set; }
        public ICommand OnLinkDocument { get; private set; }
        public ICommand OnDeleteDocument { get; private set; }

        public delegate void BeginLoading(object sender, EventArgs e);
        public event BeginLoading BeginLoadings;

        public delegate void FinishLoading(object sender, EventArgs e);
        public event FinishLoading FinishLoadings;

        public DocumentViewModel()
        {
            documentManagerContext = new Web.DocumentManagerDomainContext();
            TaxPayerDocumentList = new ObservableCollection<TaxPayerDocumentEntity>();

            OnAddSignalDocument = new DelegateCommand(onAddSignalDocument, canAddSignalDocument);
            OnAddMultiDocument = new DelegateCommand(onAddMultiDocument, canAddMultiDocument);
            OnMergeImageDocument = new DelegateCommand(onMergeImages, canMergeImages);
            OnLinkDocument = new DelegateCommand(onLinkDocument, canLinkDocument);
            OnDeleteDocument = new DelegateCommand(onDeleteDocument, canDeleteDocument);

            taxPayerDocumentSource = new EntityList<Web.Model.taxpayerdocument>(documentManagerContext.taxpayerdocuments);
            taxPayerDocumentLoader = new DomainCollectionViewLoader<Web.Model.taxpayerdocument>(
                LoadTaxPayerDocument,
                LoadTaxPayerDocument_Complete
                );
            taxPayerDocumentView = new DomainCollectionView<Web.Model.taxpayerdocument>(taxPayerDocumentLoader, taxPayerDocumentSource);

        }


        private LoadOperation<Web.Model.taxpayerdocument> LoadTaxPayerDocument()
        {
            BeginLoadings(null, null);
            EntityQuery<Web.Model.taxpayerdocument> lQuery = documentManagerContext.GetTaxpayerdocumentQuery();
            if (taxPayerEntity != null)
            {
                lQuery = lQuery.Where(c => c.taxpayer_id == taxPayerEntity.TaxPayerId);
            }
            else
            {
                lQuery = lQuery.Where(c => c.taxpayer_id == -100);
            }
            return documentManagerContext.Load(lQuery.SortAndPageBy(taxPayerDocumentView));
        }

        private void LoadTaxPayerDocument_Complete(LoadOperation<Web.Model.taxpayerdocument> sender)
        {
            TaxPayerDocumentList.Clear();
            taxPayerDocumentSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.taxpayerdocument taxpayerdocument in sender.Entities)
            {
                TaxPayerDocumentEntity taxPayerDocumentEntity = new TaxPayerDocumentEntity();
                taxPayerDocumentEntity.TaxPayerDocument = taxpayerdocument;
                taxPayerDocumentEntity.Update();

                taxPayerDocumentEntity.TaxPayerName = taxPayerEntity.TaxPayerName;
                FileTypeEntity lFileTypeEntity;
                if(FileTypeEntityDictionary != null
                    && taxPayerDocumentEntity.TaxPayerDocumentTypeId.HasValue
                    && FileTypeEntityDictionary.TryGetValue(taxPayerDocumentEntity.TaxPayerDocumentTypeId.Value, out lFileTypeEntity))
                {
                    taxPayerDocumentEntity.FileTypeName = lFileTypeEntity.FileTypeName;
                }

                UserEntity lUserEntity;
                if (UserEntityDictionary != null
                    && taxPayerDocumentEntity.TaxPayerUpdateUserId.HasValue
                    && UserEntityDictionary.TryGetValue(taxPayerDocumentEntity.TaxPayerUpdateUserId.Value, out lUserEntity))
                {
                    taxPayerDocumentEntity.UpdateUserName = lUserEntity.UserName;
                }

                TaxPayerDocumentList.Add(taxPayerDocumentEntity);
            }
            UpdateChanged("TaxPayerDocumentList");
            FinishLoadings(null, null);
        }

        private void onAddSignalDocument()
        {
            AddSignalDocument lAddSignalDocument = new AddSignalDocument(taxPayerEntity, FileTypeEntityList);
            lAddSignalDocument.Closed += new EventHandler(lAddSignalDocument_Closed);
            lAddSignalDocument.Show();
        }

        private void lAddSignalDocument_Closed(object sender, EventArgs e)
        {
            using (taxPayerDocumentView.DeferRefresh())
            {
                taxPayerDocumentView.MoveToFirstPage();
            }
        }

        

        private bool canAddSignalDocument(object aObject)
        {
            return taxPayerEntity != null;
        }

        private void onAddMultiDocument()
        {
            AddMultiDocument lAddMultiDocument = new AddMultiDocument(taxPayerEntity, FileTypeEntityList);
            lAddMultiDocument.Closed += new EventHandler(lAddSignalDocument_Closed);
            lAddMultiDocument.Show();
        }

        private void onMergeImages()
        {
            MergeImages lMergeImages = new MergeImages(taxPayerEntity, FileTypeEntityList);
            lMergeImages.Closed += new EventHandler(lMergeImage_Closed);
            lMergeImages.Show();
        }

        private void lMergeImage_Closed(object sender, EventArgs e)
        {
            using (taxPayerDocumentView.DeferRefresh())
            {
                taxPayerDocumentView.MoveToFirstPage();
            }
        }

        private bool canAddMultiDocument(object aObject)
        {
            return taxPayerEntity != null;
        }

        private bool canMergeImages(object aObject)
        {
            return taxPayerEntity != null;
        }

        private void onLinkDocument()
        {
            LinkFileWindow linkFileWindow = new LinkFileWindow(FileTypeEntityDictionary);
            linkFileWindow.Closed +=new EventHandler(linkFileWindow_Closed);
            linkFileWindow.Show();
        }
       
        private bool canLinkDocument(object aObject)
        {
            return taxPayerEntity != null;
        }

        private void linkFileWindow_Closed(object sender, EventArgs e)
        {
            LinkFileWindow linkFileWindow = sender as LinkFileWindow;
            if(linkFileWindow.DialogResult == true)
            {
                LinkFileViewModel lLinkFileViewModel = linkFileWindow.DataContext as LinkFileViewModel;
                int rTaxPayerId = lLinkFileViewModel.SelectTaxPayerEntity.TaxPayerId;
                foreach (TaxPayerEntity taxPayerEntity in lLinkFileViewModel.TaxPayerEntityLinkList)
                {
                    if (rTaxPayerId == taxPayerEntity.TaxPayerId)
                    {
                        continue;
                    }

                    foreach (TaxPayerDocumentEntity taxPayerDocumentEntity in linkFileWindow.FileListDataGrid.SelectedItems)
                    {
                        TaxPayerDocumentEntity lTaxPayerDocumentEntity = new TaxPayerDocumentEntity();

                        App lApp = Application.Current as App;

                        lTaxPayerDocumentEntity.TaxPayerId = taxPayerEntity.TaxPayerId;
                        lTaxPayerDocumentEntity.TaxPayerDocumentName = taxPayerDocumentEntity.TaxPayerDocumentName;
                        lTaxPayerDocumentEntity.TaxPayerDocumentTypeId = taxPayerDocumentEntity.TaxPayerDocumentTypeId;
                        lTaxPayerDocumentEntity.TaxPayerDocumentDescript = taxPayerDocumentEntity.TaxPayerDocumentDescript;
                        lTaxPayerDocumentEntity.TaxPayerUpdateUserId = lApp.MainPageViewModel.User.UserID;
                        lTaxPayerDocumentEntity.TaxPayerUpdateTime = taxPayerDocumentEntity.TaxPayerUpdateTime;
                        lTaxPayerDocumentEntity.TaxPayerDocumentBytes = taxPayerDocumentEntity.TaxPayerDocumentBytes;
                        lTaxPayerDocumentEntity.TaxPayerIsLink = true;
                        lTaxPayerDocumentEntity.TaxPayerLinkId = rTaxPayerId;
                        lTaxPayerDocumentEntity.TaxPayerDocument = new Web.Model.taxpayerdocument();
                        lTaxPayerDocumentEntity.DUpdate();
                        documentManagerContext.taxpayerdocuments.Add(lTaxPayerDocumentEntity.TaxPayerDocument);
                        Log.AddLog(documentManagerContext, lTaxPayerDocumentEntity.ToString());
                    }
                }
                
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;

            }

        }

        private void onDeleteDocument()
        {
            ConfirmWindow lConfirmWindow = new ConfirmWindow("删除", "删除 " + SelectTaxPayerDocumentEntity.TaxPayerDocumentName);
            lConfirmWindow.Closed += lConfirmWindow_Closed;
            lConfirmWindow.Show();
        }

        void lConfirmWindow_Closed(object sender, EventArgs e)
        {
            ConfirmWindow lConfirmWindow = sender as ConfirmWindow;
            if (lConfirmWindow.DialogResult == true)
            {
                BeginLoadings(null, null);
                documentManagerContext.taxpayerdocuments.Remove(SelectTaxPayerDocumentEntity.TaxPayerDocument);
                Log.DeleteLog(documentManagerContext, SelectTaxPayerDocumentEntity.ToString());
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
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
                using (taxPayerDocumentView.DeferRefresh())
                {
                    taxPayerDocumentView.MoveToFirstPage();
                }
            }
            FinishLoadings(null, null);
        }

        private bool canDeleteDocument(Object aObject)
        {
            if (SelectTaxPayerDocumentEntity != null)
            {
                App app = Application.Current as App;
                bool hasRight = false;
                app.MainPageViewModel.User.RightDictionary.TryGetValue(1000045, out hasRight);
                return hasRight;
            }
            else 
            {
                return false;
            }
        }
    }
}
