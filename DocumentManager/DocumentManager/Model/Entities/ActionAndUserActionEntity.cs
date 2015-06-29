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

namespace DocumentManager.Model.Entities
{
    public class ActionAndUserActionEntity : NotifyPropertyChanged
    {
        public ICommand OnCheckBox { get; set; }

        public bool IsAccessSet { set; get; }
        public bool IsAccess
        {
            get
            {
                if (UserActionEntity != null)
                {
                    return UserActionEntity.HasRight.GetValueOrDefault(false);
                }
                return false;
            }
            set
            {
                IsAccessSet = value;
            }

        }

        private UserActionEntity userActionEntity;
        public UserActionEntity UserActionEntity
        {
            get
            {
                return userActionEntity;
            }
            set
            {
                if (userActionEntity != value)
                {
                    userActionEntity = value;
                    UpdateChanged("IsAccess");
                    UpdateChanged("UserActionEntity");
                }
            }
        }

        public ActionEntity ActionEntity { get; set; }

        public ObservableCollection<ActionAndUserActionEntity> ChildList { get; set; }

        public ActionAndUserActionEntity ParentActionAdnUserActionEntity { get; set; }

        public DocumentManager.Web.DocumentManagerDomainContext DocumentManagerDomainContext { get; set; }

        public UserEntity CurrentSelectUserEntity { get; set; }

        public ActionAndUserActionEntity()
        {
            OnCheckBox = new DelegateCommand(OnCheckBoxCommand);
        }

        private void CheckBoxCommand()
        {
            if (UserActionEntity == null)
            {
                UserActionEntity = new UserActionEntity();
                UserActionEntity.HasRight = IsAccessSet;
                UserActionEntity.ActionID = ActionEntity.ActionId;
                UserActionEntity.UserID = CurrentSelectUserEntity.UserId;
                UserActionEntity.UserAction = new DocumentManager.Web.Model.useraction();
                UserActionEntity.DUpdate();
                DocumentManagerDomainContext.useractions.Add(UserActionEntity.UserAction);
                UpdateChanged("IsAccess");
            }
            else
            {
                UserActionEntity.HasRight = IsAccessSet;
                UserActionEntity.DUpdate();
            }
        }

        private void OnCheckBoxCommand()
        {
            if (UserActionEntity == null)
            {
                UserActionEntity = new UserActionEntity();
                UserActionEntity.HasRight = IsAccessSet;
                UserActionEntity.ActionID = ActionEntity.ActionId;
                UserActionEntity.UserID = CurrentSelectUserEntity.UserId;
                UserActionEntity.UserAction = new DocumentManager.Web.Model.useraction();
                UserActionEntity.DUpdate();
                DocumentManagerDomainContext.useractions.Add(UserActionEntity.UserAction);
                UpdateChanged("IsAccess");
            }
            else
            {
                UserActionEntity.HasRight = IsAccessSet;
                UserActionEntity.DUpdate();
            }

            if (!IsAccessSet)
            {
                if (ChildList != null)
                {
                    foreach (ActionAndUserActionEntity actionAndUserActionEntity in ChildList)
                    {
                        if (actionAndUserActionEntity.UserActionEntity != null)
                        {
                            actionAndUserActionEntity.UserActionEntity.HasRight = IsAccessSet;
                            actionAndUserActionEntity.IsAccessSet = IsAccessSet;
                            if (actionAndUserActionEntity.ChildList != null)
                            {
                                actionAndUserActionEntity.OnCheckBoxCommand();
                            }
                            else
                            {
                                actionAndUserActionEntity.CheckBoxCommand();
                            }
                            actionAndUserActionEntity.UpdateChanged("IsAccess");
                        }
                    }
                }
            }

            if (IsAccessSet)
            {
                if (ChildList != null)
                {
                    foreach (ActionAndUserActionEntity actionAndUserActionEntity in ChildList)
                    {
                        actionAndUserActionEntity.IsAccessSet = IsAccessSet;
                        actionAndUserActionEntity.OnCheckBoxCommand();
                    }
                }
            }

            if (ParentActionAdnUserActionEntity != null)
            {
                if (IsAccessSet == true)
                {
                    if (ParentActionAdnUserActionEntity.IsAccessSet == false)
                    {
                        ParentActionAdnUserActionEntity.IsAccessSet = true;
                        ParentActionAdnUserActionEntity.CheckBoxCommand();
                    }
                    ParentActionAdnUserActionEntity.UpdateChanged("IsAccess");
                }
            }

            UpdateChanged("IsAccess");
        }
    }
}
