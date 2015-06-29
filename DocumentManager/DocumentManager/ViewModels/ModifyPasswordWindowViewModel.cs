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
using DocumentManager.Controls;
using System.ServiceModel.DomainServices.Client;

namespace DocumentManager.ViewModels
{
    public class ModifyPasswordWindowViewModel : NotifyPropertyChanged
    {
        public ModifyPasswordEntity ModifyPasswordEntity { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        public ICommand OnModifyPassword { get; private set; }

        public ICommand OnCancel { get; private set; }

        private ChildWindow ChildWindow;

        public ModifyPasswordWindowViewModel(ChildWindow aChileWindow)
        {
            this.ChildWindow = aChileWindow;
            ModifyPasswordEntity = new ModifyPasswordEntity();
            OnModifyPassword = new DelegateCommand(OnModifyPasswordCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnModifyPasswordCommand()
        {
            if (ModifyPasswordEntity.Validate())
            {
                IsBusy = true;

                DocumentManager.Web.OperationDomainContext operationDomainContext = new DocumentManager.Web.OperationDomainContext();

                App app = Application.Current as App;

                if (app.MainPageViewModel.User.Password == Cryptography.MD5.GetMd5String(ModifyPasswordEntity.NewPassword))
                {
                    NotifyWindow notifyWindow = new NotifyWindow("密码验证", "请输入新密码");
                    notifyWindow.Show();
                    IsBusy = false;
                    return;
                }
                InvokeOperation<bool> lModifyPassword = operationDomainContext.ModifyPassword(app.MainPageViewModel.User.UserID, Cryptography.MD5.GetMd5String(ModifyPasswordEntity.NewPassword));
                lModifyPassword.Completed += ModifyPassword_Completed;
            }
        }

        void ModifyPassword_Completed(object sender, EventArgs e)
        {
            IsBusy = false;
            var lValue = (System.ServiceModel.DomainServices.Client.InvokeOperation<bool>)sender;
            if (lValue.Value)
            {
                App app = Application.Current as App;
                app.MainPageViewModel.User.Password = Cryptography.MD5.GetMd5String(ModifyPasswordEntity.NewPassword);
                ChildWindow.DialogResult = true;
                NotifyWindow notifyWindow = new NotifyWindow("修改成功", "密码修改成功");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("修改失败", "密码修改失败");
                notifyWindow.Show();
            }
        }

        private void OnCancelCommand()
        {
            ChildWindow.DialogResult = false;
        }
    }
}
