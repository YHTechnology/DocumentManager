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
using DocumentManager.ViewModels;
using DocumentManager.Model.Entities;

namespace DocumentManager.Views
{
    public partial class FileTypeWindow : ChildWindow
    {
        public FileTypeWindow(FileTypeWindowType aFileTypeWindowType, FileTypeEntity aFileTypeEntity)
        {
            InitializeComponent();
            this.DataContext = new FileTypeWindowViewModel(this, aFileTypeWindowType, aFileTypeEntity);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

