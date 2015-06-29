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

namespace DocumentManager.Views
{
    public partial class InputStandBookJA : ChildWindow
    {
        private InputStandBookJAViewModel InputStandBookJAViewModel;

        public InputStandBookJA()
        {
            InitializeComponent();
            InputStandBookJAViewModel = new InputStandBookJAViewModel(this);
            this.DataContext = InputStandBookJAViewModel;
        }


    }
}

