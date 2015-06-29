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
using DocumentManager.Views;
using Microsoft.Windows.Data.DomainServices;
using System.Collections.ObjectModel;
using DocumentManager.Model.Entities;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Lite.ExcelLibrary.SpreadSheet;
using System.IO;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{
    public class StandBookJAViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;

        private DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer> taxPayerLoader;
        private EntityList<DocumentManager.Web.Model.taxpayer> taxPayerSource;

        public ObservableCollection<TaxPayerEntity> TaxPayerList { get; set; }

        private Dictionary<int, TaxPayerTypeEntity> TaxPayerTypeEntityDictionary { get; set; }

        private Dictionary<string, decimal> ProjectTotalMonay;
        private Dictionary<string, decimal> ProjectPartMoney;
        private Dictionary<string, decimal> ProjectTax;
        private Dictionary<string, DateTime> ProjectStandBookDateTime;

        public ICommand OnInputStandBook { get; private set; }
        public ICommand OnOutputStandBook { get; private set; }
        public ICommand OnDeleteStandBook { get; private set; }

        public ICommand OnOpenDownloadTempFile { get; private set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        private TaxPayerEntity selectTaxPayerEntity;
        public TaxPayerEntity SelectTaxPayerEntity
        {
            get
            {
                return selectTaxPayerEntity;
            }
            set
            {
                if (selectTaxPayerEntity != value)
                {
                    selectTaxPayerEntity = value;
                    StandBookViewModel.TaxPayerEntity = value;
                    UpdateChanged("SelectTaxPayerEntity");
                }
            }
        }

        private StandBookEntity selectStandBookEntity;
        public StandBookEntity SelectStandBookEntity
        {
            get
            {
                return selectStandBookEntity;
            }
            set
            {
                if (selectStandBookEntity != value)
                {
                    selectStandBookEntity = value;
                    UpdateChanged("SelectStandBookEntity");
                    (OnDeleteStandBook as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public StandBookViewModel StandBookViewModel { get; set; }

        public SaveFileDialog TempSaveFileDialog { get; set; }

        public StandBookJAViewModel()
        {
            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();

            TaxPayerList = new ObservableCollection<TaxPayerEntity>();
            TaxPayerTypeEntityDictionary = new Dictionary<int, TaxPayerTypeEntity>();

            ProjectTotalMonay = new Dictionary<string, decimal>();
            ProjectPartMoney = new Dictionary<string, decimal>();
            ProjectTax = new Dictionary<string, decimal>();
            ProjectStandBookDateTime = new Dictionary<string, DateTime>();

            StandBookViewModel = new StandBookViewModel();
            StandBookViewModel.BeginLoadings += BeginLoading;
            StandBookViewModel.FinishLoadings += FinishLoading;
            StandBookViewModel.StandBookJAViewModel = this;
            OnInputStandBook = new DelegateCommand(onInputStandBook);
            OnOutputStandBook = new DelegateCommand(onOutputStandBook);
            OnDeleteStandBook = new DelegateCommand(onDeleteStandBook, canDeleteStandBook);

            OnOpenDownloadTempFile = new DelegateCommand(onOpenDownloadTempFile);
        }

        private void BeginLoading(object sender, EventArgs e)
        {
            IsBusy = true;
        }

        private void FinishLoading(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        public void LoadData()
        {
            IsBusy = true;
            LoadOperation<DocumentManager.Web.Model.standbook> loadOperationStandBookType =
                documentManagerContext.Load<DocumentManager.Web.Model.standbook>(documentManagerContext.GetStandBookQuery());
            loadOperationStandBookType.Completed += loadOperationStandBook_Completed;
        }

        private void loadOperationStandBook_Completed(object sender, EventArgs e)
        {
            ProjectTotalMonay.Clear();
            ProjectPartMoney.Clear();
            ProjectTax.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.standbook standbook in loadOperation.Entities)
            {
                StandBookEntity lStandBookEntity = new StandBookEntity();
                lStandBookEntity.StandBook = standbook;
                lStandBookEntity.Update();

                string lProjectName = lStandBookEntity.ProjectName;
                
                decimal lTotalMoney;
                if (!ProjectTotalMonay.TryGetValue(lProjectName, out lTotalMoney))
                {
                    ProjectTotalMonay.Add(lProjectName, lStandBookEntity.TotalMoney.GetValueOrDefault(0));
                }

                decimal lPartMoney;
                if (ProjectPartMoney.TryGetValue(lProjectName, out lPartMoney))
                {
                    ProjectPartMoney[lProjectName] += lStandBookEntity.ThisPartMoney.GetValueOrDefault(0);
                }
                else
                {
                    ProjectPartMoney.Add(lProjectName, lStandBookEntity.ThisPartMoney.GetValueOrDefault(0));
                }

                decimal lTax;
                if (ProjectTax.TryGetValue(lProjectName, out lTax))
                {
                    ProjectTax[lProjectName] += lStandBookEntity.TotalTax;
                }
                else
                {
                    ProjectTax.Add(lProjectName, lStandBookEntity.TotalTax);
                }

                DateTime lDataTime;
                if (!ProjectStandBookDateTime.TryGetValue(lProjectName, out lDataTime))
                {
                    ProjectStandBookDateTime.Add(lProjectName, lStandBookEntity.PayTime.GetValueOrDefault());
                }

                //TaxPayerTypeEntityDictionary.Add(lTaxPayerTypeEntity.TaxPayerTypeId, lTaxPayerTypeEntity);
            }

            taxPayerSource = new EntityList<Web.Model.taxpayer>(documentManagerContext.taxpayers);
            taxPayerLoader = new DomainCollectionViewLoader<Web.Model.taxpayer>(
                LoadTaxPayerEntities,
                loadOperation_Completed);
            taxPayerView = new DomainCollectionView<Web.Model.taxpayer>(taxPayerLoader, taxPayerSource);


            using (this.taxPayerView.DeferRefresh())
            {
                this.taxPayerView.MoveToFirstPage();
            }
        }

        private LoadOperation<Web.Model.taxpayer> LoadTaxPayerEntities()
        {
            IsBusy = false;
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuery = documentManagerContext.GetTaxpayerQuery();
            lQuery = lQuery.Where(c => (c.taxpayer_group_id == 1));

            return documentManagerContext.Load(lQuery.SortAndPageBy(this.taxPayerView));
        }

        private void loadOperation_Completed(LoadOperation<DocumentManager.Web.Model.taxpayer> sender)
        {
            TaxPayerList.Clear();
            taxPayerSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.taxpayer taxpayer in sender.Entities)
            {
                TaxPayerEntity taxPayerEntity = new TaxPayerEntity();
                taxPayerEntity.TaxPayer = taxpayer;
                taxPayerEntity.Update();

                string lProjectName = taxPayerEntity.TaxPayerProject;

                if (lProjectName == null)
                {
                    continue;
                }

                decimal lTotalMoney;
                if (ProjectTotalMonay.TryGetValue(lProjectName, out lTotalMoney))
                {
                    taxPayerEntity.TotalMoney = lTotalMoney;
                }

                decimal lPartMoney;
                if (ProjectPartMoney.TryGetValue(lProjectName, out lPartMoney))
                {
                    taxPayerEntity.PartMoney = lPartMoney;
                }

                decimal lTax;
                if (ProjectTax.TryGetValue(lProjectName, out lTax))
                {
                    taxPayerEntity.TotalTax = lTax;
                }

                DateTime lDateTime;
                if (ProjectStandBookDateTime.TryGetValue(lProjectName, out lDateTime))
                {
                    taxPayerEntity.FirstStandBookDateTime = lDateTime;
                }

                TaxPayerList.Add(taxPayerEntity);
            }
            UpdateChanged("TaxPayerList");
            IsBusy = false;
        }

        private void onInputStandBook()
        {
            InputStandBookJA lInputStandBookJA = new InputStandBookJA();
            lInputStandBookJA.Closed += new EventHandler(InputStandBook_Closed);
            lInputStandBookJA.Show();
        }

        private void InputStandBook_Closed(object sender, EventArgs e)
        {
            LoadData();
        }

        private string getAbsPath(string aPath)
        {
            if (string.IsNullOrEmpty(aPath))
            {
                return aPath;
            }

            string fullUrl;

            if (aPath.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
             || aPath.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
             || aPath.StartsWith("file:", StringComparison.OrdinalIgnoreCase)
                )
            {
                fullUrl = aPath;
            }
            else
            {
                fullUrl = System.Windows.Application.Current.Host.Source.AbsoluteUri;
                if (fullUrl.IndexOf("ClientBin") > 0)
                {
                    fullUrl = fullUrl.Substring(0, fullUrl.IndexOf("ClientBin")) + aPath;
                }
                else
                {
                    fullUrl = fullUrl.Substring(0, fullUrl.LastIndexOf("/") + 1) + aPath;
                }
            }
            return fullUrl;
        }

        private void onOpenDownloadTempFile()
        {
            TempSaveFileDialog = new SaveFileDialog();
            TempSaveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(TempSaveFileDialog, new object[] { "建安申请表模板.xls" });

            bool? saveResult = TempSaveFileDialog.ShowDialog();

            if (saveResult.HasValue)
            {
                if (saveResult.Value == true)
                {
                    string appstr = System.Windows.Application.Current.Host.Source.AbsoluteUri;

                    WebClient client = new WebClient();
                    Uri uri = new Uri(getAbsPath("TempFiles/申报表_模板.xls"), UriKind.RelativeOrAbsolute);

                    client.OpenReadCompleted += client_OpenReadCompleted;
                    client.OpenReadAsync(uri);
                }
            }

        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                using (Stream sf = (Stream)TempSaveFileDialog.OpenFile())
                {
                    e.Result.CopyTo(sf);
                    sf.Flush();

                    NotifyWindow notificationWindow = new NotifyWindow("下载完成", "下载模板文件 " + "申报表_模板" + " 完成");
                    notificationWindow.Show();
                }
            }
            else
            {
                NotifyWindow notificationWindow = new NotifyWindow("下载失败", "下载模板文件 " + "申报表_模板" + " 失败");
                notificationWindow.Show();
            }
            //throw new NotImplementedException();
        }

        private void onOutputStandBook()
        {
            /*SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == true)
            {
                Workbook workbook = new Workbook();

                Worksheet lMainWorksheet = new Worksheet("台账封面");

                // 台账封面
                {
                    // 标题
                    Cell lTitleCell = new Cell("基建项目征收总台账");
                    lMainWorksheet.Cells[0, 0] = lTitleCell;
                    //lTitleCellStyle.RichTextFormat =
                    //lTitleCell.Style = lTitleCellStyle;
                }
                workbook.Worksheets.Add(lMainWorksheet);
                Stream sFile = sDialog.OpenFile();
                workbook.Save(sFile);
            }
            */
        }

        bool canDeleteStandBook(object obj)
        {
            return SelectStandBookEntity != null;
        }

        void onDeleteStandBook()
        {
            ConfirmWindow lConfirmWindow = new ConfirmWindow("是否删除台账"
                , "是否删除台账, 项目:" + SelectStandBookEntity.ProjectName + ", 本次拨款金额:" + SelectStandBookEntity.ThisPartMoney.GetValueOrDefault(0).ToString("#0,##0.0") + ",申报日期：" + SelectStandBookEntity.PayTime.GetValueOrDefault().ToString("yyyy年MM月dd日"));
            lConfirmWindow.Closed += new EventHandler(DeleteStandBookConfirmWindow_Closed);
            lConfirmWindow.Show();
        }

        void DeleteStandBookConfirmWindow_Closed(object sender, EventArgs e)
        {
            ConfirmWindow lConfirmWindow = sender as ConfirmWindow;
            if (lConfirmWindow.DialogResult == true)
            {
                StandBookViewModel.DeleteStandBook(SelectStandBookEntity.StandBook);
                //documentManagerContext.standbooks.Remove(SelectStandBookEntity.StandBook);
                //SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
                //lSubmitOperation.Completed += SubOperation_Completed;
            }
        }

        private void SubOperation_Completed(object sender, EventArgs e)
        {
//             SubmitOperation submitOperation = sender as SubmitOperation;
//             if (submitOperation.HasError)
//             {
//                 submitOperation.MarkErrorAsHandled();
//                 NotifyWindow notifyWindow = new NotifyWindow("错误", "删除失败 " + submitOperation.Error);
//                 notifyWindow.Show();
//             }
//             else
//             {
//                 NotifyWindow notifyWindow = new NotifyWindow("删除成功", "删除成功！");
//                 notifyWindow.Show();
//             }
// 
//             LoadData();
        }
    }
}
