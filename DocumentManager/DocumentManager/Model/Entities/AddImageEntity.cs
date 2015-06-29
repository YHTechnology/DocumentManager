using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DocumentManager.Model.Entities
{
    public class AddImageEntity
    {
        public DelegateCommand UpCommand { get; set; }
        public DelegateCommand DownCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }

        public ImageSource ThumbImage { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
