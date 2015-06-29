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
using FileHelper;
using DocumentManager.Model.Entities;
using System.Collections.ObjectModel;
using DocumentManager.Controls;
using System.ServiceModel.DomainServices.Client;

namespace DocumentManager.ViewModels
{
    public class AddSignalDocumentViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private TaxPayerEntity taxPayerEntity;
        private TaxPayerDocumentEntity taxPayerDocumentEntity;
        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }
        
        public UserFile UserFile { get; set; }

        public ICommand OnUpdate { get; private set; }
        public ICommand OnCancel { get; private set; }
        public ICommand OnOpenFile { get; private set; }

        public FileTypeEntity SelectFileTypeEntity { get; set; }
        

        private string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    UpdateChanged("FileName");
                }
            }
        }

        private string fileDescription;
        public string FileDescription
        {
            get
            {
                return fileDescription;
            }
            set
            {
                if(fileDescription != value)
                {
                    fileDescription = value;
                    UpdateChanged("FileDescription");
                }

            }
        }

        private Visibility showProgess;
        public Visibility ShowProgress
        {
            get { return showProgess; }
            set
            {
                if (showProgess != value) 
                { 
                    showProgess = value;
                    UpdateChanged("ShowProgress");
                }
            }
        }

        private Visibility showUpdate;
        public Visibility ShowUpdate
        {
            get { return showUpdate; }
            set
            {
                if (showUpdate != value)
                {
                    showUpdate = value;
                    UpdateChanged("ShowUpdate");
                }
            }
        }

        public AddSignalDocumentViewModel(ChildWindow aChildWindow
                                          , TaxPayerEntity aTaxPayerEntity
                                          , ObservableCollection<FileTypeEntity> aFileTypeList)
        {
            childWindow = aChildWindow;
            documentManagerContext = new Web.DocumentManagerDomainContext();
            taxPayerEntity = aTaxPayerEntity;
            FileTypeEntityList = aFileTypeList;
            ShowProgress = Visibility.Collapsed;
            ShowUpdate = Visibility.Visible;
            OnUpdate = new DelegateCommand(onUpdate, canUpdate);
            OnOpenFile = new DelegateCommand(onOpenFile);
            OnCancel = new DelegateCommand(onCancel);
        }

        private void onOpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                String fileName = ofd.File.Name;

                if (fileName.Contains("\"")
                    || fileName.Contains("#")
                    || fileName.Contains("%")
                    || fileName.Contains("&")
                    || fileName.Contains("\'")
                    || fileName.Contains("~")
                    || fileName.Contains("|")
                    || fileName.Contains(">")
                    || fileName.Contains("<")
                    || fileName.Contains("[")
                    || fileName.Contains("]")
                    || fileName.Contains("^")
                    || fileName.Contains("{")
                    || fileName.Contains("}"))
                {
                    NotifyWindow notificationWindow = new NotifyWindow("错误", "文件名包含 \"#%&\'~|><[]^{} 等非法字符！");
                    notificationWindow.Show();
                    return;
                }

                
                FileName = fileName;
                UserFile = new UserFile();
                UserFile.FileName = fileName;
                UserFile.FileFolder = taxPayerEntity.TaxPayerId.ToString();
                UserFile.FileStream = ofd.File.OpenRead();
                taxPayerDocumentEntity = new TaxPayerDocumentEntity();
                taxPayerDocumentEntity.TaxPayerDocumentBytes = UserFile.FileStream.Length;
                //ProjectFilesEntity.FileBytes = UserFile.FileStream.Length;
                (OnUpdate as DelegateCommand).RaiseCanExecuteChanged();
                UpdateChanged("UserFile");
            }
        }

        
        private void onUpdate()
        {
            ShowUpdate = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            UserFile.FinishUpdates += UserFile_FinishUpdate;
            UserFile.Upload(UserFile.FileFolder, childWindow.Dispatcher);
        }

        private void UserFile_FinishUpdate(object sender, EventArgs e)
        {
            ShowProgress = Visibility.Collapsed;
            taxPayerDocumentEntity.TaxPayerDocumentName = FileName;
            taxPayerDocumentEntity.TaxPayerDocumentTypeId = SelectFileTypeEntity.FileTypeId;
            taxPayerDocumentEntity.TaxPayerId = taxPayerEntity.TaxPayerId;
            taxPayerDocumentEntity.TaxPayerDocumentDescript = FileDescription;
            taxPayerDocumentEntity.TaxPayerUpdateTime = DateTime.Now;
            App app = Application.Current as App;
            taxPayerDocumentEntity.TaxPayerUpdateUserId = app.MainPageViewModel.User.UserID;
            taxPayerDocumentEntity.TaxPayerDocument = new Web.Model.taxpayerdocument();
            taxPayerDocumentEntity.DUpdate();

            documentManagerContext.taxpayerdocuments.Add(taxPayerDocumentEntity.TaxPayerDocument);
            Log.AddLog(documentManagerContext, taxPayerDocumentEntity.ToString());
            SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
            lSubmitOperation.Completed += SubOperation_Completed;
        }

        private bool canUpdate(object aObject)
        {
            return !String.IsNullOrWhiteSpace(FileName);
        }

        private void onCancel()
        {
            childWindow.DialogResult = false;
        }

        void SubOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "上传失败 " + submitOperation.Error);
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("上传成功", "上传成功！");
                notifyWindow.Show();
            }
       }
    }
}
