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
using System.Linq;
using System.Collections.ObjectModel;
using DocumentManager.Model.Entities;

namespace DocumentManager.Model.SearchEntities
{
    public class TaxPayerSearchEntity : NotifyPropertyChanged
    {
        public int GroupID { get; set; }

        private SearchCondition searchCondition;
        public SearchCondition TPSearchCondition
        {
            get { return searchCondition; }
            set { if (searchCondition != value) { searchCondition = value; UpdateChanged("TPSearchCondition"); UpdateChanged("TPSearchConditionStr"); } }
        }

        public string TPSearchConditionStr
        {
            get
            {
                return SearchConditionString.GetString(searchCondition);
            }
        }

        private SearchOperator searchOperator;
        public SearchOperator TPSearchOperator
        {
            get { return searchOperator; }
            set { if (searchOperator != value) { searchOperator = value; UpdateChanged("TPSearchOperator"); UpdateChanged("TPSearchOperatorStr"); } }
        }

        public string TPSearchOperatorStr
        {
            get
            {
                return SearchOperatorString.GetString(searchOperator);
            }
        }

        private TaxPayerField taxPayerField;
        public TaxPayerField TPTaxPayerField
        {
            get
            {
                return taxPayerField;
            }
            set
            {
                if (taxPayerField != value)
                {
                    taxPayerField = value;

                    switch (taxPayerField)
                    {
                        case TaxPayerField.CODE:
                        case TaxPayerField.NAME:
                        case TaxPayerField.PROJECT:
                            TPValueType = ValueType.VALUESTR;
                            break;
                        case TaxPayerField.FINISH:
                        case TaxPayerField.FREE:
                            TPValueType = ValueType.VALUEBOOLEAN;
                            break;
                        case TaxPayerField.TYPE:
                            TPValueType = ValueType.VALUEINT;
                            break;
                    }
                    UpdateChanged("TPTaxPayerField");
                }
            }
        }

        public string TPTaxPayerFieldStr
        {
            get
            {
                return TaxPayerFieldString.GetString(taxPayerField, GroupID);
            }
        }

        private ValueType valueType;
        public ValueType TPValueType
        {
            get { return valueType; }
            set { if (valueType != value) { valueType = value; UpdateChanged("TPValueType"); } }
        }

        public string ValueStr { get; set; }
        public int ValueInt { get; set; }
        public bool ValueBoolean { get; set; }

        public ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }

        public string Value
        {
            get
            {
                string lRet = "";
                switch (valueType)
                {
                    case ValueType.VALUESTR:
                        lRet = ValueStr;
                        break;
                    case ValueType.VALUEINT:
                        if (TPTaxPayerField == TaxPayerField.TYPE)
                        {
                            foreach (TaxPayerTypeEntity lTaxPayerTypeEntity in TaxPayerTypeList)
                            {
                                if (lTaxPayerTypeEntity.TaxPayerTypeId == ValueInt)
                                {
                                    return lTaxPayerTypeEntity.TaxPayerTypeName;
                                }
                            }
                        }
                        else
                        {
                            lRet = ValueInt.ToString();
                        }
                        break;
                    case ValueType.VALUEBOOLEAN:
                        lRet = ValueBoolean.ToString();
                        break;
                }
                return lRet;
            }
        }

    }
}
