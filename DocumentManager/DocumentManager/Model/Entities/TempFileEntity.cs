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
    public class TempFileEntity
    {
        public string FileTypeName { get; set; }

        public string FileServerPath { get; set; }

        public string Filter { get; set; }

        public string DefaultFileName { get; set; }

        public override string ToString()
        {
            return FileTypeName;
            //return base.ToString();
        }
    }
}
