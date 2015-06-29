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
using Microsoft.Windows.Data.DomainServices;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Views;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class UserManagerViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.user> userView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.user> userLoader;
        private EntityList<DocumentManager.Web.Model.user> userSource;
        private Dictionary<String, UserEntity> UserEntityDictionary { get; set; }
        private UserEntity selectUserEntity;

        public ObservableCollection<UserEntity> UserList { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        public UserEntity AddUserEntity { get; set; }
        public UserEntity SelectUserEntity
        {
            get
            {
                return selectUserEntity;
            }
            set
            {
                if (selectUserEntity != value)
                {
                    selectUserEntity = value;
                    UpdateChanged("SelectUserEntity");
                    (OnModifyUser as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        public ICommand OnAddUser { get; private set; }
        public ICommand OnModifyUser { get; private set; }
        public ICommand OnDoubleClickList { get; private set; }

        public UserManagerViewModel()
        {
            UserList = new ObservableCollection<UserEntity>();
            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();
            OnAddUser = new DelegateCommand(onAddUser);
            OnModifyUser = new DelegateCommand(onModifyUser, canModifyUser);
            OnDoubleClickList = new DelegateCommand(onDoubleClickList);
        }

        public void LoadData()
        {
            IsBusy = true;

            this.userSource = new EntityList<DocumentManager.Web.Model.user>(this.documentManagerContext.users);
            this.userLoader = new DomainCollectionViewLoader<DocumentManager.Web.Model.user>(
                this.LoadUserEntities,
                this.loadOperation_Completed);
            this.userView = new DomainCollectionView<DocumentManager.Web.Model.user>(this.userLoader, this.userSource);

            using (this.userView.DeferRefresh())
            {
                this.userView.MoveToFirstPage();
            }
        }

        private LoadOperation<DocumentManager.Web.Model.user> LoadUserEntities()
        {
            this.IsBusy = true;
            EntityQuery<DocumentManager.Web.Model.user> lQuery = documentManagerContext.GetUserQuery();
            return documentManagerContext.Load(lQuery.SortAndPageBy(this.userView));
        }

        private void loadOperation_Completed(LoadOperation<DocumentManager.Web.Model.user> sender)
        {
            UserList.Clear();
            this.userSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.user user in sender.Entities)
            {
                if (user.user_name == "admin")
                {
                    continue;
                }
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                UserList.Add(userEntity);
            }
            UpdateChanged("UserList");
            IsBusy = false;
        }

        private void onAddUser()
        {
            AddUserEntity = new UserEntity();
            DocumentManager.Web.Model.user user = new DocumentManager.Web.Model.user();
            AddUserEntity.User = user;
            user.user_password = Cryptography.MD5CryptoServiceProvider.GetMd5String("123456");
            AddUserEntity.Update();
            UserWindow lUserWindow = new UserWindow(UserWindowType.MODIFY, AddUserEntity);
            lUserWindow.Closed += AddUser_Closed;
            lUserWindow.Show();
        }

        private void onModifyUser()
        {
            UserWindow lUserWindow = new UserWindow(UserWindowType.MODIFY, SelectUserEntity);
            lUserWindow.Closed += UserWindow_Closed;
            lUserWindow.Show();
        }

        private bool canModifyUser(object aObject)
        {
            return SelectUserEntity != null;
        }

        private void onDoubleClickList()
        {
            UserWindow lUserWindow = new UserWindow(UserWindowType.MODIFY, SelectUserEntity);
            lUserWindow.Closed += UserWindow_Closed;
            lUserWindow.Show();
        }

        private void AddUser_Closed(object sender, EventArgs e)
        {
            UserWindow lUserWindow = sender as UserWindow;
            if (lUserWindow.DialogResult == true)
            {
                IsBusy = true;
                UserList.Add(AddUserEntity);
                documentManagerContext.users.Add(AddUserEntity.User);
                Log.AddLog(documentManagerContext, AddUserEntity.ToString());
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        private void UserWindow_Closed(object sender, EventArgs e)
        {
            UserWindow lUserWindow = sender as UserWindow;
            if (lUserWindow.DialogResult == true)
            {
                IsBusy = true;
                Log.AddLog(documentManagerContext, SelectUserEntity.ToString());
                SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        void SubOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
                if (AddUserEntity != null)
                {
                    UserList.Remove(AddUserEntity);
                    AddUserEntity = null;
                }
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
                AddUserEntity = null;
                LoadData();
            }
            IsBusy = false;
        }
    }
}
