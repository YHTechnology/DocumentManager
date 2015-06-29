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
    public enum UserWindowType : uint
    {
        ADD = 0,
        MODIFY = 1,
    }

    public class UserWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private UserWindowType userWindowType;

        public UserEntity UserEntity { get; set; }
        public string Title { get; set; }
        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }
        public ICommand OnClose { get; private set; }

        public UserWindowViewModel(ChildWindow aChildWindow, UserWindowType aUserWindowType, UserEntity aUserEntity)
        {
            childWindow = aChildWindow;
            userWindowType = aUserWindowType;
            UserEntity = aUserEntity;
            if( aUserWindowType == UserWindowType.ADD )
            {
                Title = "添加用户";
            }
            else
            {
                Title = "修改用户 用户名：" + UserEntity.UserName;
            }

            OnOK = new DelegateCommand(onOK);
            OnCancel = new DelegateCommand(onCancel);
            OnClose = new DelegateCommand(onClose);
        }

        public void onOK()
        {
            if (UserEntity.Validate())
            {
                UserEntity.DUpdate();
                UserEntity.RaisALL();
                childWindow.DialogResult = true;
            }
        }

        public void onCancel()
        {
            UserEntity.Update();
            UserEntity.RaisALL();
            childWindow.DialogResult = false;
        }

        public void onClose()
        {
            UserEntity.Update();
            UserEntity.RaisALL();
            childWindow.DialogResult = false;
        }
    }
}
