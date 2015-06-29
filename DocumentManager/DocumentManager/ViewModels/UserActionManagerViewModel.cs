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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public delegate void FinishLoaded();

    public class UserActionManagerViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext DocumentManagerDomainContext { get; set; }

        public List<UserEntity> UserList { get; set; }

        public ObservableCollection<ActionEntity> ActionEntityList { get; set; }

        public ObservableCollection<ActionAndUserActionEntity> ActionAndUserActionEntityList { get; set; }

        public Dictionary<int, ActionAndUserActionEntity> ActionAndUserActionEntityDictionary { get; set; }

        public ObservableCollection<UserActionEntity> UserActionEntityList { get; set; }

        public ActionAndUserActionEntity RootActionAndUserActionEntity { get; set; }

        private PagedCollectionView userDataView;
        public PagedCollectionView UserDataView
        {
            get
            {
                return userDataView;
            }
            set
            {
                if (userDataView != value)
                {
                    userDataView = value;
                    UpdateChanged("UserDataView");
                }
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        private FinishLoaded finishLoaded;

        public void SetCallBackLoaded(FinishLoaded afinishLoaded)
        {
            finishLoaded = afinishLoaded;
        }

        public void LoadData()
        {
            IsBusy = true;
            DocumentManagerDomainContext = new DocumentManager.Web.DocumentManagerDomainContext();
            DocumentManagerDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            DocumentManagerDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            LoadOperation<DocumentManager.Web.Model.user> loadOperationUser =
                DocumentManagerDomainContext.Load<DocumentManager.Web.Model.user>(DocumentManagerDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperation_Completed;
        }

        void loadOperation_Completed(object sender, EventArgs e)
        {
            UserList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (DocumentManager.Web.Model.user user in loadOperation.Entities)
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

            PagedCollectionView lPagedCollectionView = new PagedCollectionView(UserList);
            UserDataView = lPagedCollectionView;
            UserDataView.Refresh();
            UpdateChanged("UserList");
            IsBusy = false;
            //finishLoaded();
        }

        void UpdateAction()
        {
            IsBusy = true;
            LoadOperation<DocumentManager.Web.Model.action> loadOperationAction
                = DocumentManagerDomainContext.Load<DocumentManager.Web.Model.action>(DocumentManagerDomainContext.GetActionQuery());
            loadOperationAction.Completed += LoadOperation_ActionCompleted;
        }

        void LoadOperation_ActionCompleted(object sender, EventArgs e)
        {
            ActionEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (DocumentManager.Web.Model.action action in loadOperation.Entities)
            {
                ActionEntity actionEntity = new ActionEntity();
                actionEntity.Action = action;
                actionEntity.Update();
                ActionEntityList.Add(actionEntity);
            }
            UpdateRoleAndRoleAction();
        }

        private UserEntity selectUserEntity = null;
        public UserEntity SelectUserEntity
        {
            get
            {
                return selectUserEntity;
            }
            set
            {
                if (selectUserEntity != value && value != null)
                {
                    selectUserEntity = value;
                    UpdateChanged("SelectUserEntity");
                    UpdateAction();
                }
            }
        }

        private void UpdateUserAction()
        {
            LoadOperation<DocumentManager.Web.Model.useraction> loadOperationRole
                           = DocumentManagerDomainContext.Load<DocumentManager.Web.Model.useraction>(DocumentManagerDomainContext.GetUserActionByUserIDQuery(selectUserEntity.UserId));
            loadOperationRole.Completed += loadOperation_UserActionCompleted;
        }

        void loadOperation_UserActionCompleted(object sender, EventArgs e)
        {
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (DocumentManager.Web.Model.useraction useraction in loadOperation.Entities)
            {
                UserActionEntity userActionEntity = new UserActionEntity();
                userActionEntity.UserAction = useraction;
                userActionEntity.Update();
                ActionAndUserActionEntity actionAndUserActionEntity;
                if (ActionAndUserActionEntityDictionary.TryGetValue(userActionEntity.ActionID.Value, out actionAndUserActionEntity))
                {
                    actionAndUserActionEntity.UserActionEntity = userActionEntity;
                }
            }

            RootActionAndUserActionEntity.ChildList.Clear();
            foreach (KeyValuePair<int, ActionAndUserActionEntity> actionAndUserActionEntityPair in ActionAndUserActionEntityDictionary)
            {
                actionAndUserActionEntityPair.Value.CurrentSelectUserEntity = SelectUserEntity;
                int supperActionID = actionAndUserActionEntityPair.Value.ActionEntity.SupperActionId.GetValueOrDefault(0);
                if (supperActionID == 0)
                {
                    RootActionAndUserActionEntity.ChildList.Add(actionAndUserActionEntityPair.Value);
                }
            }
            UpdateChanged("RootActionAndUserActionEntity");
            IsBusy = false;
        }

        private void UpdateRoleAndRoleAction()
        {
            ActionAndUserActionEntityDictionary.Clear();
            ActionAndUserActionEntityList.Clear();
            foreach (ActionEntity actionEntity in ActionEntityList)
            {
                ActionAndUserActionEntity actionAndUserActionEntity = new ActionAndUserActionEntity();
                actionAndUserActionEntity.ActionEntity = actionEntity;
                actionAndUserActionEntity.DocumentManagerDomainContext = DocumentManagerDomainContext;
                ActionAndUserActionEntityDictionary.Add(actionAndUserActionEntity.ActionEntity.ActionId, actionAndUserActionEntity);
            }

            foreach (KeyValuePair<int, ActionAndUserActionEntity> actionAndUserActionEntityPair in ActionAndUserActionEntityDictionary)
            {
                int supperActionID = actionAndUserActionEntityPair.Value.ActionEntity.SupperActionId.GetValueOrDefault(-1);
                if (supperActionID != 0)
                {
                    ActionAndUserActionEntity supperActionAndUserActionEntity;
                    if (ActionAndUserActionEntityDictionary.TryGetValue(supperActionID, out supperActionAndUserActionEntity))
                    {
                        AddRoleAndRoleAction(supperActionAndUserActionEntity, actionAndUserActionEntityPair.Value);
                    }
                }
            }
            UpdateUserAction();
        }

        private void AddRoleAndRoleAction(ActionAndUserActionEntity supperActionAndUserActionEntity, ActionAndUserActionEntity actionAndUserActionEntity)
        {
            if (supperActionAndUserActionEntity.ChildList == null)
            {
                supperActionAndUserActionEntity.ChildList = new ObservableCollection<ActionAndUserActionEntity>();
            }
            supperActionAndUserActionEntity.ChildList.Add(actionAndUserActionEntity);
            actionAndUserActionEntity.ParentActionAdnUserActionEntity = supperActionAndUserActionEntity;
        }

        public ICommand OnSave { get; private set; }

        public UserActionManagerViewModel()
        {
            ActionEntityList = new ObservableCollection<ActionEntity>();
            ActionAndUserActionEntityList = new ObservableCollection<ActionAndUserActionEntity>();
            ActionAndUserActionEntityDictionary = new Dictionary<int, ActionAndUserActionEntity>();
            UserList = new List<UserEntity>();
            RootActionAndUserActionEntity = new ActionAndUserActionEntity();
            RootActionAndUserActionEntity.ChildList = new ObservableCollection<ActionAndUserActionEntity>();
            OnSave = new DelegateCommand(OnSaveCommand, CanSave);
            
        }

        private bool CanSave(object aObject)
        {
            return DocumentManagerDomainContext.HasChanges;
        }

        private void OnSaveCommand()
        {
            IsBusy = true;
            SubmitOperation subOperation = DocumentManagerDomainContext.SubmitChanges();
            subOperation.Completed += subOperation_Completed;
        }

        void subOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
            }

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        public void ConfirmLeave()
        {
            if (DocumentManagerDomainContext.HasChanges)
            {
                //ConfirmWindow confirmWindow = new ConfirmWindow("保存", "有改变，是否保存？");
                //confirmWindow.Closed += new EventHandler(Confirm_Closed);
                //confirmWindow.Show();
            }
        }

        void Confirm_Closed(object sender, EventArgs e)
        {
            // ConfirmWindow confirmWindow = sender as ConfirmWindow;
            //if (confirmWindow.DialogResult == true)
            {
            //    IsBusy = true;
            //    DocumentManagerDomainContext.SubmitChanges();
            }
            //else
            {
            //    DocumentManagerDomainContext.RejectChanges();
            }
        }
    }
}
