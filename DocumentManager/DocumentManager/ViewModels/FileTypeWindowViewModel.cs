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

namespace DocumentManager.ViewModels
{
    public enum FileTypeWindowType : uint
    {
        ADD = 0,
        MODIFY = 1
    }

    public class FileTypeWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private FileTypeWindowType fileTypeWindowType;

        public FileTypeEntity FileTypeEntity { get; set; }

        public string Title { get; set; }
        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public FileTypeWindowViewModel(ChildWindow aChildWindow, FileTypeWindowType aFileTypeWindowType, FileTypeEntity aFileTypeEntity)
        {
            childWindow = aChildWindow;
            fileTypeWindowType = aFileTypeWindowType;
            FileTypeEntity = aFileTypeEntity;

            if (aFileTypeWindowType == FileTypeWindowType.ADD)
            {
                Title = "添加档案类型";
            }
            else
            {
                Title = "修改档案类型";
            }

            OnOK = new DelegateCommand(onOK);
            OnCancel = new DelegateCommand(onCancel);
        }

        public void onOK()
        {
            if (FileTypeEntity.Validate())
            {
                FileTypeEntity.DUpdate();
                FileTypeEntity.RaisALL();
                childWindow.DialogResult = true;
            }
        }

        public void onCancel()
        {
            FileTypeEntity.Update();
            FileTypeEntity.RaisALL();
            childWindow.DialogResult = false;
        }
    }
}
