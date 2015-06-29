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
using System.IO;

using DocumentManager.Model.Entities;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class DownloadTemplateFileViewModel : NotifyPropertyChanged
    {
        private SaveFileDialog m_SaveFileDialog;

        public ObservableCollection<TempFileEntity> TempFileList { get; set; }

        public TempFileEntity SelectedTempFileEntity { get; set; }

        public ICommand OnOpenSaveFileDialog { get; private set; }

        public DownloadTemplateFileViewModel()
        {
            TempFileList = new ObservableCollection<TempFileEntity>(){
                new TempFileEntity() { FileTypeName = "登记证", FileServerPath = "TempFiles/登记证_模板.xls", Filter = "XLS文件(.xls)|*.xls", DefaultFileName="登记证_模板.xls" },
                new TempFileEntity() { FileTypeName = "拨款报告", FileServerPath = "TempFiles/拨款报告_模板.xls", Filter = "XLS文件(.xls)|*.xls", DefaultFileName="拨款报告_模板.xls" },
                new TempFileEntity() { FileTypeName = "申报表", FileServerPath = "TempFiles/申报表_模板.xls", Filter = "XLS文件(.xls)|*.xls", DefaultFileName="申报表_模板.xls" }
            };

            SelectedTempFileEntity = TempFileList[0];

            OnOpenSaveFileDialog = new DelegateCommand(SaveFileTolocal);
        }

        private void SaveFileTolocal()
        {
            if (SelectedTempFileEntity == null)
            {
                return;
            }

            m_SaveFileDialog = new SaveFileDialog();
            m_SaveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(m_SaveFileDialog, new object[] { SelectedTempFileEntity.DefaultFileName });

            bool? saveResult = m_SaveFileDialog.ShowDialog();

            if (saveResult == null)
            {
                return;
            }

            if (saveResult == false)
            {
                return;
            }

            string appstr = System.Windows.Application.Current.Host.Source.AbsoluteUri;

            WebClient client = new WebClient();
            Uri uri = new Uri(getAbsPath(SelectedTempFileEntity.FileServerPath), UriKind.RelativeOrAbsolute);

            client.OpenReadCompleted += client_OpenReadCompleted;
            client.OpenReadAsync(uri);

        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                using (Stream sf = (Stream)m_SaveFileDialog.OpenFile())
                {
                    e.Result.CopyTo(sf);
                    sf.Flush();

                    NotifyWindow notificationWindow = new NotifyWindow("下载完成", "下载模板文件 " + SelectedTempFileEntity.DefaultFileName + " 完成");
                    notificationWindow.Show();
                }
            }
            else
            {
                NotifyWindow notificationWindow = new NotifyWindow("下载失败", "下载模板文件 " + SelectedTempFileEntity.DefaultFileName + " 失败");
                notificationWindow.Show();
            }
            //throw new NotImplementedException();
        }


        private string getAbsPath(string aPath)
        {
            if (string.IsNullOrEmpty(aPath))
            {
                return aPath;
            }

            string fullUrl;

            if (aPath.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
             || aPath.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
             || aPath.StartsWith("file:", StringComparison.OrdinalIgnoreCase)
                )
            {
                fullUrl = aPath;
            }
            else
            {
                fullUrl = System.Windows.Application.Current.Host.Source.AbsoluteUri;
                if (fullUrl.IndexOf("ClientBin") > 0)
                {
                    fullUrl = fullUrl.Substring(0, fullUrl.IndexOf("ClientBin")) + aPath;
                }
                else
                {
                    fullUrl = fullUrl.Substring(0, fullUrl.LastIndexOf("/") + 1) + aPath;
                }
            }

            return fullUrl;
        }
        

    }
}
