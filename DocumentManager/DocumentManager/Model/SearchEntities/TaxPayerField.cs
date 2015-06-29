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

namespace DocumentManager.Model.SearchEntities
{
    public enum TaxPayerField : uint
    {
        CODE = 0,
        NAME = 1,
        TYPE = 2,
        PROJECT = 3,
        YEAR = 4,
        FINISH = 5,
        FREE = 6
    }

    public static class TaxPayerFieldString
    {
        public static string GetString(TaxPayerField aTaxPayerField, int aGroupID)
        {
            string lRet = "";
            switch (aTaxPayerField)
            {
                case TaxPayerField.CODE:
                    lRet = "编号";
                    break;
                case TaxPayerField.NAME:
                    lRet = "名称";
                    break;
                case TaxPayerField.TYPE:
                    lRet = "类型";
                    break;
                case TaxPayerField.PROJECT:
                    if (aGroupID == 0)
                    {
                        lRet = "税种";
                    }
                    else
                    {
                        lRet = "项目";
                    }
                    break;
                case TaxPayerField.YEAR:
                    lRet = "年份";
                    break;
                case TaxPayerField.FINISH:
                    lRet = "完成";
                    break;
                case TaxPayerField.FREE:
                    lRet = "免税";
                    break;
            }
            return lRet;
        }
    }


    public static class TaxPayerFieldFieldString
    {
        public static string GetString(TaxPayerField aTaxPayerField)
        {
            string lRet = "";
            switch (aTaxPayerField)
            {
                case TaxPayerField.CODE:
                    lRet = "taxpayer_code";
                    break;
                case TaxPayerField.NAME:
                    lRet = "taxpayer_name";
                    break;
                case TaxPayerField.TYPE:
                    lRet = "taxpayer_type_id";
                    break;
                case TaxPayerField.PROJECT:
                    lRet = "taxpayer_project";
                    break;
                case TaxPayerField.YEAR:
                    lRet = "taxpayer_regyear";
                    break;
                case TaxPayerField.FINISH:
                    lRet = "taxpayer_project_finish";
                    break;
                case TaxPayerField.FREE:
                    lRet = "taxpayer_isfree";
                    break;
            }
            return lRet;
        }
    }
}
