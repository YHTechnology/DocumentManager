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
using System.Collections.Generic;
using DocumentManager.Model.Entities;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Views;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class FileTypeManagerViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.filetype> fileTypeView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.filetype> fileTypeLoader;
        private EntityList<DocumentManager.Web.Model.filetype> fileTypeSource;
        private FileTypeEntity selectFileTypeEntity;
        private FileTypeEntity addFileTypeEntity;

        public ObservableCollection<FileTypeEntity> FileTypeList { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        public FileTypeEntity SelectFileTypeEntity
        {
            get
            {
                return selectFileTypeEntity;
            }
            set
            {
                if (selectFileTypeEntity != value)
                {
                    selectFileTypeEntity = value;
                    UpdateChanged("SelectFileTypeEntity");
                    (OnModifyFileType as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnAddFileType { get; private set; }
        public ICommand OnModifyFileType { get; private set; }
        public ICommand OnDoubleClickList { get; private set; }

        public FileTypeManagerViewModel()
        {
            FileTypeList = new ObservableCollection<FileTypeEntity>();
            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();

            OnAddFileType = new DelegateCommand(onAddFileType);
            OnModifyFileType = new DelegateCommand(onModifyFileType, canModifyFileType);
            OnDoubleClickList = new DelegateCommand(onDoubleClickList);
        }

        public void LoadData()
        {
            IsBusy = true;

            fileTypeSource = new EntityList<Web.Model.filetype>(documentManagerContext.filetypes);
            fileTypeLoader = new DomainCollectionViewLoader<Web.Model.filetype>(
                loadFileTypeEntities,
                loadOperation_Completed);
            fileTypeView = new DomainCollectionView<Web.Model.filetype>(fileTypeLoader, fileTypeSource);

            using (this.fileTypeView.DeferRefresh())
            {
                fileTypeView.MoveToFirstPage();
            }
        }

        private LoadOperation<Web.Model.filetype> loadFileTypeEntities()
        {
            IsBusy = true;
            EntityQuery<Web.Model.filetype> lQuery = documentManagerContext.GetFiletypeQuery();
            return documentManagerContext.Load(lQuery.SortAndPageBy(fileTypeView));
        }

        private void loadOperation_Completed(LoadOperation<DocumentManager.Web.Model.filetype> sender)
        {
            FileTypeList.Clear();
            fileTypeSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.filetype filetype in sender.Entities)
            {
                FileTypeEntity fileTypeEntity = new FileTypeEntity();
                fileTypeEntity.FileType = filetype;
                fileTypeEntity.Update();
                FileTypeList.Add(fileTypeEntity);
            }
            UpdateChanged("FileTypeList");
            IsBusy = false;
        }

        private void onAddFileType()
        {
            addFileTypeEntity = new FileTypeEntity();
            Web.Model.filetype fileType = new Web.Model.filetype();
            addFileTypeEntity.FileType = fileType;
            addFileTypeEntity.Update();
            FileTypeWindow lFileTypeWindow = new FileTypeWindow(FileTypeWindowType.ADD, addFileTypeEntity);
            lFileTypeWindow.Closed += AddFileType_Closed;
            lFileTypeWindow.Show();
        }

        private void onModifyFileType()
        {
            FileTypeWindow lFileTypeWindow = new FileTypeWindow(FileTypeWindowType.MODIFY, SelectFileTypeEntity);
            lFileTypeWindow.Closed += FileTypeWindow_Closed;
            lFileTypeWindow.Show();
        }

        private bool canModifyFileType(object aObject)
        {
            return SelectFileTypeEntity != null;
        }

        private void onDoubleClickList()
        {
            FileTypeWindow lFileTypeWindow = new FileTypeWindow(FileTypeWindowType.MODIFY, SelectFileTypeEntity);
            lFileTypeWindow.Closed += FileTypeWindow_Closed;
            lFileTypeWindow.Show();
        }

        private void AddFileType_Closed(object sender, EventArgs e)
        {
            FileTypeWindow lFileTypeWindow = sender as FileTypeWindow;
            if (lFileTypeWindow.DialogResult == true)
            {
                IsBusy = true;
                FileTypeList.Add(addFileTypeEntity);
                documentManagerContext.filetypes.Add(addFileTypeEntity.FileType);
                Log.AddLog(documentManagerContext, addFileTypeEntity.ToString());
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        private void FileTypeWindow_Closed(object sender, EventArgs e)
        {
            FileTypeWindow lFileTypeWindow = sender as FileTypeWindow;
            if (lFileTypeWindow.DialogResult == true)
            {
                IsBusy = true;
                Log.ModifyLog(documentManagerContext, selectFileTypeEntity.ToString());
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
                if (addFileTypeEntity != null)
                {
                    FileTypeList.Remove(addFileTypeEntity);
                    addFileTypeEntity = null;
                }
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
                addFileTypeEntity = null;
                LoadData();
            }
            IsBusy = false;
        }
    }
}
