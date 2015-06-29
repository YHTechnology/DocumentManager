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
using System.Collections.ObjectModel;

using DocumentManager.Model.Entities;
using System.IO;
using FileHelper;
using DocumentManager.Controls;
using System.ServiceModel.DomainServices.Client;

using sharpPDF;
using sharpPDF.Enumerators;
using System.Windows.Media.Imaging;


namespace DocumentManager.ViewModels
{
    public class MergeImagesViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private TaxPayerEntity taxPayerEntity;
        private TaxPayerDocumentEntity taxPayerDocumentEntity;

        private string tempPDFFile = string.Empty;
        private bool createPDFFileSuccess = false;

        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }
        public ObservableCollection<AddImageEntity> ImagesList { get; set; }

        public UserFile UserFile { get; set; }

        public ICommand OnUpdate { get; private set; }
        public ICommand OnCancel { get; private set; }
        public ICommand OnCreatePDFFile { get; private set; }
        public ICommand OnSetPDFFileName { get; private set; }

        public FileTypeEntity SelectFileTypeEntity { get; set; }

        public int SelectedFileIndex { get; set; }

        private string fileName;
        public string PDFFileName
        {
            get
            {
                return fileName;
            }
            set
            {
                if (fileName != value)
                {
                    if (!value.ToUpper().EndsWith(".PDF"))
                    {
                        fileName = value + ".pdf";
                    }
                    UpdateChanged("PDFFileName");
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
                if (fileDescription != value)
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

        public MergeImagesViewModel(ChildWindow aChildWindow
                                          , TaxPayerEntity aTaxPayerEntity
                                          , ObservableCollection<FileTypeEntity> aFileTypeList)
        {
            childWindow = aChildWindow;
            documentManagerContext = new Web.DocumentManagerDomainContext();
            taxPayerEntity = aTaxPayerEntity;
            FileTypeEntityList = aFileTypeList;
            ImagesList = new ObservableCollection<AddImageEntity>();
            ShowProgress = Visibility.Collapsed;
            ShowUpdate = Visibility.Visible;
            OnUpdate = new DelegateCommand(onUpdate, canUpdate);
            OnCancel = new DelegateCommand(onCancel);
            OnCreatePDFFile = new DelegateCommand(onCreatePDFFile);
            
        }



        private void onUpdate()
        {
            ShowUpdate = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            UserFile.FinishUpdates += UserFile_FinishUpdate;
            UserFile.Upload(UserFile.FileFolder, childWindow.Dispatcher);
            createPDFFileSuccess = false;
        }

        private void UserFile_FinishUpdate(object sender, EventArgs e)
        {
            ShowProgress = Visibility.Collapsed;
            taxPayerDocumentEntity.TaxPayerDocumentName = PDFFileName;
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
            //return true;
            return (!String.IsNullOrWhiteSpace(PDFFileName) && createPDFFileSuccess);
        }

        private void onCancel()
        {
            childWindow.DialogResult = false;
        }

        private void onUp()
        {
            int i = SelectedFileIndex;

            if (i < 1 || i > ImagesList.Count -1)
            {
                return;
            }

            AddImageEntity tempImageEntity = ImagesList[i];

            ImagesList.RemoveAt(i);
            ImagesList.Insert(i - 1, tempImageEntity);
        }

        private void onDown()
        {
            int i = SelectedFileIndex;

            if (i < 0 || i > ImagesList.Count - 2)
            {
                return;
            }

            AddImageEntity tempImageEntity = ImagesList[i];

            ImagesList.RemoveAt(i);
            ImagesList.Insert(i + 1, tempImageEntity);
        }

        private void onDelete()
        {
            int i = SelectedFileIndex;
            if (i < 0 || i > ImagesList.Count - 1)
            {
                return;
            }

            ImagesList.RemoveAt(i);
        }

        private void onCreatePDFFile()
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请先输入生成PDF文件名称!");
                notifyWindow.Show();
                return;
            }

            if (ImagesList.Count == 0)
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请先添加文件!");
                notifyWindow.Show();
                return;
            }

            pdfDocument doc = new pdfDocument(fileName, "", false);
            bool getIamge = false;
            foreach (AddImageEntity imageEntity in ImagesList)
            {
                try
                {
                    WriteableBitmap bitmap;
                    using (FileStream fs = new FileStream(imageEntity.FilePath, FileMode.Open))
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.SetSource(fs);
                        bitmap = new WriteableBitmap(bi);
                    }

                    if (null != bitmap)
                    {
                        pdfPage page = doc.addPage((int)bitmap.PixelHeight, (int)bitmap.PixelWidth);
                        page.addImage(bitmap, 0, 0);
                        getIamge = true;
                        createPDFFileSuccess = true;
                    }
                    else
                    {
                        createPDFFileSuccess = false;
                    }
                }
                catch
                {
                    createPDFFileSuccess = false;

                    NotifyWindow notifyWindow = new NotifyWindow("错误", string.Format("打开图片文件{0}失败，请检查文件!!!", imageEntity.FilePath));
                    notifyWindow.Show();
                }

            }

            if (getIamge)
            {
                tempPDFFile = System.IO.Path.GetTempFileName();

                using (FileStream fs = new FileStream(tempPDFFile, FileMode.Create))
                {
                    doc.createPDF(fs);
                }


                UserFile = new UserFile();
                UserFile.FileName = fileName;
                UserFile.FileFolder = taxPayerEntity.TaxPayerId.ToString();
                UserFile.FileStream = (new FileInfo(tempPDFFile)).OpenRead();

                taxPayerDocumentEntity = new TaxPayerDocumentEntity();
                taxPayerDocumentEntity.TaxPayerDocumentBytes = UserFile.FileStream.Length;

                (OnUpdate as DelegateCommand).RaiseCanExecuteChanged();

                UpdateChanged("UserFile");
            }
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

        public void AddFile(FileInfo aFileInfo)
        {
            AddImageEntity imageEntity = new AddImageEntity();
            imageEntity.FilePath = aFileInfo.FullName;
            imageEntity.FileName = aFileInfo.Name;

            BitmapImage bi;
            try
            {
                using (FileStream fs = new FileStream(imageEntity.FilePath, FileMode.Open))
                {
                    bi = new BitmapImage();
                    bi.SetSource(fs);
                }
            }
            catch
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", string.Format("打开图片文件 {0} 失败，请检查文件!", imageEntity.FilePath));
                notifyWindow.Show();
                return;
            }


            imageEntity.ThumbImage = bi;

            imageEntity.UpCommand = new DelegateCommand(onUp);
            imageEntity.DownCommand = new DelegateCommand(onDown);
            imageEntity.DeleteCommand = new DelegateCommand(onDelete);


            ImagesList.Add(imageEntity);
            UpdateChanged("ImagesList");

        }
    }
}
