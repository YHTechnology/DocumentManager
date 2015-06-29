using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PDFTron.SilverDox.IO;
using PDFTron.SilverDox.Controls;
using System.Windows.Printing;

namespace DocumentManager.Controls
{
    public partial class ReviewWindow : ChildWindow
    {
        private String fileUrl;

        private List<Canvas> m_PageList;

        public ReviewWindow(String aFileUrl)
        {
            InitializeComponent();
            fileUrl = aFileUrl;
            m_PageList = new List<Canvas>();
        }

        public void LoadDocument()
        {
            try
            {
                //Busy.IsBusy = true;
                Uri documentUri = new Uri(fileUrl);
                HttpPartRetriever myHttpPartRetriever = new HttpPartRetriever(documentUri);
                this.MyDocumentViewer.LoadAsync(myHttpPartRetriever, OnLoadAsyncCallback);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        public void OnLoadAsyncCallback(Exception ex)
        {
            if (ex != null)
            {
                //An error has occurred
                Busy.IsBusy = false;
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                NotifyWindow lNotifyWIndow = new NotifyWindow("无法预览文件", "等待后台处理，请联系管理员！");
                lNotifyWIndow.Show();
                //Busy.IsBusy = false;
                //this.DialogResult = false;
            }

            MyDocumentViewer.SetFitMode(DocumentViewer.FitModes.Panel, DocumentViewer.FitModes.None);
            Busy.IsBusy = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.CurrentPageNumber -= 1;
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.CurrentPageNumber += 1;
        }

        private void ChildWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadDocument();
        }

        private void TrunLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.RotateCounterClockwise();
        }

        private void TrunRightButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.RotateClockwise();
        }

        private void ZoonInButton_Click(object sender, RoutedEventArgs e)
        {
            double lZoon = MyDocumentViewer.Zoom * 1.1;
            MyDocumentViewer.Zoom = lZoon;
        }

        private void ZoonOutButton_Click(object sender, RoutedEventArgs e)
        {
            double lZoon = MyDocumentViewer.Zoom / 1.1;
            MyDocumentViewer.Zoom = lZoon;
        }

        private void PointButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyDocumentViewer.Document.Print(false);
            }
            catch (Exception ex)
            {
                string lerror = ex.ToString();
            }
        }


    }
}

