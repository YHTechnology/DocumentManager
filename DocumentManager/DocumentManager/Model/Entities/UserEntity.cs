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
using System.ComponentModel.DataAnnotations;

namespace DocumentManager.Model.Entities
{
    public class UserEntity : NotifyPropertyChanged
    {
        private int userId;
        private string userName;
        private string userPassword;
        private string userCName;

        public int UserId
        {
            get { return userId; }
            set { if (userId != value) { userId = value; UpdateChanged("UserId"); } }
        }

        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName
        {
            get { return userName; }
            set { if (userName != value) { userName = value; UpdateChanged("UserName"); } }
        }

        public string UserPassword
        {
            get { return userPassword; }
            set { if (userPassword != value) { userPassword = value; UpdateChanged("UserPassword"); } }
        }

        [Required(ErrorMessage = "姓名不能为空")]
        public string UserCName
        {
            get { return userCName; }
            set { if( userCName != value ) { userCName = value; UpdateChanged("UserCName"); } }
        }

        public void Update()
        {
            this.UserId = User.user_id;
            this.UserName = User.user_name;
            this.UserPassword = User.user_password;
            this.UserCName = User.user_cname;
        }

        public void DUpdate()
        {
            User.user_id = UserId;
            User.user_name = UserName;
            User.user_password = UserPassword;
            User.user_cname = UserCName;
        }

        public void RaisALL()
        {
            UpdateChanged("UserId");
            UpdateChanged("UserName");
            UpdateChanged("UserPassword");
            UpdateChanged("UserCName");
        }

        public string ToString()
        {
            return string.Format("UserId:{0},UserName:{1},UserCName:{2}"
                , UserId, UserName, UserCName);
        }

        public DocumentManager.Web.Model.user User { get; set; }
    }
}
