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
using System.IO;

namespace DocumentManager.ViewModels
{
    public class AddMultiDocumentViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private TaxPayerEntity taxPayerEntity;

        public ObservableCollection<MultiFileUpdateEntity> MutiFileUpdateEntityList { get; set; }
        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }
        public ICommand OnUpdate { get; private set; }
        public ICommand OnClose { get; private set; }
        public FileTypeEntity SelectFileTypeEntity { get; set; }

        public AddMultiDocumentViewModel(ChildWindow aChildWindow
                                        , TaxPayerEntity aTaxPayerEntity
                                        , ObservableCollection<FileTypeEntity> aFileTypeList)
        {
            childWindow = aChildWindow;
            taxPayerEntity = aTaxPayerEntity;
            FileTypeEntityList = aFileTypeList;
            MutiFileUpdateEntityList = new ObservableCollection<MultiFileUpdateEntity>();
            OnUpdate = new DelegateCommand(onUpdate, canUpdate);
            OnClose = new DelegateCommand(onClose);
        }

        private void onUpdate()
        {
            foreach (MultiFileUpdateEntity multiFileUpdateEntity in MutiFileUpdateEntityList)
            {
                if (multiFileUpdateEntity.MultiFileUpdateStatus == MultiFileUpdateStatus.PREPARE)
                {
                    multiFileUpdateEntity.MultiFileUpdateStatus = MultiFileUpdateStatus.UPDATING;
                    multiFileUpdateEntity.Status = "正在上传";
                    multiFileUpdateEntity.UserFile.Upload(multiFileUpdateEntity.UserFile.FileFolder, childWindow.Dispatcher);
                }
            }
        }

        private bool canUpdate(object aObject)
        {
            return true;
        }

        private void onClose()
        {
            childWindow.DialogResult = false;
        }

        public void AddFile(FileInfo aFileInfo)
        {
            MultiFileUpdateEntity lMultiFileUpdateEntity = new MultiFileUpdateEntity();
            lMultiFileUpdateEntity.TaxPayerDocumentEntity.TaxPayerDocumentTypeId = SelectFileTypeEntity.FileTypeId;
            lMultiFileUpdateEntity.TaxPayerDocumentEntity.FileTypeName = SelectFileTypeEntity.FileTypeName;
            lMultiFileUpdateEntity.TaxPayerDocumentEntity.TaxPayerId = taxPayerEntity.TaxPayerId;
            lMultiFileUpdateEntity.TaxPayerDocumentEntity.TaxPayerName = taxPayerEntity.TaxPayerName;
            lMultiFileUpdateEntity.TaxPayerDocumentEntity.TaxPayerDocumentName = aFileInfo.Name;
            lMultiFileUpdateEntity.UserFile.FileName = aFileInfo.Name;
            lMultiFileUpdateEntity.UserFile.FileFolder = taxPayerEntity.TaxPayerId.ToString();
            lMultiFileUpdateEntity.UserFile.FileStream = aFileInfo.OpenRead();
            lMultiFileUpdateEntity.TaxPayerDocumentEntity.TaxPayerDocumentBytes = lMultiFileUpdateEntity.UserFile.FileStream.Length;
            MutiFileUpdateEntityList.Add(lMultiFileUpdateEntity);
            UpdateChanged("MutiFileUpdateEntityList");
        }

        public bool canUpdateFile()
        {
            if (SelectFileTypeEntity != null)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
}
