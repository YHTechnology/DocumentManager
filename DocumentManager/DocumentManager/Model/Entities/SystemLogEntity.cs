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
    public class SystemLogEntity : NotifyPropertyChanged
    {
        private int systemLogId;
        private string systemLog;

        public int SystemLogId
        {
            get { return systemLogId; }
            set { if (systemLogId != value) { systemLogId = value; UpdateChanged("SystemLogId"); } }
        }

        public string SystemLog
        {
            get { return systemLog; }
            set { if (systemLog != value) { systemLog = value; UpdateChanged("SystemLog"); } }
        }

        public void Update()
        {
            SystemLogId = SystemLogData.system_log_id;
            SystemLog = SystemLogData.system_log;
        }

        public void DUpdate()
        {
            SystemLogData.system_log_id = SystemLogId;
            SystemLogData.system_log = SystemLog;
        }

        public void RaisALL()
        {
            UpdateChanged("SystemLogId");
            UpdateChanged("SystemLog");
        }

        public DocumentManager.Web.Model.systemlog SystemLogData { get; set; }



    }
}
