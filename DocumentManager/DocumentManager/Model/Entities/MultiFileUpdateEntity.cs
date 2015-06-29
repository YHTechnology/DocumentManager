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
using System.ServiceModel.DomainServices.Client;
using DocumentManager.ViewModels;

namespace DocumentManager.Model.Entities
{
    public enum MultiFileUpdateStatus : uint
    {
        PREPARE = 0,
        UPDATING = 1,
        FINISH = 2
    }

    public class MultiFileUpdateEntity : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;

        public TaxPayerDocumentEntity TaxPayerDocumentEntity { get; set; }
        public UserFile UserFile { get; set; }
        public MultiFileUpdateStatus MultiFileUpdateStatus { get; set; }
        private string status;
        public string Status 
        {
            get { return status; }
            set { if (status != value) { status = value; UpdateChanged("Status"); } }
        }

        public ICommand OnMove { get; private set; }

        public MultiFileUpdateEntity()
        {
            documentManagerContext = new Web.DocumentManagerDomainContext(); 
            TaxPayerDocumentEntity = new TaxPayerDocumentEntity();
            TaxPayerDocumentEntity.TaxPayerDocument = new Web.Model.taxpayerdocument();
            UserFile = new UserFile();
            Status = "就绪";
            MultiFileUpdateStatus = Entities.MultiFileUpdateStatus.PREPARE;
            UserFile.FinishUpdates += UserFile_FinishUpdate;
            OnMove = new DelegateCommand(onMove);
        }

        private void onMove()
        {

        }

        private void UserFile_FinishUpdate(object sender, EventArgs e)
        {
            Status = "完成上传，正在存储";
            App app = Application.Current as App;
            TaxPayerDocumentEntity.TaxPayerUpdateUserId = app.MainPageViewModel.User.UserID;
            TaxPayerDocumentEntity.TaxPayerUpdateTime = DateTime.Now;
            TaxPayerDocumentEntity.DUpdate();
            documentManagerContext.taxpayerdocuments.Add(TaxPayerDocumentEntity.TaxPayerDocument);
            Log.AddLog(documentManagerContext, TaxPayerDocumentEntity.ToString());
            SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
            lSubmitOperation.Completed += SubOperation_Completed;
        }

        void SubOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                Status = "上传失败 " + submitOperation.Error;
            }
            else
            {
                Status = "上传成功";
                MultiFileUpdateStatus = Entities.MultiFileUpdateStatus.FINISH;
            }
        }
    }
}
