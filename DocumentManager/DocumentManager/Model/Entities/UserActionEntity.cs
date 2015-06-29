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

namespace DocumentManager.Model.Entities
{
    public class UserActionEntity : NotifyPropertyChanged
    {
        private int userActionID;
        public int UserActionID
        {
            get { return userActionID; }
            set { if (userActionID != value) { userActionID = value; UpdateChanged("UserActionID"); } }
        }

        private Nullable<int> userID;
        public Nullable<int> UserID
        {
            get { return userID; }
            set { if (userID != value) { userID = value; UpdateChanged("UserID"); } }
        }

        private Nullable<int> actionID;
        public Nullable<int> ActionID
        {
            get { return actionID; }
            set { if (actionID != value) { actionID = value; UpdateChanged("ActionID"); } }
        }

        private Nullable<bool> hasRight;
        public Nullable<bool> HasRight
        {
            get { return hasRight; }
            set { if (hasRight != value) { hasRight = value; UpdateChanged("HasRight"); } }
        }

        public void Update()
        {
            this.UserActionID = UserAction.user_action_id;
            this.UserID = UserAction.user_id;
            this.ActionID = UserAction.action_id;
            this.HasRight = UserAction.hasRight;
        }

        public void DUpdate()
        {
            UserAction.user_action_id = this.UserActionID;
            UserAction.user_id = this.UserID;
            UserAction.action_id = this.ActionID;
            UserAction.hasRight = this.HasRight;
        }

        public void RaisALL()
        {
            UpdateChanged("UserActionID");
            UpdateChanged("UserID");
            UpdateChanged("ActionID");
            UpdateChanged("HasRight");
        }

        public string ToString()
        {
            return string.Format("UserActionID:{0},UserID:{1},ActionID:{2},HasRight:{3}"
                , UserActionID, UserID, ActionID, HasRight);
        }

        public DocumentManager.Web.Model.useraction UserAction { get; set; }
    }
}
