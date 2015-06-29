using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using DocumentManager.Views;
using DocumentManager.Model.Entities;
using Microsoft.Windows.Data.DomainServices;

namespace DocumentManager.Model.SearchEntities
{
    public class TaxPayerSearch : NotifyPropertyChanged
    {
        public ObservableCollection<TaxPayerSearchEntity> TaxPayerSearchEntitis { get; set; }
        public ObservableCollection<TaxPayerTypeEntity> TaxPayerTypeList { get; set; }

        public DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;

        private ParameterExpression Param; //= Expression.Parameter(typeof(DocumentManager.Web.Model.taxpayer), "TaxPayer");

        private TaxPayerSearchEntity selectTaxPayerSearchEntity;
        public TaxPayerSearchEntity SelectTaxPayerSearchEntity
        {
            get { return selectTaxPayerSearchEntity; }
            set
            {
                if (selectTaxPayerSearchEntity != value)
                {
                    selectTaxPayerSearchEntity = value;
                    UpdateChanged("SelectTaxPayerSearchEntity");
                    (OnModifyCondition as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteCondition as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnAddCondition { get; private set; }
        public ICommand OnModifyCondition { get; private set; }
        public ICommand OnDeleteCondition { get; private set; }
        public ICommand OnSearch { get; private set; }

        public int GroupID { get; set; }

        public TaxPayerSearch()
        {
            TaxPayerSearchEntitis = new ObservableCollection<TaxPayerSearchEntity>();
            Param = Expression.Parameter(typeof(DocumentManager.Web.Model.taxpayer), "TaxPayer");

            OnAddCondition = new DelegateCommand(onAddCondition);
            OnModifyCondition = new DelegateCommand(onModifyCondition, canModeifyCondition);
            OnDeleteCondition = new DelegateCommand(onDeleteCondition, canDeleteCondition);
            OnSearch = new DelegateCommand(onSearch);
        }

        private TaxPayerSearchEntity AddTaxPayerSearchEntity;

        private void onAddCondition()
        {
            bool lIsFirst = true;
            if(TaxPayerSearchEntitis.Count > 0)
            {
                lIsFirst = false;
            }
            AddSearchWindow lAddSearchWindow = new AddSearchWindow(GroupID, lIsFirst, TaxPayerTypeList);
            lAddSearchWindow.Closed += new EventHandler(lAddSearchWindow_Closed);
            lAddSearchWindow.Show();
        }

        void lAddSearchWindow_Closed(object sender, EventArgs e)
        {
            AddSearchWindow lAddSearchWindow = sender as AddSearchWindow;
            if (lAddSearchWindow.DialogResult == true)
            {
                TaxPayerSearchEntity laddTaxPayerSearchEntity = lAddSearchWindow.GetTaxPayerSearchEntity();
                TaxPayerSearchEntitis.Add(laddTaxPayerSearchEntity);
            }
        }

        private void onModifyCondition()
        {
            int lIndex = 0;
            foreach (TaxPayerSearchEntity lTaxPayerSearchEntity in TaxPayerSearchEntitis)
            {
                if (lTaxPayerSearchEntity == SelectTaxPayerSearchEntity)
                {
                    break;
                }
                lIndex++;
            }
            

            bool lIsFirst = true;
            if (lIndex > 0)
            {
                lIsFirst = false;
            }

            AddSearchWindow lAddSearchWindow = new AddSearchWindow(GroupID, lIsFirst, TaxPayerTypeList);

            lAddSearchWindow.SetTaxPayerSearchEntity(SelectTaxPayerSearchEntity);
            lAddSearchWindow.Closed += new EventHandler(lAddSearchWindow_ClosedModify);
            lAddSearchWindow.Show();
        }

        void lAddSearchWindow_ClosedModify(object sender, EventArgs e)
        {
            AddSearchWindow lAddSearchWindow = sender as AddSearchWindow;
            if (lAddSearchWindow.DialogResult == true)
            {
                TaxPayerSearchEntity laddTaxPayerSearchEntity = lAddSearchWindow.GetTaxPayerSearchEntity();
                //SelectTaxPayerSearchEntity = laddTaxPayerSearchEntity;
                int lIndex = 0;
                foreach (TaxPayerSearchEntity lTaxPayerSearchEntity in TaxPayerSearchEntitis)
                {
                    if (lTaxPayerSearchEntity == SelectTaxPayerSearchEntity)
                    {
                        break;
                    }
                    lIndex++;
                }
                TaxPayerSearchEntitis[lIndex] = laddTaxPayerSearchEntity;

                //TaxPayerSearchEntitis.Add(laddTaxPayerSearchEntity);
            }
        }

        private bool canModeifyCondition(Object aObject)
        {
            return SelectTaxPayerSearchEntity != null;
        }

        private void onDeleteCondition()
        {
            TaxPayerSearchEntitis.Remove(SelectTaxPayerSearchEntity);
            if (TaxPayerSearchEntitis.Count > 0)
            {
                TaxPayerSearchEntitis[0].TPSearchCondition = SearchCondition.KNULL;
            }
        }

        private bool canDeleteCondition(Object aObject)
        {
            return SelectTaxPayerSearchEntity != null;
        }

        private void onSearch()
        {
            using (this.taxPayerView.DeferRefresh())
            {
                this.taxPayerView.MoveToFirstPage();
            }
        }

        public EntityQuery<DocumentManager.Web.Model.taxpayer> CreateQuery(EntityQuery<DocumentManager.Web.Model.taxpayer> aQuerable)
        {
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuerable = aQuerable;

            int lIndex = 0;
            Expression lExpression = Expression.Constant(true);
            foreach (TaxPayerSearchEntity lTaxPayerSearchEntity in TaxPayerSearchEntitis)
            {
                if (lIndex == 0)
                {
                    lExpression = FirstExpression(lTaxPayerSearchEntity);
                }
                else
                {
                    lExpression = CreateExpression(lExpression, lTaxPayerSearchEntity);
                }
                lIndex++;
                //lQuerable = Search(lQuerable, lTaxPayerSearchEntity);
            }
            return lQuerable.Where((Expression<Func<DocumentManager.Web.Model.taxpayer, bool>>)Expression.Lambda(lExpression, Param));
        }

        private Expression FirstExpression(TaxPayerSearchEntity aTaxPayerSearchEntity)
        {
            

            Expression filter = Expression.Constant(true);

            Expression left = Expression.Property(Param, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)));
            Expression right = Expression.Constant(null);
            switch (aTaxPayerSearchEntity.TPValueType)
            {
                case ValueType.VALUEBOOLEAN:
                    //right = Expression.Constant(aTaxPayerSearchEntity.ValueBoolean);
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueBoolean, typeof(bool?));
                    break;
                case ValueType.VALUEINT:
                    //right = Expression.Constant(aTaxPayerSearchEntity.ValueInt);
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueInt, typeof(int?));
                    break;
                case ValueType.VALUESTR:
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueStr);
                    break;
            }

            switch (aTaxPayerSearchEntity.TPSearchOperator)
            {
                case SearchOperator.CONTAINS:
                    filter = Expression.Call(Expression.Property(Param, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)))
                        , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                        , Expression.Constant(aTaxPayerSearchEntity.ValueStr));
                    //totalExpr = Expression.And(filter, totalExpr);
                    break;
                case SearchOperator.NOTCONTAINS:
                    filter = Expression.Call(Expression.Property(Param, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)))
                        , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                        , Expression.Constant(aTaxPayerSearchEntity.ValueStr));
                    filter = Expression.Not(filter);
                    break;
                case SearchOperator.EQUAL:
                    filter = Expression.Equal(left, right);
                    break;
                case SearchOperator.NOEQUAL:
                    filter = Expression.NotEqual(left, right);
                    break;
                case SearchOperator.GREATER:
                    filter = Expression.GreaterThan(left, right);
                    break;
                case SearchOperator.LESS:
                    filter = Expression.LessThan(left, right);
                    break;
                case SearchOperator.GREATEREQUAL:
                    filter = Expression.GreaterThanOrEqual(left, right);
                    break;
                case SearchOperator.LESSEQUAL:
                    filter = Expression.LessThanOrEqual(left, right);
                    break;
            }

            return filter;
        }

        private Expression CreateExpression(Expression aExpression, TaxPayerSearchEntity aTaxPayerSearchEntity)
        {
            //ParameterExpression lParam = Expression.Parameter(typeof(DocumentManager.Web.Model.taxpayer), "TaxPayer");

            Expression filter = Expression.Constant(true);

            Expression left = Expression.Property(Param, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)));
            Expression right = Expression.Constant(null);
            switch (aTaxPayerSearchEntity.TPValueType)
            {
                case ValueType.VALUEBOOLEAN:
                    //right = Expression.Constant(aTaxPayerSearchEntity.ValueBoolean);
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueBoolean, typeof(bool?));
                    break;
                case ValueType.VALUEINT:
                    //right = Expression.Constant(aTaxPayerSearchEntity.ValueInt);
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueInt, typeof(int?));
                    break;
                case ValueType.VALUESTR:
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueStr);
                    break;
            }

            switch (aTaxPayerSearchEntity.TPSearchOperator)
            {
                case SearchOperator.CONTAINS:
                    filter = Expression.Call(Expression.Property(Param, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)))
                        , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                        , Expression.Constant(aTaxPayerSearchEntity.ValueStr));
                    //totalExpr = Expression.And(filter, totalExpr);
                    break;
                case SearchOperator.NOTCONTAINS:
                    filter = Expression.Call(Expression.Property(Param, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)))
                        , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                        , Expression.Constant(aTaxPayerSearchEntity.ValueStr));
                    filter = Expression.Not(filter);
                    break;
                case SearchOperator.EQUAL:
                    filter = Expression.Equal(left, right);
                    break;
                case SearchOperator.NOEQUAL:
                    filter = Expression.NotEqual(left, right);
                    break;
                case SearchOperator.GREATER:
                    filter = Expression.GreaterThan(left, right);
                    break;
                case SearchOperator.LESS:
                    filter = Expression.LessThan(left, right);
                    break;
                case SearchOperator.GREATEREQUAL:
                    filter = Expression.GreaterThanOrEqual(left, right);
                    break;
                case SearchOperator.LESSEQUAL:
                    filter = Expression.LessThanOrEqual(left, right);
                    break;
            }

            switch (aTaxPayerSearchEntity.TPSearchCondition)
            {
                case SearchCondition.AND:
                    filter = Expression.AndAlso(filter, aExpression);
                    break;
                case SearchCondition.OR:
                    filter = Expression.OrElse(filter, aExpression);
                    break;
            }

            return filter;
        }


        private EntityQuery<DocumentManager.Web.Model.taxpayer> TaxPayerQuery(EntityQuery<DocumentManager.Web.Model.taxpayer> aQuerable, TaxPayerSearchEntity aTaxPayerSearchEntity)
        {
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuerable = aQuerable;

            switch (aTaxPayerSearchEntity.TPTaxPayerField)
            {
                case TaxPayerField.CODE:
                    lQuerable = Search(lQuerable, aTaxPayerSearchEntity);
                    break;
                case TaxPayerField.NAME:
                    break;
                case TaxPayerField.TYPE:
                    break;
                case TaxPayerField.PROJECT:
                    break;
                case TaxPayerField.FINISH:
                    break;
                case TaxPayerField.FREE:
                    break;
            }
            return lQuerable;
        }

        public EntityQuery<DocumentManager.Web.Model.taxpayer> Search(EntityQuery<DocumentManager.Web.Model.taxpayer> aQuerable
                                                                    , TaxPayerSearchEntity aTaxPayerSearchEntity)
        {
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuerable = aQuerable;

            Expression filter = Expression.Constant(true);
            Expression totalExpr = Expression.Constant(true);

            ParameterExpression lParam = Expression.Parameter(typeof(DocumentManager.Web.Model.taxpayer), "TaxPayer");
            
            {
               // Expression leftk = Expression.Property(lParam, typeof(DocumentManager.Web.Model.taxpayer).GetProperty("taxpayer_group_id"));
                //Nullable<int> lValue = 0;

                //Expression rightk = Expression.Constant(0);
                //totalExpr = Expression.Equal(leftk, rightk);
            }
            
            Expression left = Expression.Property(lParam, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)));
            Expression right = Expression.Constant(null);
            switch(aTaxPayerSearchEntity.TPValueType)
            {
                case ValueType.VALUEBOOLEAN:
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueBoolean);
                    break;
                case ValueType.VALUEINT:
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueInt);
                    break;
                case ValueType.VALUESTR:
                    right = Expression.Constant(aTaxPayerSearchEntity.ValueStr);
                    break;
            }

            switch (aTaxPayerSearchEntity.TPSearchOperator)
            {
                case SearchOperator.CONTAINS:
                    filter = Expression.Call( Expression.Property( lParam, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)))
                        , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                        , Expression.Constant(aTaxPayerSearchEntity.ValueStr));
                    //totalExpr = Expression.And(filter, totalExpr);
                    break;
                case SearchOperator.NOTCONTAINS:
                    filter = Expression.Call(Expression.Property(lParam, typeof(DocumentManager.Web.Model.taxpayer).GetProperty(TaxPayerFieldFieldString.GetString(aTaxPayerSearchEntity.TPTaxPayerField)))
                        , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                        , Expression.Constant(aTaxPayerSearchEntity.ValueStr));
                    filter = Expression.Not(filter);
                    break;
                case SearchOperator.EQUAL:
                    filter = Expression.Equal(left, right);
                    break;
                case SearchOperator.NOEQUAL:
                    filter = Expression.NotEqual(left, right);
                    break;
                case SearchOperator.GREATER:
                    filter = Expression.GreaterThan(left, right);
                    break;
                case SearchOperator.LESS:
                    filter = Expression.LessThan(left, right);
                    break;
                case SearchOperator.GREATEREQUAL:
                    filter = Expression.GreaterThanOrEqual(left, right);
                    break;
                case SearchOperator.LESSEQUAL:
                    filter = Expression.LessThanOrEqual(left, right);
                    break;
            }

            switch (aTaxPayerSearchEntity.TPSearchCondition)
            {
                case SearchCondition.AND:
                    totalExpr = Expression.AndAlso(filter, totalExpr);
                    break;
                case SearchCondition.OR:
                    totalExpr = Expression.OrElse(filter, totalExpr);
                    break;
            }

            //Expression pred = Expression.Lambda(totalExpr, lParam);
            //Expression whereExpression = Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(DocumentManager.Web.Model.taxpayer) }, Expression.Constant(lQuerable), pred);
            //OrderBy部分排序
            //MethodCallExpression orderByCallExpression = Expression.Call(typeof(Queryable), queryCondition.IsDesc ? "OrderByDescending" : "OrderBy", new Type[] { typeof(TestUser), orderEntries[queryCondition.OrderField].OrderType }, whereExpression, Expression.Lambda(Expression.Property(param, orderEntries[queryCondition.OrderField].OrderStr), param));
            //EntityQuery<DocumentManager.Web.Model.taxpayer>()
            //return lQuerable.Query.AsQueryable().Provider.CreateQuery<DocumentManager.Web.Model.taxpayer>(whereExpression);
            //return lQuerable.Query.AsQueryable().Provider.CreateQuery<DocumentManager.Web.Model.taxpayer>(whereExpression);
            return lQuerable.Where((Expression<Func<DocumentManager.Web.Model.taxpayer, bool>>)Expression.Lambda(totalExpr, lParam));
        }
    }
}
