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
    public class ActionEntity : NotifyPropertyChanged
    {
        private int actionId;
        private string actionName;
        private Nullable<int> supperActionId;

        public int ActionId
        {
            get { return actionId; }
            set { if (actionId != value) { actionId = value; UpdateChanged("ActionId"); } }
        }

        public string ActionName
        {
            get { return actionName; }
            set { if (actionName != value) { actionName = value; UpdateChanged("ActionName"); } }
        }

        public Nullable<int> SupperActionId
        {
            get { return supperActionId; }
            set { if (supperActionId != value) { supperActionId = value; UpdateChanged("SupperActionId"); } }
        }

        public void Update()
        {
            ActionId = Action.action_id;
            ActionName = Action.action_name;
            SupperActionId = Action.supper_action_id;
        }

        public void DUpdate()
        {
            Action.action_id= ActionId;
            Action.action_name = ActionName;
            Action.supper_action_id = SupperActionId;
        }

        public void RaisALL()
        {
            UpdateChanged("ActionId");
            UpdateChanged("ActionName");
            UpdateChanged("SupperActionId");
        }

        public DocumentManager.Web.Model.action Action { get; set; }
    }
}
