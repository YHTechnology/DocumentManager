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
    public class StandBookEntity : NotifyPropertyChanged
    {
        private string standBookID;
        private string projectName;
        private Nullable<decimal> totalMoney;
        private string taxPayerName;
        private string taxPayerPersonName;
        private string capitalConstruction;
        private Nullable<bool> hasOutVerify;

        private string economicnature;
        private string phonenumber;

        private Nullable<DateTime> payTime;
        private Nullable<decimal> thisPartMoney;

        private Nullable<bool> hasAddValueTax;
        private Nullable<decimal> addValueTaxRate;
        private Nullable<decimal> addValueTax;
        private string addValueTaxItem;


        private Nullable<bool> hasBusinessTax;
        private decimal businessCalculationAmount;
        private Nullable<decimal> businessTaxRate;
        private Nullable<decimal> businessTax;
        private string businessTaxItem;

        private Nullable<bool> hasEducationalSurtax;
        private decimal educationalSurtaxCalculationAmount;
        private Nullable<decimal> educationalSurtaxTaxRate;
        private Nullable<decimal> educationalSurtaxTax;
        private string educationalSurtaxTaxItem;

        private Nullable<bool> hasUrbanTax;
        private decimal urbanCalculationAmount;
        private Nullable<decimal> urbanTaxRate;
        private Nullable<decimal> urbanTax;
        private string urbanTaxItem;

        private Nullable<bool> hasLocalEducationalSurtax;
        private decimal localEducationalSurtaxCalculationAmount;
        private Nullable<decimal> localEducationalSurtaxTaxRate;
        private Nullable<decimal> localEducationalSurtaxTax;
        private string localEducationalSurtaxTaxItem;

        private Nullable<bool> hasStampTax;
        private decimal stampCalculationAmount;
        private Nullable<decimal> stampTaxRate;
        private Nullable<decimal> stampTax;
        private string stampTaxItem;

        private Nullable<bool> hasIncomeTax;
        private decimal incomeCalculationAmount;
        private Nullable<decimal> incomeTaxRate;
        private Nullable<decimal> incomeTax;
        private string incomeTaxItem;

        private string invoiceNumber;
        private string taxReceiptNumber;

        private string note;

        private Nullable<decimal> totalTaxRate;

        private Nullable<int> groupId;

        public string StandBookID
        {
            get { return standBookID; }
            set { if (standBookID != value) { standBookID = value; UpdateChanged("StandBookID"); } }
        }

        public string ProjectName
        {
            get { return projectName; }
            set { if (projectName != value) { projectName = value; UpdateChanged("ProjectName"); } }
        }

        public Nullable<decimal> TotalMoney
        {
            get { return totalMoney; }
            set { if (totalMoney != value) { totalMoney = value; UpdateChanged("TotalMoney"); } }
        }

        public string CapitalConstruction
        {
            get { return capitalConstruction; }
            set { if (capitalConstruction != value) { capitalConstruction = value; UpdateChanged("CapitalConstruction"); } }
        
        }

        public Nullable<bool> HasOutVerify
        {
            get { return hasOutVerify; }
            set { if (hasOutVerify != value) { hasOutVerify = value; UpdateChanged("HasOutVerify"); } }
        }

        public string HasOutVerifyStr
        {
            get
            {
                if (hasOutVerify.HasValue)
                {
                    if (hasOutVerify.Value)
                    {
                        return "有";
                    }
                    else
                    {
                        return "无";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public string EconomicNature
        {
            get { return economicnature; }
            set { if (economicnature != value) { economicnature = value; UpdateChanged("EconomicNature"); } }
        }

        public string PhoneNumber
        {
            get { return phonenumber; }
            set { if (phonenumber != value) { phonenumber = value; UpdateChanged("PhoneNumber"); } }
        }

        public Nullable<decimal> ThisPartMoney
        {
            get { return thisPartMoney; }
            set { if (thisPartMoney != value) { thisPartMoney = value; UpdateChanged("ThisPartMoney"); } }
        }

        public string TaxPayerName
        {
            get { return taxPayerName; }
            set { if (taxPayerName != value) { taxPayerName = value; UpdateChanged("TaxPayerName"); } }
        }

        public string TaxPayerPersonName
        {
            get { return taxPayerPersonName; }
            set { if (taxPayerPersonName != value) { taxPayerPersonName = value; UpdateChanged("TaxPayerPersonName"); } }
        }

        public Nullable<DateTime> PayTime
        {
            get { return payTime; }
            set { if (payTime != value) { payTime = value; UpdateChanged("PayTime"); } }
        }

        public Nullable<bool> HasAddValueTax
        {
            get { return hasAddValueTax; }
            set { if (hasAddValueTax != value) { hasAddValueTax = value; UpdateChanged("HasAddValueTax"); } }
        }

        public Nullable<decimal> AddValueTaxRate
        {
            get { return addValueTaxRate; }
            set { if (addValueTaxRate != value) { addValueTaxRate = value; UpdateChanged("AddValueTaxRate"); } }
        }

        public Nullable<decimal> AddValueTax
        {
            get { return addValueTax; }
            set { if (addValueTax != value) { addValueTax = value; UpdateChanged("AddValueTax"); } }
        }

        public string AddValueTaxItem
        {
            get { return addValueTaxItem; }
            set { if (addValueTaxItem != value) { addValueTaxItem = value; UpdateChanged("AddValueTaxItem"); } }
        }

        public Nullable<bool> HasBusinessTax
        {
            get { return hasBusinessTax; }
            set { if (hasBusinessTax != value) { hasBusinessTax = value; UpdateChanged("HasBusinessTax"); } }
        }

        public decimal BusinessCalculationAmount
        {
            get { return businessCalculationAmount; }
            set { if (businessCalculationAmount != value) { businessCalculationAmount = value; UpdateChanged("BusinessCalculationAmount"); } }
        }

        public Nullable<decimal> BusinessTaxRate
        {
            get { return businessTaxRate; }
            set { if (businessTaxRate != value) { businessTaxRate = value; UpdateChanged("BusinessTaxRate"); } }
        }

        public Nullable<decimal> BusinessTax
        {
            get { return businessTax; }
            set { if (businessTax != value) { businessTax = value; UpdateChanged("BusinessTax"); } }
        }

        public string BusinessTaxItem
        {
            get { return businessTaxItem; }
            set { if (businessTaxItem != value) { businessTaxItem = value; UpdateChanged("BusinessTaxItem"); } }
        }

        public Nullable<bool> HasEducationalSurtax
        {
            get { return hasEducationalSurtax; }
            set { if (hasEducationalSurtax != value) { hasEducationalSurtax = value; UpdateChanged("HasEducationalSurtax"); } }
        }

        public decimal EducationalSurtaxCalculationAmount
        {
            get { return educationalSurtaxCalculationAmount; }
            set { if (educationalSurtaxCalculationAmount != value) { educationalSurtaxCalculationAmount = value; UpdateChanged("EducationalSurtaxCalculationAmount"); } }
        }

        public Nullable<decimal> EducationalSurtaxTaxRate
        {
            get { return educationalSurtaxTaxRate; }
            set { if (educationalSurtaxTaxRate != value) { educationalSurtaxTaxRate = value; UpdateChanged("EducationalSurtaxTaxRate"); } }
        }

        public Nullable<decimal> EducationalSurtaxTax
        {
            get { return educationalSurtaxTax; }
            set { if (educationalSurtaxTax != value) { educationalSurtaxTax = value; UpdateChanged("EducationalSurtaxTax"); } }
        }

        public string EducationalSurtaxTaxItem
        {
            get { return educationalSurtaxTaxItem; }
            set { if (educationalSurtaxTaxItem != value) { educationalSurtaxTaxItem = value; UpdateChanged("EducationalSurtaxTaxItem"); } }
        }

        public Nullable<bool> HasUrbanTax
        {
            get { return hasUrbanTax; }
            set { if (hasUrbanTax != value) { hasUrbanTax = value; UpdateChanged("HasUrbanTax"); } }
        }

        public decimal UrbanCalculationAmount
        {
            get { return urbanCalculationAmount; }
            set { if (urbanCalculationAmount != value) { urbanCalculationAmount = value; UpdateChanged("UrbanCalculationAmount"); } }
        }

        public Nullable<decimal> UrbanTaxRate
        {
            get { return urbanTaxRate; }
            set { if (urbanTaxRate != value) { urbanTaxRate = value; UpdateChanged("UrbanTaxRate"); } }
        }

        public Nullable<decimal> UrbanTax
        {
            get { return urbanTax; }
            set { if (urbanTax != value) { urbanTax = value; UpdateChanged("UrbanTax"); } }
        }

        public string UrbanTaxItem
        {
            get { return urbanTaxItem; }
            set { if (urbanTaxItem != value) { urbanTaxItem = value; UpdateChanged("UrbanTaxItem"); } }
        }

        public Nullable<bool> HasLocalEducationalSurtax
        {
            get { return hasLocalEducationalSurtax; }
            set { if (hasLocalEducationalSurtax != value) { hasLocalEducationalSurtax = value; UpdateChanged("HasLocalEducationalSurtax"); } }
        }

        public decimal LocalEducationalSurtaxCalculationAmount
        {
            get { return localEducationalSurtaxCalculationAmount; }
            set { if (localEducationalSurtaxCalculationAmount != value) { localEducationalSurtaxCalculationAmount = value; UpdateChanged("LocalEducationalSurtaxCalculationAmount"); } }
        }

        public Nullable<decimal> LocalEducationalSurtaxTaxRate
        {
            get { return localEducationalSurtaxTaxRate; }
            set { if (localEducationalSurtaxTaxRate != value) { localEducationalSurtaxTaxRate = value; UpdateChanged("LocalEducationalSurtaxTaxRate"); } }
        }

        public Nullable<decimal> LocalEducationalSurtaxTax
        {
            get { return localEducationalSurtaxTax; }
            set { if (localEducationalSurtaxTax != value) { localEducationalSurtaxTax = value; UpdateChanged("LocalEducationalSurtaxTax"); } }
        }

        public string LocalEducationalSurtaxTaxItem
        {
            get { return localEducationalSurtaxTaxItem; }
            set { if (localEducationalSurtaxTaxItem != value) { localEducationalSurtaxTaxItem = value; UpdateChanged("LocalEducationalSurtaxTaxItem"); } }
        }

        public Nullable<bool> HasStampTax
        {
            get { return hasStampTax; }
            set { if (hasStampTax != value) { hasStampTax = value; UpdateChanged("HasStampTax"); } }
        }

        public decimal StampCalculationAmount
        {
            get { return stampCalculationAmount; }
            set { if (stampCalculationAmount != value) { stampCalculationAmount = value; UpdateChanged("StampCalculationAmount"); } }
        }

        public Nullable<decimal> StampTaxRate
        {
            get { return stampTaxRate; }
            set { if (stampTaxRate != value) { stampTaxRate = value; UpdateChanged("StampTaxRate"); } }
        }

        public Nullable<decimal> StampTax
        {
            get { return stampTax; }
            set { if (stampTax != value) { stampTax = value; UpdateChanged("StampTax"); } }
        }

        public string StampTaxItem
        {
            get { return stampTaxItem; }
            set { if (stampTaxItem != value) { stampTaxItem = value; UpdateChanged("StampTaxItem"); } }
        }

        public Nullable<bool> HasIncomeTax
        {
            get { return hasIncomeTax; }
            set { if (hasIncomeTax != value) { hasIncomeTax = value; UpdateChanged("HasIncomeTax"); } }
        }

        public decimal IncomeCalculationAmount
        {
            get { return incomeCalculationAmount; }
            set { if (incomeCalculationAmount != value) { incomeCalculationAmount = value; UpdateChanged("IncomeCalculationAmount"); } }
        }

        public Nullable<decimal> IncomeTaxRate
        {
            get { return incomeTaxRate; }
            set { if (incomeTaxRate != value) { incomeTaxRate = value; UpdateChanged("IncomeTaxRate"); } }
        }

        public Nullable<decimal> IncomeTax
        {
            get { return incomeTax; }
            set { if (incomeTax != value) { incomeTax = value; UpdateChanged("IncomeTax"); } }
        }

        public string IncomeTaxItem
        {
            get { return incomeTaxItem; }
            set { if (incomeTaxItem != value) { incomeTaxItem = value; UpdateChanged("IncomeTaxItem"); } }
        }

        public decimal TotalTax
        {
            get
            {
                decimal lTotalTax = 0;
                lTotalTax += addValueTax.GetValueOrDefault(0);
                lTotalTax += businessTax.GetValueOrDefault(0);
                lTotalTax += educationalSurtaxTax.GetValueOrDefault(0);
                lTotalTax += urbanTax.GetValueOrDefault(0);
                lTotalTax += localEducationalSurtaxTax.GetValueOrDefault(0);
                lTotalTax += stampTax.GetValueOrDefault(0);
                lTotalTax += incomeTax.GetValueOrDefault(0);

                return lTotalTax;

            }
        }

        public string InvoiceNumber
        {
            get { return invoiceNumber; }
            set { if (invoiceNumber != value) { invoiceNumber = value; UpdateChanged("IvoiceNumber"); } }
        }

        public string TaxReceiptNumber
        {
            get { return taxReceiptNumber; }
            set { if (taxReceiptNumber != value) { taxReceiptNumber = value; UpdateChanged("TaxReceiptNumber"); } }
        }

        public Nullable<int> GroupID
        {
            get { return groupId; }
            set { if (groupId != value) { groupId = value; UpdateChanged("GroupID"); } }
        }

        public string Note
        {
            get { return note; }
            set { if (note != value) { note = value; UpdateChanged("Note"); } }
        }

        public Nullable<decimal> TotalTaxRate
        {
            get { return totalTaxRate; }
            set { if (totalTaxRate != value) { totalTaxRate = value; UpdateChanged("TotalTaxRate"); } }
        }

        public string PartPriceStrTYear
        {
            get
            {
                if (payTime.HasValue)
                {
                    DateTime lCurrentTime = DateTime.Now;
                    int lCurrentYear = lCurrentTime.Year;
                    int lPayYear = payTime.Value.Year;
                    if (lCurrentYear == lPayYear)
                    {
                        if(thisPartMoney.HasValue)
                        {
                            return thisPartMoney.Value.ToString("#0,##0.00");
                        }
                    }
                }
                return "";
            }
        }

        public string PartPriceStrPYear
        {
            get
            {
                if (payTime.HasValue)
                {
                    DateTime lCurrentTime = DateTime.Now;
                    int lCurrentYear = lCurrentTime.Year;
                    int lPayYear = payTime.Value.Year;
                    if (lCurrentYear != lPayYear)
                    {
                        if (thisPartMoney.HasValue)
                        {
                            return thisPartMoney.Value.ToString("#0,##0.00");
                        }
                    }
                }
                return "";
            }
        }

        public string TotalTaxRateStr
        {
            get
            {
                string lTotalRateStr = (totalTaxRate.GetValueOrDefault(0) * 100).ToString("0.0000");
                return lTotalRateStr + "%";
            }
        }

        public decimal TotalTaxTotal
        {
            get
            {
                decimal lTotalTaxTotal = 0;
                if(totalTaxRate.HasValue && totalMoney.HasValue)
                {
                    lTotalTaxTotal = totalTaxRate.Value * totalMoney.Value;
                }
                
                return lTotalTaxTotal;
            }
        }

        public bool IsVaild { get; set; }

        public Visibility ShowSuccess { get; set; }

        public Visibility ShowError { get; set; }

        private string errorText;
        public string ErrorText
        {
            get { return errorText; }
            set { if (errorText != value) { errorText = value; UpdateChanged("ErrorText"); } }
        }

        private string successText;
        public string SuccessText
        {
            get { return successText; }
            set { if (successText != value) { successText = value; UpdateChanged("SuccessText"); } }
        }

        public void Update()
        {
            standBookID = StandBook.standbook_id;
            projectName = StandBook.projectname;
            totalMoney = StandBook.totalmoney;
            taxPayerName = StandBook.taxpayername;
            taxPayerPersonName = StandBook.taxpayerpersonname;
            capitalConstruction = StandBook.capitalcontruction;
            hasOutVerify = StandBook.hasoutverify;
            economicnature = StandBook.economicnature;
            phonenumber = StandBook.phonenumber;
            payTime = StandBook.paytime;
            thisPartMoney = StandBook.thispartmoney;

            hasAddValueTax = StandBook.hasaddedvaluetax;
            addValueTax = StandBook.addedvaluetax;
            addValueTaxRate = StandBook.addedvaluetaxrate;
            addValueTaxItem = StandBook.addedvaluetaxitem;

            hasBusinessTax = StandBook.hasbusinesstax;
            businessTax = StandBook.businesstax;
            businessTaxRate = StandBook.businestaxrate;
            businessTaxItem = StandBook.businesstaxitem;

            hasEducationalSurtax = StandBook.haseducationsurtax;
            educationalSurtaxTax = StandBook.educationsurtax;
            educationalSurtaxTaxRate = StandBook.educationsurtaxrate;
            educationalSurtaxTaxItem = StandBook.educationsurtaxitem;

            hasUrbanTax = StandBook.hasurbantax;
            urbanTax = StandBook.urbantax;
            urbanTaxRate = StandBook.urbantaxrate;
            urbanTaxItem = StandBook.urbantaxitem;

            hasLocalEducationalSurtax = StandBook.haslocaleducationsurtax;
            localEducationalSurtaxTax = StandBook.localeducationsurtax;
            localEducationalSurtaxTaxRate = StandBook.localeducationsurtaxrate;
            localEducationalSurtaxTaxItem = StandBook.localeducationsurtaxitem;

            hasStampTax = StandBook.hasstamptax;
            stampTax = StandBook.stamptax;
            stampTaxRate = StandBook.stamptaxrate;
            stampTaxItem = StandBook.stamptaxitem;

            hasIncomeTax = StandBook.hasincometax;
            incomeTax = StandBook.incometax;
            incomeTaxRate = StandBook.incometaxrate;
            incomeTaxItem = StandBook.incometaxitem;

            invoiceNumber = StandBook.invoicenumber;
            taxReceiptNumber = StandBook.taxreceiptnumber;
            groupId = StandBook.groupid;
            note = StandBook.note;

            totalTaxRate = StandBook.totaltaxrate;
        }

        public void DUpdate()
        {
            StandBook.standbook_id = standBookID;
            StandBook.projectname = projectName;
            StandBook.totalmoney = totalMoney;
            StandBook.taxpayername = taxPayerName;
            StandBook.taxpayerpersonname = taxPayerPersonName;
            StandBook.capitalcontruction = capitalConstruction;
            StandBook.hasoutverify = hasOutVerify;
            StandBook.economicnature = economicnature;
            StandBook.phonenumber = phonenumber;
            StandBook.paytime = payTime;
            StandBook.thispartmoney = thisPartMoney;

            StandBook.hasaddedvaluetax = hasAddValueTax;
            StandBook.addedvaluetax = addValueTax;
            StandBook.addedvaluetaxrate = addValueTaxRate;
            StandBook.addedvaluetaxitem = addValueTaxItem;

            StandBook.hasbusinesstax = hasBusinessTax;
            StandBook.businesstax = businessTax;
            StandBook.businestaxrate = businessTaxRate;
            StandBook.businesstaxitem = businessTaxItem;
            
            StandBook.haseducationsurtax = hasEducationalSurtax;
            StandBook.educationsurtax = educationalSurtaxTax;
            StandBook.educationsurtaxrate = educationalSurtaxTaxRate;
            StandBook.educationsurtaxitem = educationalSurtaxTaxItem;

            StandBook.hasurbantax = hasUrbanTax;
            StandBook.urbantax = urbanTax;
            StandBook.urbantaxrate = urbanTaxRate;
            StandBook.urbantaxitem = urbanTaxItem;

            StandBook.haslocaleducationsurtax = hasLocalEducationalSurtax;
            StandBook.localeducationsurtax = localEducationalSurtaxTax;
            StandBook.localeducationsurtaxrate = localEducationalSurtaxTaxRate;
            StandBook.localeducationsurtaxitem = localEducationalSurtaxTaxItem;

            StandBook.hasstamptax = hasStampTax;
            StandBook.stamptax = stampTax;
            StandBook.stamptaxrate = stampTaxRate;
            StandBook.stamptaxitem = stampTaxItem;

            StandBook.hasincometax = hasIncomeTax;
            StandBook.incometax = incomeTax;
            StandBook.incometaxrate = incomeTaxRate;
            StandBook.incometaxitem = incomeTaxItem;
            
            StandBook.invoicenumber = invoiceNumber;
            StandBook.taxreceiptnumber = taxReceiptNumber;
            StandBook.groupid = groupId;
            StandBook.note = note;

            StandBook.totaltaxrate = totalTaxRate;
        }

        public void RaisALL()
        {
            UpdateChanged("StandBookID");
            UpdateChanged("ProjectName");
            UpdateChanged("TotalMoney");
            UpdateChanged("CapitalConstruction");
            UpdateChanged("HasOutVerify");
            UpdateChanged("EconomicNature");
            UpdateChanged("PhoneNumber");
            UpdateChanged("ThisPartMoney");
            UpdateChanged("TaxPayerName");
            UpdateChanged("TaxPayerPersonName");
            UpdateChanged("PayTime");
            UpdateChanged("HasAddValueTax");
            UpdateChanged("AddValueTax");
            UpdateChanged("AddValueTaxRate");
            UpdateChanged("AddValueTaxItem");
            UpdateChanged("HasBusinessTax");
            UpdateChanged("BusinessCalculationAmount");
            UpdateChanged("BusinessTaxRate");
            UpdateChanged("BusinessTax");
            UpdateChanged("BusinessTaxItem");
            UpdateChanged("HasEducationalSurtax");
            UpdateChanged("EducationalSurtaxCalculationAmount");
            UpdateChanged("EducationalSurtaxTaxRate");
            UpdateChanged("EducationalSurtaxTax");
            UpdateChanged("EducationalSurtaxTaxItem");
            UpdateChanged("HasUrbanTax");
            UpdateChanged("UrbanCalculationAmount");
            UpdateChanged("UrbanTaxRate");
            UpdateChanged("UrbanTax");
            UpdateChanged("UrbanTaxItem");
            UpdateChanged("HasLocalEducationalSurtax");
            UpdateChanged("LocalEducationalSurtaxCalculationAmount");
            UpdateChanged("LocalEducationalSurtaxTaxRate");
            UpdateChanged("LocalEducationalSurtaxTax");
            UpdateChanged("LocalEducationalSurtaxTaxItem");
            UpdateChanged("HasStampTax");
            UpdateChanged("StampCalculationAmount");
            UpdateChanged("StampTaxRate");
            UpdateChanged("StampTax");
            UpdateChanged("StampTaxItem");
            UpdateChanged("HasIncomeTax");
            UpdateChanged("IncomeCalculationAmount");
            UpdateChanged("IncomeTaxRate");
            UpdateChanged("IncomeTax");
            UpdateChanged("IncomeTaxItem");
            UpdateChanged("TotalTax");
            UpdateChanged("IvoiceNumber");
            UpdateChanged("TaxReceiptNumber");
            UpdateChanged("GroupID");
            UpdateChanged("Note");
            UpdateChanged("TotalTaxRate");
        }

        public DocumentManager.Web.Model.standbook StandBook { get; set; }

        public string ToString()
        {
            return string.Format("StandBookID:{0},ProjectName:{1},TotalMoney:{2},CapitalConstruction:{3},HasOutVerify:{4},ThisPartMoney:{5},TaxPayerName:{6},TaxPayerPersonName:{7}"
                , StandBookID, ProjectName, TotalMoney, CapitalConstruction, HasOutVerifyStr, ThisPartMoney, TaxPayerName, TaxPayerPersonName);
        }

    }
}
