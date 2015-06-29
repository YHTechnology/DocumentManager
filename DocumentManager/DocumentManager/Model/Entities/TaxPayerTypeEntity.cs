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
    public class TaxPayerTypeEntity : NotifyPropertyChanged
    {
        private int taxPayerTypeId;
        private string taxPayerTypeName;

        public int TaxPayerTypeId
        {
            get { return taxPayerTypeId; }
            set { if (taxPayerTypeId != value) { taxPayerTypeId = value; UpdateChanged("TaxPayerTypeId"); } }
        }

        [Required(ErrorMessage = "纳税人类型名不能为空")]
        public string TaxPayerTypeName
        {
            get { return taxPayerTypeName; }
            set { if (taxPayerTypeName != value) { taxPayerTypeName = value; UpdateChanged("TaxPayerTypeName"); } }
        }

        public void Update()
        {
            this.TaxPayerTypeId = TaxPayerType.taxpayertype_id;
            this.TaxPayerTypeName = TaxPayerType.taxpayertype_name;
        }

        public void DUpdate()
        {
            TaxPayerType.taxpayertype_id = TaxPayerTypeId;
            TaxPayerType.taxpayertype_name = TaxPayerTypeName;
        }

        public void RaisALL()
        {
            UpdateChanged("TaxPayerTypeId");
            UpdateChanged("TaxPayerTypeName");
        }

        public string ToString()
        {
            return string.Format("TaxPayerTypeId:{0},TaxPayerTypeName:{1}"
                , TaxPayerTypeId, TaxPayerTypeName);
        }

        public DocumentManager.Web.Model.taxpayertype TaxPayerType { get; set; }

    }
}
