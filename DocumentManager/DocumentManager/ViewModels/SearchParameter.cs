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

namespace DocumentManager.ViewModels
{
    public class SearchParameter : NotifyPropertyChanged
    {
        public string TaxPayerID { get; set; }
        public string TaxPayerName { get; set; }
        public string TaxPayerYear { get; set; }
        public string TaxPayerProject { get; set; }

        public string[] GetList()
        {
            string[] lRet = new string[4];

            lRet[0] = TaxPayerID;
            lRet[1] = TaxPayerName;
            lRet[2] = TaxPayerYear;
            lRet[3] = TaxPayerProject;

            return lRet;
        }
    }
}
