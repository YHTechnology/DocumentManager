using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DocumentManager.ViewModels;
using DocumentManager.Model.Entities;

namespace DocumentManager.Views
{
    public partial class UserWindow : ChildWindow
    {
        public UserWindow(UserWindowType aUserWindowType, UserEntity aUserEntity)
        {
            InitializeComponent();
            this.DataContext = new UserWindowViewModel(this, aUserWindowType, aUserEntity);
        }
    }
}

