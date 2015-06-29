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
using DocumentManager.Model.SearchEntities;
using System.Collections.ObjectModel;
using DocumentManager.Controls;
using DocumentManager.Model.Entities;

namespace DocumentManager.ViewModels
{
    public class SearchConditionStruct
    {
        public string Name { get; set; }
        public SearchCondition Value { get; set; }
    }

    public class SearchOperatorStruct
    {
        public string Name { get; set; }
        public SearchOperator Value { get; set; }
    }

    public class TaxPayerFieldStruct
    {
        public string Name { get; set; }
        public TaxPayerField Value { get; set; }
    }

    public class AddSearchWindowViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<SearchConditionStruct> SearchConditionList { get; set; }
        public ObservableCollection<SearchOperatorStruct> SearchOperatorList { get; set; }
        public ObservableCollection<TaxPayerFieldStruct> TaxPayerFieldList { get; set; }

        public ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }

        private TaxPayerTypeEntity selectTaxPayerType;
        public TaxPayerTypeEntity SelectTaxPayerType
        {
            get
            {
                return selectTaxPayerType;
            }
            set
            {
                if (selectTaxPayerType != value)
                {
                    selectTaxPayerType = value;
                    ValueInt = selectTaxPayerType.TaxPayerTypeId;
                    UpdateChanged("SelectTaxPayerType");
                }
            }
        }

        public int GroupID { get; set; }
        public bool IsFirst { get; set; }

        private ObservableCollection<SearchOperatorStruct> SearchOperatorStructIntList { get; set; }
        private ObservableCollection<SearchOperatorStruct> SearchOperatorStructStrList { get; set; }
        private ObservableCollection<SearchOperatorStruct> SearchOperatorStructBoolList { get; set; }

        private SearchConditionStruct selectSearchCondition;
        public SearchConditionStruct SelectSearchCondition
        {
            get
            {
                return selectSearchCondition;
            }
            set
            {
                if (selectSearchCondition != value)
                {
                    selectSearchCondition = value;
                    UpdateChanged("SelectSearchCondition");
                }
            }
        }

        private SearchOperatorStruct selectSearchOperator;
        public SearchOperatorStruct SelectSearchOperator
        {
            get
            {
                return selectSearchOperator;
            }
            set
            {
                if (selectSearchOperator != value)
                {
                    selectSearchOperator = value;
                    UpdateChanged("SelectSearchOperator");
                }

            }
        }

        private TaxPayerFieldStruct selectTaxPayerField;
        public TaxPayerFieldStruct SelectTaxPayerField
        {
            get
            {
                return selectTaxPayerField;
            }
            set
            {
                if (selectTaxPayerField != value)
                {
                    selectTaxPayerField = value;

                    switch (selectTaxPayerField.Value)
                    {
                        case TaxPayerField.CODE:
                            VisibilityStrChange();
                            break;
                        case TaxPayerField.NAME:
                            VisibilityStrChange();
                            break;
                        case TaxPayerField.TYPE:
                            VisibilityTypeChange();
                            break;
                        case TaxPayerField.YEAR:
                            VisibilityStrChange();
                            break;
                        case TaxPayerField.FINISH:
                            VisibilityBoolChange();
                            break;
                        case TaxPayerField.PROJECT:
                            VisibilityStrChange();
                            break;
                        case TaxPayerField.FREE:
                            VisibilityBoolChange();
                            break;
                    }
                    UpdateChanged("SelectTaxPayerField");
                }
            }
        }

        private ChildWindow childWindow;

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public string ValueStr { get; set; }
        public int ValueInt { get; set; }
        public bool ValueBoolean { get; set; }

        private Visibility visibilityStr;
        public Visibility VisibilityStr
        {
            get { return visibilityStr; }
            set { if (visibilityStr != value) { visibilityStr = value; UpdateChanged("VisibilityStr");} }
        }

        private Visibility visibilityBool;
        public Visibility VisibilityBool
        {
            get { return visibilityBool; }
            set { if (visibilityBool != value) { visibilityBool = value; UpdateChanged("VisibilityBool"); } }
        }

        private Visibility visibilityType;
        public Visibility VisibilityType
        {
            get { return visibilityType; }
            set { if (visibilityType != value) { visibilityType = value; UpdateChanged("VisibilityType"); } }
        }

        public AddSearchWindowViewModel(ChildWindow aChildWindow, int aGroupID, bool aIsFirst, ObservableCollection<TaxPayerTypeEntity> aTaxPayerTypeList)
        {
            childWindow = aChildWindow;
            ValueInt = -1;
            TaxPayerTypeList = aTaxPayerTypeList;

            SearchConditionList = new ObservableCollection<SearchConditionStruct>();
            SearchOperatorList = new ObservableCollection<SearchOperatorStruct>();
            TaxPayerFieldList = new ObservableCollection<TaxPayerFieldStruct>();

            SearchOperatorStructIntList = new ObservableCollection<SearchOperatorStruct>();
            SearchOperatorStructStrList = new ObservableCollection<SearchOperatorStruct>();
            SearchOperatorStructBoolList = new ObservableCollection<SearchOperatorStruct>();

            GroupID = aGroupID;
            IsFirst = aIsFirst;
            
            InitCondition();

            InitOperatorIntList();
            InitOperatorStrList();
            InitOperatorBoolList();

            switch (aGroupID)
            {
                case 0: // 固定户
                    InitGD();
                    break;
                case 1: // 建安代开
                    InitJA();
                    break;
                case 2: // 普票代开
                    InitPP();
                    break;
                case 3: // 专票代开
                    InitZP();
                    break;
            }
            OnOK = new DelegateCommand(onOK);
            OnCancel = new DelegateCommand(onCancel);
            VisibilityStrChange();
        }

        private void VisibilityStrChange()
        {
            VisibilityStr = Visibility.Visible;
            VisibilityBool = Visibility.Collapsed;
            VisibilityType = Visibility.Collapsed;
            SearchOperatorList.Clear();
            foreach(SearchOperatorStruct lItem in SearchOperatorStructStrList)
            {
                SearchOperatorList.Add(lItem);
            }
            //SearchOperatorList.( SearchOperatorStructStrList);
            UpdateChanged("SearchOperatorList");
        }

        private void VisibilityBoolChange()
        {
            VisibilityStr = Visibility.Collapsed;
            VisibilityBool = Visibility.Visible;
            VisibilityType = Visibility.Collapsed;
            SearchOperatorList.Clear();
            foreach (SearchOperatorStruct lItem in SearchOperatorStructBoolList)
            {
                SearchOperatorList.Add(lItem);
            }
            //SearchOperatorList = SearchOperatorStructBoolList;
            UpdateChanged("SearchOperatorList");
        }

        private void VisibilityTypeChange()
        {
            VisibilityStr = Visibility.Collapsed;
            VisibilityBool = Visibility.Collapsed;
            VisibilityType = Visibility.Visible;
            SearchOperatorList.Clear();
            foreach (SearchOperatorStruct lItem in SearchOperatorStructBoolList)
            {
                SearchOperatorList.Add(lItem);
            }
            //SearchOperatorList = SearchOperatorStructBoolList;
            UpdateChanged("SearchOperatorList");
        }

        private void onOK()
        {
            if (SelectSearchCondition == null && IsFirst == false)
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请选择条件！");
                notifyWindow.Show();
                return;
            }

            if (SelectTaxPayerField == null)
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请选择字段！");
                notifyWindow.Show();
                return;
            }

            if (SelectSearchOperator == null)
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请选择操作！");
                notifyWindow.Show();
                return;
            }

            if (ValueStr == null && VisibilityStr == Visibility.Visible)
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请设置值！");
                notifyWindow.Show();
                return;
            }

            if (ValueInt == -1 && visibilityType == Visibility.Visible)
            {
                NotifyWindow notifyWindow = new NotifyWindow("错误", "请设置值！");
                notifyWindow.Show();
                return;
            }

            childWindow.DialogResult = true;
        }

        private void onCancel()
        {
            childWindow.DialogResult = false;
        }

        private void InitGD()
        {
            TaxPayerFieldStruct lTaxPayerFieldName = new TaxPayerFieldStruct();
            lTaxPayerFieldName.Value = TaxPayerField.NAME;
            lTaxPayerFieldName.Name = TaxPayerFieldString.GetString(TaxPayerField.NAME, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldName);

            TaxPayerFieldStruct lTaxPayerFieldType = new TaxPayerFieldStruct();
            lTaxPayerFieldType.Value = TaxPayerField.TYPE;
            lTaxPayerFieldType.Name = TaxPayerFieldString.GetString(TaxPayerField.TYPE, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldType);

            TaxPayerFieldStruct lTaxPayerFieldProject = new TaxPayerFieldStruct();
            lTaxPayerFieldProject.Value = TaxPayerField.PROJECT;
            lTaxPayerFieldProject.Name = TaxPayerFieldString.GetString(TaxPayerField.PROJECT, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldProject);

            TaxPayerFieldStruct lTaxPayerFieldStructFree = new TaxPayerFieldStruct();
            lTaxPayerFieldStructFree.Value = TaxPayerField.FREE;
            lTaxPayerFieldStructFree.Name = TaxPayerFieldString.GetString(TaxPayerField.FREE, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructFree);

        }

        private void InitJA()
        {
            TaxPayerFieldStruct lTaxPayerFieldName = new TaxPayerFieldStruct();
            lTaxPayerFieldName.Value = TaxPayerField.NAME;
            lTaxPayerFieldName.Name = TaxPayerFieldString.GetString(TaxPayerField.NAME, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldName);

            TaxPayerFieldStruct lTaxPayerFieldType = new TaxPayerFieldStruct();
            lTaxPayerFieldType.Value = TaxPayerField.TYPE;
            lTaxPayerFieldType.Name = TaxPayerFieldString.GetString(TaxPayerField.TYPE, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldType);

            TaxPayerFieldStruct lTaxPayerFieldProject = new TaxPayerFieldStruct();
            lTaxPayerFieldProject.Value = TaxPayerField.PROJECT;
            lTaxPayerFieldProject.Name = TaxPayerFieldString.GetString(TaxPayerField.PROJECT, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldProject);

            TaxPayerFieldStruct lTaxPayerFieldStructYear = new TaxPayerFieldStruct();
            lTaxPayerFieldStructYear.Value = TaxPayerField.YEAR;
            lTaxPayerFieldStructYear.Name = TaxPayerFieldString.GetString(TaxPayerField.YEAR, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructYear);

            TaxPayerFieldStruct lTaxPayerFieldStructFinish = new TaxPayerFieldStruct();
            lTaxPayerFieldStructFinish.Value = TaxPayerField.FINISH;
            lTaxPayerFieldStructFinish.Name = TaxPayerFieldString.GetString(TaxPayerField.FINISH, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructFinish);
        }

        private void InitPP()
        {
            TaxPayerFieldStruct lTaxPayerFieldName = new TaxPayerFieldStruct();
            lTaxPayerFieldName.Value = TaxPayerField.NAME;
            lTaxPayerFieldName.Name = TaxPayerFieldString.GetString(TaxPayerField.NAME, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldName);

            TaxPayerFieldStruct lTaxPayerFieldType = new TaxPayerFieldStruct();
            lTaxPayerFieldType.Value = TaxPayerField.TYPE;
            lTaxPayerFieldType.Name = TaxPayerFieldString.GetString(TaxPayerField.TYPE, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldType);

            TaxPayerFieldStruct lTaxPayerFieldProject = new TaxPayerFieldStruct();
            lTaxPayerFieldProject.Value = TaxPayerField.PROJECT;
            lTaxPayerFieldProject.Name = TaxPayerFieldString.GetString(TaxPayerField.PROJECT, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldProject);

            TaxPayerFieldStruct lTaxPayerFieldStructYear = new TaxPayerFieldStruct();
            lTaxPayerFieldStructYear.Value = TaxPayerField.YEAR;
            lTaxPayerFieldStructYear.Name = TaxPayerFieldString.GetString(TaxPayerField.YEAR, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructYear);

            TaxPayerFieldStruct lTaxPayerFieldStructFinish = new TaxPayerFieldStruct();
            lTaxPayerFieldStructFinish.Value = TaxPayerField.FINISH;
            lTaxPayerFieldStructFinish.Name = TaxPayerFieldString.GetString(TaxPayerField.FINISH, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructFinish);
        }

        private void InitZP()
        {
            TaxPayerFieldStruct lTaxPayerFieldName = new TaxPayerFieldStruct();
            lTaxPayerFieldName.Value = TaxPayerField.NAME;
            lTaxPayerFieldName.Name = TaxPayerFieldString.GetString(TaxPayerField.NAME, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldName);

            TaxPayerFieldStruct lTaxPayerFieldType = new TaxPayerFieldStruct();
            lTaxPayerFieldType.Value = TaxPayerField.TYPE;
            lTaxPayerFieldType.Name = TaxPayerFieldString.GetString(TaxPayerField.TYPE, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldType);

            TaxPayerFieldStruct lTaxPayerFieldProject = new TaxPayerFieldStruct();
            lTaxPayerFieldProject.Value = TaxPayerField.PROJECT;
            lTaxPayerFieldProject.Name = TaxPayerFieldString.GetString(TaxPayerField.PROJECT, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldProject);

            TaxPayerFieldStruct lTaxPayerFieldStructYear = new TaxPayerFieldStruct();
            lTaxPayerFieldStructYear.Value = TaxPayerField.YEAR;
            lTaxPayerFieldStructYear.Name = TaxPayerFieldString.GetString(TaxPayerField.YEAR, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructYear);

            TaxPayerFieldStruct lTaxPayerFieldStructFinish = new TaxPayerFieldStruct();
            lTaxPayerFieldStructFinish.Value = TaxPayerField.FINISH;
            lTaxPayerFieldStructFinish.Name = TaxPayerFieldString.GetString(TaxPayerField.FINISH, GroupID);
            TaxPayerFieldList.Add(lTaxPayerFieldStructFinish);
        }

        private void InitCondition()
        {
            if(!IsFirst)
            {
                SearchConditionStruct lSearchConditionAnd = new SearchConditionStruct();
                lSearchConditionAnd.Value = SearchCondition.AND;
                lSearchConditionAnd.Name = SearchConditionString.GetString(SearchCondition.AND);
                SearchConditionList.Add(lSearchConditionAnd);

                SearchConditionStruct lSearchConditionOr = new SearchConditionStruct();
                lSearchConditionOr.Value = SearchCondition.OR;
                lSearchConditionOr.Name = SearchConditionString.GetString(SearchCondition.OR);
                SearchConditionList.Add(lSearchConditionOr);
            }
        }

        void InitOperatorIntList()
        {
            SearchOperatorStruct lSearchOperatorStructEqual = new SearchOperatorStruct();
            lSearchOperatorStructEqual.Value = SearchOperator.EQUAL;
            lSearchOperatorStructEqual.Name = SearchOperatorString.GetString(SearchOperator.EQUAL);
            SearchOperatorStructIntList.Add(lSearchOperatorStructEqual);

            SearchOperatorStruct lSearchOperatorStructNoEqual = new SearchOperatorStruct();
            lSearchOperatorStructNoEqual.Value = SearchOperator.NOEQUAL;
            lSearchOperatorStructNoEqual.Name = SearchOperatorString.GetString(SearchOperator.NOEQUAL);
            SearchOperatorStructIntList.Add(lSearchOperatorStructNoEqual);

            SearchOperatorStruct lSearchOperatorStructGreater = new SearchOperatorStruct();
            lSearchOperatorStructGreater.Value = SearchOperator.GREATER;
            lSearchOperatorStructGreater.Name = SearchOperatorString.GetString(SearchOperator.GREATER);
            SearchOperatorStructIntList.Add(lSearchOperatorStructGreater);

            SearchOperatorStruct lSearchOperatorStructLess = new SearchOperatorStruct();
            lSearchOperatorStructLess.Value = SearchOperator.LESS;
            lSearchOperatorStructLess.Name = SearchOperatorString.GetString(SearchOperator.LESS);
            SearchOperatorStructIntList.Add(lSearchOperatorStructLess);

            SearchOperatorStruct lSearchOperatorStructGreaterEqual = new SearchOperatorStruct();
            lSearchOperatorStructGreaterEqual.Value = SearchOperator.GREATEREQUAL;
            lSearchOperatorStructGreaterEqual.Name = SearchOperatorString.GetString(SearchOperator.GREATEREQUAL);
            SearchOperatorStructIntList.Add(lSearchOperatorStructGreaterEqual);

            SearchOperatorStruct lSearchOperatorStructLessEqual = new SearchOperatorStruct();
            lSearchOperatorStructLessEqual.Value = SearchOperator.LESSEQUAL;
            lSearchOperatorStructLessEqual.Name = SearchOperatorString.GetString(SearchOperator.LESSEQUAL);
            SearchOperatorStructIntList.Add(lSearchOperatorStructLessEqual);

        }

        void InitOperatorStrList()
        {
            SearchOperatorStruct lSearchOperatorStructContains = new SearchOperatorStruct();
            lSearchOperatorStructContains.Value = SearchOperator.CONTAINS;
            lSearchOperatorStructContains.Name = SearchOperatorString.GetString(SearchOperator.CONTAINS);
            SearchOperatorStructStrList.Add(lSearchOperatorStructContains);

            SearchOperatorStruct lSearchOperatorStructNotContains = new SearchOperatorStruct();
            lSearchOperatorStructNotContains.Value = SearchOperator.NOTCONTAINS;
            lSearchOperatorStructNotContains.Name = SearchOperatorString.GetString(SearchOperator.NOTCONTAINS);
            SearchOperatorStructStrList.Add(lSearchOperatorStructNotContains);
        }

        void InitOperatorBoolList()
        {
            SearchOperatorStruct lSearchOperatorStructEqual = new SearchOperatorStruct();
            lSearchOperatorStructEqual.Value = SearchOperator.EQUAL;
            lSearchOperatorStructEqual.Name = SearchOperatorString.GetString(SearchOperator.EQUAL);
            SearchOperatorStructBoolList.Add(lSearchOperatorStructEqual);

            SearchOperatorStruct lSearchOperatorStructNoEqual = new SearchOperatorStruct();
            lSearchOperatorStructNoEqual.Value = SearchOperator.NOEQUAL;
            lSearchOperatorStructNoEqual.Name = SearchOperatorString.GetString(SearchOperator.NOEQUAL);
            SearchOperatorStructBoolList.Add(lSearchOperatorStructNoEqual);
        }

        public void SetTacPayerSearchEntity(TaxPayerSearchEntity aTaxPayerSearchEntity)
        {
            foreach(SearchConditionStruct lSearchConditionStruct in SearchConditionList)
            {
                if(lSearchConditionStruct.Value == aTaxPayerSearchEntity.TPSearchCondition)
                {
                    SelectSearchCondition = lSearchConditionStruct;
                }
            }

            foreach (TaxPayerFieldStruct lTaxPayerFieldStruct in TaxPayerFieldList)
            {
                if (lTaxPayerFieldStruct.Value == aTaxPayerSearchEntity.TPTaxPayerField)
                {
                    SelectTaxPayerField = lTaxPayerFieldStruct;
                }
            }

            foreach (SearchOperatorStruct lSearchOperatorStruct in SearchOperatorList)
            {
                if (lSearchOperatorStruct.Value == aTaxPayerSearchEntity.TPSearchOperator)
                {
                    SelectSearchOperator = lSearchOperatorStruct;
                }
            }

            ValueStr = aTaxPayerSearchEntity.ValueStr;
            ValueInt = aTaxPayerSearchEntity.ValueInt;
            ValueBoolean = aTaxPayerSearchEntity.ValueBoolean;
        }

        public TaxPayerSearchEntity GetTaxPayerSearchEntity()
        {
            TaxPayerSearchEntity lTaxPayerSearchEntity = new TaxPayerSearchEntity();

            if(SelectSearchCondition != null)
            {
                lTaxPayerSearchEntity.TPSearchCondition = SelectSearchCondition.Value;
            }

            if (SelectTaxPayerField != null)
            {
                lTaxPayerSearchEntity.TPTaxPayerField = SelectTaxPayerField.Value;
            }

            if (SelectSearchOperator != null)
            {
                lTaxPayerSearchEntity.TPSearchOperator = SelectSearchOperator.Value;
            }

            lTaxPayerSearchEntity.GroupID = GroupID;
            lTaxPayerSearchEntity.ValueBoolean = ValueBoolean;
            lTaxPayerSearchEntity.ValueInt = ValueInt;
            lTaxPayerSearchEntity.ValueStr = ValueStr;
            lTaxPayerSearchEntity.TaxPayerTypeList = TaxPayerTypeList;
            return lTaxPayerSearchEntity;
        }
    }
}
