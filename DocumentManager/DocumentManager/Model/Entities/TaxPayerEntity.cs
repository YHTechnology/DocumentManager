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
    public class TaxPayerEntity : NotifyPropertyChanged
    {
        private int taxPayerId;
        private string taxPayerCode;
        private string taxPayerName;
        private Nullable<int> taxPayerTypeId;
        private Nullable<int> taxPayerGroupId;
        private string taxPayerRegyear;
        private string taxPayerProject;
        private Nullable<bool> taxPayerProjectFinish;
        private Nullable<bool> taxPayerIsFree;
        private Nullable<bool> taxPayerFtk;

        public int TaxPayerId
        {
            get { return taxPayerId; }
            set { if (taxPayerId != value) { taxPayerId = value; UpdateChanged("TaxPayerId"); } }
        }

        public string TaxPayerCode
        {
            get { return taxPayerCode; }
            set { if (taxPayerCode != value) { taxPayerCode = value; UpdateChanged("TaxPayerCode"); } }
        }

        public string TaxPayerName
        {
            get { return taxPayerName; }
            set { if (taxPayerName != value) { taxPayerName = value; UpdateChanged("TaxPayerName"); } }
        }

        public Nullable<int> TaxPayerTypeId
        {
            get { return taxPayerTypeId; }
            set { if (taxPayerTypeId != value) { taxPayerTypeId = value; UpdateChanged("TaxPayerTypeId"); } }
        }

        public Nullable<int> TaxPayerGroupId
        {
            get { return taxPayerGroupId; }
            set { if (taxPayerGroupId != value) { taxPayerGroupId = value; UpdateChanged("TaxPayerGroupId"); UpdateChanged("TaxPayerGroup"); } }
        }

        [Range(2000,9999,ErrorMessage = "合理的年份 如2014，四位数字")]
        public string TaxPayerRegyear
        {
            get { return taxPayerRegyear; }
            set { if (taxPayerRegyear != value) { taxPayerRegyear = value; UpdateChanged("TaxPayerRegyear"); } }
        }

        public string TaxPayerProject
        {
            get { return taxPayerProject; }
            set { if (taxPayerProject != value) { taxPayerProject = value; UpdateChanged("TaxPayerProject"); } }
        }

        public Nullable<bool> TaxPayerProjectFinish
        {
            get 
            {
                if (taxPayerProjectFinish.HasValue)
                {
                    return taxPayerProjectFinish;
                }
                else
                {
                    return false;
                }
            }
            set { if (taxPayerProjectFinish != value) { taxPayerProjectFinish = value; UpdateChanged("TaxPayerProjectFinish"); } }
        }

        public Nullable<bool> TaxPayerIsFree
        {
            get
            {
                if (taxPayerIsFree.HasValue)
                {
                    return taxPayerIsFree;
                }
                else
                {
                    return false;
                }
            }
            set { if (taxPayerIsFree != value) { taxPayerIsFree = value; UpdateChanged("TaxPayerIsFree"); } }
        }

        public Nullable<bool> TaxPayerFtk
        {
            get
            {
                if (taxPayerFtk.HasValue)
                {
                    return taxPayerFtk;
                }
                else
                {
                    return false;
                }
            }
            set { if (taxPayerFtk != value) { taxPayerFtk = value; UpdateChanged("TaxPayerFtk"); } }
        }

        public string TaxPayerGroup
        {
            get 
            {
                string lRet = "";
                if (taxPayerGroupId.HasValue)
                {
                    switch (taxPayerGroupId.Value)
                    {
                        case 0:
                            lRet = "固定户";
                            break;
                        case 1:
                            lRet = "建安代开";
                            break;
                        case 2:
                            lRet = "普票代开";
                            break;
                        case 3:
                            lRet = "专票代开";
                            break;
                    }
                }
                return lRet;
            }
        }

        public string TaxPayerTypeName
        {
            get
            {
                if (TaxPayerTypeEntity != null)
                {
                    return TaxPayerTypeEntity.TaxPayerTypeName;
                }
                else
                {
                    return "";
                }
            }
        }

        public TaxPayerTypeEntity TaxPayerTypeEntity { get; set; }

        private decimal totalMoney;
        public decimal TotalMoney
        {
            get { return totalMoney; }
            set
            {
                if (totalMoney != value)
                {
                    totalMoney = value;
                    UpdateChanged("TotalMoney");
                    UpdateChanged("TotalMoneyStr");
                }
            }
        }

        public string TotalMoneyStr
        {
            get
            {
                if (totalMoney == 0)
                {
                    return "";
                }
                else
                {
                    return totalMoney.ToString("#0,##0.00");
                }
            }
        }

        private decimal partMoney;
        public decimal PartMoney
        {
            get { return partMoney; }
            set
            {
                if (partMoney != value)
                {
                    partMoney = value;
                    UpdateChanged("PartMoney");
                    UpdateChanged("PartMoneyStr");
                }
            }
        }

        public string PartMoneyStr
        {
            get
            {
                if (partMoney == 0)
                {
                    return "";
                }
                else
                {
                    return partMoney.ToString("#0,##0.00");
                }
            }
        }

        private decimal totalTax;
        public decimal TotalTax
        {
            get { return totalTax; }
            set
            {
                if (totalTax != value)
                {
                    totalTax = value;
                    UpdateChanged("TotalTax");
                    UpdateChanged("TotalTaxStr");
                }
            }
        }

        public string TotalTaxStr
        {
            get
            {
                if (totalTax == 0)
                {
                    return "";
                }
                else
                {
                    return totalTax.ToString("#0,##0.00");
                }
            }
        }

        private DateTime firstStandBookDateTime;
        public DateTime FirstStandBookDateTime
        {
            set
            {
                if (firstStandBookDateTime != value)
                {
                    firstStandBookDateTime = value;
                    UpdateChanged("FirstStandBookDateTime");
                    UpdateChanged("FirstStandBookDateTimeStr");
                }
            }
        }

        public string FirstStandBookDateTimeStr
        {
            get
            {
                if (firstStandBookDateTime.Year != 1)
                {
                    return firstStandBookDateTime.ToString("yyyy年MM月dd日");
                }
                return "";
            }
        }

        public void Update()
        {
            this.TaxPayerId = TaxPayer.taxpayer_id;
            this.TaxPayerCode = TaxPayer.taxpayer_code;
            this.TaxPayerName = TaxPayer.taxpayer_name;
            this.TaxPayerTypeId = TaxPayer.taxpayer_type_id;
            TaxPayerGroupId = TaxPayer.taxpayer_group_id;
            TaxPayerRegyear = TaxPayer.taxpayer_regyear;
            TaxPayerProject = TaxPayer.taxpayer_project;
            TaxPayerProjectFinish = TaxPayer.taxpayer_project_finish;
            TaxPayerIsFree = TaxPayer.taxpayer_isfree;
            TaxPayerFtk = TaxPayer.taxpayer_ftk;
        }

        public void DUpdate()
        {
            TaxPayer.taxpayer_id = TaxPayerId;
            TaxPayer.taxpayer_code = TaxPayerCode;
            TaxPayer.taxpayer_name = TaxPayerName;
            TaxPayer.taxpayer_type_id = TaxPayerTypeId;
            TaxPayer.taxpayer_group_id = TaxPayerGroupId;
            TaxPayer.taxpayer_regyear = TaxPayerRegyear;
            TaxPayer.taxpayer_project = TaxPayerProject;
            TaxPayer.taxpayer_project_finish = TaxPayerProjectFinish;
            TaxPayer.taxpayer_isfree = TaxPayerIsFree;
            TaxPayer.taxpayer_ftk = TaxPayerFtk;
        }

        public void RaisALL()
        {
            UpdateChanged("TaxPayerId");
            UpdateChanged("TaxPayerCode");
            UpdateChanged("TaxPayerName");
            UpdateChanged("TaxPayerTypeId");
            UpdateChanged("TaxPayerTypeEntity");
            UpdateChanged("TaxPayerGroupId");
            UpdateChanged("TaxPayerGroup");
            UpdateChanged("TaxPayerRegyear");
            UpdateChanged("TaxPayerProject");
            UpdateChanged("TaxPayerProjectFinish");
            UpdateChanged("TaxPayerIsFree");
            UpdateChanged("TaxPayerFtk");
            UpdateChanged("FirstStandBookDateTime");
            UpdateChanged("FirstStandBookDateTimeStr");
        }

        public string ToString()
        {
            return string.Format("TaxPayerId:{0},TaxPayerCode:{1},TaxPayerName:{2},TaxPayerTypeId:{3},TaxPayerGroup:{4},TaxPayerRegyear:{5},TaxPayerProject:{6},TaxPayerProjectFinish:{7}, {8}, {9}"
                , TaxPayerId, TaxPayerCode, TaxPayerName, TaxPayerTypeId, TaxPayerGroup, TaxPayerRegyear, TaxPayerProject, TaxPayerProjectFinish, TaxPayerIsFree, TaxPayerFtk);
        }

        public DocumentManager.Web.Model.taxpayer TaxPayer { get; set; }
    }
}
