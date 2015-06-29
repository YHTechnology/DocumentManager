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
using System.IO;
using DocumentManager.Controls;
using FileHelper;

namespace DocumentManager.Model.Entities
{
    public class TaxPayerDocumentEntity : NotifyPropertyChanged
    {
        private int taxPayerDocumentId;
        private Nullable<int> taxPayerId;
        private string taxPayerDocumentName;
        private Nullable<int> taxPayerDocumentTypeId;
        private string taxPayerDocumentDescript;
        private Nullable<int> taxPayerUpdateUserId;
        private Nullable<DateTime> taxPayerUpdateTime;
        private Nullable<long> taxPayerDocumentBytes;
        private Nullable<bool> taxPayerIsLink;
        private Nullable<int> taxPayerLinkId;

        public int TaxPayerDocumentId
        {
            get { return taxPayerDocumentId; }
            set { if (taxPayerDocumentId != value) { taxPayerDocumentId = value; UpdateChanged("TaxPayerDocumentId"); } }
        }

        public Nullable<int> TaxPayerId
        {
            get { return taxPayerId; }
            set { if (taxPayerId != value) { taxPayerId = value; UpdateChanged("TaxPayerId"); } }
        }

        public string TaxPayerDocumentName
        {
            get { return taxPayerDocumentName; }
            set { if (taxPayerDocumentName != value) { taxPayerDocumentName = value; UpdateChanged("TaxPayerDocumentName"); } }
        }

        public Nullable<int> TaxPayerDocumentTypeId
        {
            get { return taxPayerDocumentTypeId; }
            set { if (taxPayerDocumentTypeId != value) { taxPayerDocumentTypeId = value; UpdateChanged("TaxPayerDocumentTypeId"); } }
        }

        public string TaxPayerDocumentDescript
        {
            get { return taxPayerDocumentDescript; }
            set { if (taxPayerDocumentDescript != value) { taxPayerDocumentDescript = value; UpdateChanged("TaxPayerDocumentDescript"); } }
        }

        public Nullable<int> TaxPayerUpdateUserId
        {
            get { return taxPayerUpdateUserId; }
            set { if (taxPayerUpdateUserId != value) { taxPayerUpdateUserId = value; UpdateChanged("TaxPayerUpdateUserId"); } }
        }

        public Nullable<DateTime> TaxPayerUpdateTime
        {
            get { return taxPayerUpdateTime; }
            set { if (taxPayerUpdateTime != value) { taxPayerUpdateTime = value; UpdateChanged("TaxPayerUpdateTime"); } }
        }

        public Nullable<long> TaxPayerDocumentBytes
        {
            get { return taxPayerDocumentBytes; }
            set { if (taxPayerDocumentBytes != value) { taxPayerDocumentBytes = value; UpdateChanged("TaxPayerDocumentBytes"); } }
        }

        public Nullable<bool> TaxPayerIsLink
        {
            get { return taxPayerIsLink; }
            set { if (taxPayerIsLink != value) { taxPayerIsLink = value; UpdateChanged("TaxPayerIsLink"); } }
        }

        public Nullable<int> TaxPayerLinkId
        {
            get { return taxPayerLinkId; }
            set { if (taxPayerLinkId != value) { taxPayerLinkId = value; UpdateChanged("TaxPayerLinkId"); } }
        }

        public string TaxPayerName { get; set; }
        public string FileTypeName { get; set; }
        public string UpdateUserName { get; set; }

        public TaxPayerEntity TaxPayerEntity { get; set; }

        public ICommand OnDownload { get; private set; }
        public ICommand OnView { get; private set; }

        private string fileUrl;
        
        private Visibility downLoading = Visibility.Collapsed;
        public Visibility DownLoading
        {
            get
            {
                return downLoading;
            }
            set
            {
                if (downLoading != value)
                {
                    downLoading = value;
                    UpdateChanged("DownLoading");
                }
            }
        }

        private int downloadPer;
        public int DownloadPer
        {
            get
            {
                return downloadPer;
            }
            set
            {
                if (downloadPer != value)
                {
                    downloadPer = value;
                    UpdateChanged("DownloadPer");
                }
            }
        }

        private SaveFileDialog saveFileDialog;

        public TaxPayerDocumentEntity()
        {
            OnDownload = new DelegateCommand(onDownload);
            OnView = new DelegateCommand(onView, canView);
        }

        private void onDownload()
        {
            if (fileUrl == null)
            {
                if (TaxPayerIsLink.GetValueOrDefault(false))
                {
                    fileUrl = (CustomUri.GetAbsoluteUrl("upload/" + TaxPayerLinkId.Value) + "/" + TaxPayerDocumentName);
                }
                else
                {
                    fileUrl = (CustomUri.GetAbsoluteUrl("upload/" + TaxPayerId.Value) + "/" + TaxPayerDocumentName);
                }
            }

            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All Files|*.*";
                saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { TaxPayerDocumentName });
                bool? dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult != true) return;
                WebClient client = new WebClient();
                Uri uri = new Uri(fileUrl, UriKind.RelativeOrAbsolute);
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                client.OpenReadAsync(uri);
                DownLoading = Visibility.Visible;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                using (Stream sf = (Stream)saveFileDialog.OpenFile())
                {
                    e.Result.CopyTo(sf);
                    sf.Flush();
                    sf.Close();
                    DownLoading = Visibility.Collapsed;
                    NotifyWindow notifyWindow = new NotifyWindow("下载完成", "下载完成！");
                    notifyWindow.Show();
                }
            }
            saveFileDialog = null;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadPer = e.ProgressPercentage;
        }

        private void onView()
        {
            if (fileUrl == null)
            {
                if (TaxPayerIsLink.GetValueOrDefault(false))
                {
                    fileUrl = (CustomUri.GetAbsoluteUrl("upload/" + TaxPayerLinkId.Value) + "/" + TaxPayerDocumentName);
                }
                else
                {
                    fileUrl = (CustomUri.GetAbsoluteUrl("upload/" + TaxPayerId.Value) + "/" + TaxPayerDocumentName);
                }
            }
            ReviewWindow review = new ReviewWindow(fileUrl + ".xod");
            review.Show();
        }

        private bool canView(object aObject)
        {
            if (fileUrl == null)
            {
                if (TaxPayerIsLink.GetValueOrDefault(false))
                {
                    fileUrl = (CustomUri.GetAbsoluteUrl("upload/" + TaxPayerLinkId.Value) + "/" + TaxPayerDocumentName);
                }
                else
                {
                    fileUrl = (CustomUri.GetAbsoluteUrl("upload/" + TaxPayerId.Value) + "/" + TaxPayerDocumentName);
                }
            }

            if (fileUrl != null)
            {
                int lastDot = fileUrl.LastIndexOf(".");
                if (lastDot <= 0)
                {
                    return false;
                }
                String lExt = fileUrl.Substring(lastDot, fileUrl.Length - lastDot).ToLower();
                if (lExt == ".pdf"
                   || lExt == ".doc"
                   || lExt == ".docx"
                   || lExt == ".xls"
                   || lExt == ".xlsx"
                   || lExt == ".jpeg"
                   || lExt == ".jpg"
                   || lExt == ".png"
                   || lExt == ".bmp"
                   || lExt == ".tiff"
                   || lExt == ".tif"
                   || lExt == ".gif")
                {
                    if (!TaxPayerUpdateTime.HasValue)
                    {
                        return false;
                    }
                    DateTime lToday = DateTime.Now;
                    if (lToday.Year == TaxPayerUpdateTime.Value.Year && lToday.Month == TaxPayerUpdateTime.Value.Month && lToday.Day == TaxPayerUpdateTime.Value.Day)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Update()
        {
            TaxPayerDocumentId = TaxPayerDocument.taxpayer_document_id;
            TaxPayerId = TaxPayerDocument.taxpayer_id;
            TaxPayerDocumentName = TaxPayerDocument.taxpayer_document_name;
            TaxPayerDocumentTypeId = TaxPayerDocument.taxpayer_document_type_id;
            TaxPayerDocumentDescript = TaxPayerDocument.taxpayer_document_descript;
            TaxPayerUpdateUserId = TaxPayerDocument.taxpayer_update_user_id;
            TaxPayerUpdateTime = TaxPayerDocument.taxpayer_update_time;
            TaxPayerDocumentBytes = TaxPayerDocument.taxpayer_document_bytes;
            TaxPayerIsLink = TaxPayerDocument.taxpayer_islink;
            TaxPayerLinkId = TaxPayerDocument.taxpayer_linkid;
        }

        public void DUpdate()
        {
            TaxPayerDocument.taxpayer_document_id = TaxPayerDocumentId;
            TaxPayerDocument.taxpayer_id = TaxPayerId;
            TaxPayerDocument.taxpayer_document_name = TaxPayerDocumentName;
            TaxPayerDocument.taxpayer_document_type_id = TaxPayerDocumentTypeId;
            TaxPayerDocument.taxpayer_document_descript = TaxPayerDocumentDescript;
            TaxPayerDocument.taxpayer_update_user_id = TaxPayerUpdateUserId;
            TaxPayerDocument.taxpayer_update_time = TaxPayerUpdateTime;
            TaxPayerDocument.taxpayer_document_bytes = TaxPayerDocumentBytes;
            TaxPayerDocument.taxpayer_islink = TaxPayerIsLink;
            TaxPayerDocument.taxpayer_linkid = TaxPayerLinkId;
        }

        public void RaisALL()
        {
            UpdateChanged("TaxPayerDocumentId");
            UpdateChanged("TaxPayerId");
            UpdateChanged("TaxPayerDocumentName");
            UpdateChanged("TaxPayerDocumentTypeId");
            UpdateChanged("TaxPayerDocumentDescript");
            UpdateChanged("TaxPayerUpdateUserId");
            UpdateChanged("TaxPayerUpdateTime");
            UpdateChanged("TaxPayerDocumentBytes");
            UpdateChanged("TaxPayerIsLink");
            UpdateChanged("TaxPayerLinkId");
        }

        public string ToString()
        {
            return string.Format("TaxPayerDocumentId:{0},TaxPayerId:{1},TaxPayerDocumentName:{2},TaxPayerDocumentTypeId:{3},TaxPayerDocumentDescript:{4},TaxPayerUpdateUserId:{5},TaxPayerUpdateTime:{6},TaxPayerDocumentBytes:{7},{8},{9}"
                , TaxPayerDocumentId, TaxPayerId, TaxPayerDocumentName, TaxPayerDocumentTypeId, TaxPayerDocumentDescript, TaxPayerUpdateUserId, TaxPayerUpdateTime, TaxPayerDocumentBytes, TaxPayerIsLink, TaxPayerLinkId);
        }

        public DocumentManager.Web.Model.taxpayerdocument TaxPayerDocument { get; set; }
    }
}
