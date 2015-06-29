﻿using System;
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
using DocumentManager.Model.Entities;
using System.Collections.ObjectModel;
using DocumentManager.ViewModels;
using System.IO;
using DocumentManager.Controls;

namespace DocumentManager.Views
{
    public partial class AddMultiDocument : ChildWindow
    {
        public AddMultiDocument(TaxPayerEntity aTaxPayerEntity
                                , ObservableCollection<FileTypeEntity> aFileTypeList)
        {
            InitializeComponent();
            this.DataContext = new AddMultiDocumentViewModel(this, aTaxPayerEntity, aFileTypeList);
        }

        private void MultiFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {

                AddMultiDocumentViewModel lAddMultiDocumentViewModel = this.DataContext as AddMultiDocumentViewModel;

                if (!lAddMultiDocumentViewModel.canUpdateFile())
                {
                    NotifyWindow notificationWindow = new NotifyWindow("错误", "请选择文件类型！");
                    notificationWindow.Show();
                    return;
                }
                
                var files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

                foreach (var file in files)
                {
                    string fileName = file.Name;

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
                        continue;
                    }

                    lAddMultiDocumentViewModel.AddFile(file);
                }
            }
        }
    }
}

