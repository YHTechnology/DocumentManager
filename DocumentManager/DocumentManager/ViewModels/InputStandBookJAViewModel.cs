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
using DocumentManager.Model.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.IO;
using Lite.ExcelLibrary.SpreadSheet;
using DocumentManager.Controls;
using System.Collections.ObjectModel;
using FileHelper;

namespace DocumentManager.ViewModels
{
    public class InputStandBookJAViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;

        private ChildWindow childWindow;

        public ICommand OnOpenStandBookFile { get; private set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public StandBookEntity StandBookEntity { get; set; }

        public Visibility HasShowTemp { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } } 
        }

        public Dictionary<int, TaxPayerEntity> TaxPayerEntityDirectory { get; set; }
        public Dictionary<string, StandBookEntity> StandBookEntityDirectory { get; set; }

        private bool canInput;
        public bool CanInput
        {
            get { return canInput; }
            set
            {
                if (canInput != value)
                {
                    canInput = value;
                    UpdateChanged("CanInput");
                    (OnOK as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }


        private FileTypeEntity selectFileTypeEntity;
        public FileTypeEntity SelectFileTypeEntity
        {
            get { return selectFileTypeEntity; }
            set { if (selectFileTypeEntity != value) { selectFileTypeEntity = value; UpdateChanged("SelectFileTypeEntity"); (OnOK as DelegateCommand).RaiseCanExecuteChanged(); } }
        }
        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }

        private TaxPayerDocumentEntity taxPayerDocumentEntity;

        public UserFile UserFile { get; set; }

        private Visibility showProgess;
        public Visibility ShowProgress
        {
            get { return showProgess; }
            set
            {
                if (showProgess != value)
                {
                    showProgess = value;
                    UpdateChanged("ShowProgress");
                }
            }
        }

        private Visibility showUpdate;
        public Visibility ShowUpdate
        {
            get { return showUpdate; }
            set
            {
                if (showUpdate != value)
                {
                    showUpdate = value;
                    UpdateChanged("ShowUpdate");
                }
            }
        }

        private TaxPayerEntity currentTaxPayerEntity;

        public InputStandBookJAViewModel(ChildWindow aChildWindow)
        {
            IsBusy = false;
            TaxPayerEntityDirectory = new Dictionary<int, TaxPayerEntity>();
            StandBookEntityDirectory = new Dictionary<string, StandBookEntity>();
            FileTypeEntityList = new ObservableCollection<FileTypeEntity>();

            childWindow = aChildWindow;

            OnOpenStandBookFile = new DelegateCommand(onOpenStandBookFile);
            OnOK = new DelegateCommand(onOK, canOK);
            OnCancel = new DelegateCommand(onCancel);

            StandBookEntity = new StandBookEntity();
            HasShowTemp = Visibility.Collapsed;
            ShowProgress = Visibility.Collapsed;
            ShowUpdate = Visibility.Visible;

            documentManagerContext = new Web.DocumentManagerDomainContext();
            CanInput = false;
            LoadData();
        }

        private void LoadData()
        {
            IsBusy = true;
            LoadOperation<DocumentManager.Web.Model.filetype> loadOperationFileType =
                documentManagerContext.Load<DocumentManager.Web.Model.filetype>(documentManagerContext.GetFiletypeQuery());
            loadOperationFileType.Completed += loadOperationFileType_Completed;
        }

        private void loadOperationFileType_Completed(object sender, EventArgs e)
        {
            FileTypeEntityList.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.filetype filetype in loadOperation.Entities)
            {
                FileTypeEntity lFileTypeEntity = new FileTypeEntity();
                lFileTypeEntity.FileType = filetype;
                lFileTypeEntity.Update();
                FileTypeEntityList.Add(lFileTypeEntity);
            }
        
            LoadOperation<DocumentManager.Web.Model.taxpayer> loadOperationTaxPayer =
                documentManagerContext.Load<DocumentManager.Web.Model.taxpayer>(documentManagerContext.GetTaxpayerQuery());
            loadOperationTaxPayer.Completed += loadOperationTaxPayer_Completed;
        }

        private void loadOperationTaxPayer_Completed(object sender, EventArgs e)
        {
            TaxPayerEntityDirectory.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.taxpayer taxpayer in loadOperation.Entities)
            {
                TaxPayerEntity lTaxPayerEntity = new TaxPayerEntity();
                lTaxPayerEntity.TaxPayer = taxpayer;
                lTaxPayerEntity.Update();
                TaxPayerEntityDirectory.Add(lTaxPayerEntity.TaxPayerId, lTaxPayerEntity);
            }

            LoadOperation<DocumentManager.Web.Model.standbook> loadOperationStandBook =
                documentManagerContext.Load<DocumentManager.Web.Model.standbook>(documentManagerContext.GetStandBookQuery());
            loadOperationStandBook.Completed += loadOperationStandBook_Completed;
        }

        private void loadOperationStandBook_Completed(object sender, EventArgs e)
        {
            StandBookEntityDirectory.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (Web.Model.standbook standbook in loadOperation.Entities)
            {
                StandBookEntity lStandBookEntity = new StandBookEntity();
                lStandBookEntity.StandBook = standbook;
                lStandBookEntity.Update();
                StandBookEntityDirectory.Add(lStandBookEntity.StandBookID, lStandBookEntity);
            }

            IsBusy = false;
        }

        private void onOpenStandBookFile()
        {
            OpenFileDialog oFile = new OpenFileDialog();
            // .xls filter specified to select only .xls file.
            oFile.Filter = "Excel (*.xls)|*.xls";

            if (oFile.ShowDialog() == true)
            {
                HasShowTemp = Visibility.Collapsed;
                try
                {
                    FileStream fs = oFile.File.OpenRead();
                    Workbook book = Workbook.Open(fs);
                    bool lIsSuccessRead = ReadSheet(book.Worksheets[0]);
                    if (lIsSuccessRead)
                    {
                        fs.Close();
                        HasShowTemp = Visibility.Visible;
                        CanInput = CheckVaild();
                        if (CanInput)
                        {
                            String fileName = oFile.File.Name;
                            UserFile = new UserFile();
                            DateTime lDateTime = DateTime.Now;
                            UserFile.FileName = lDateTime.ToString("yyyyMMdd_HHmmss_") + fileName;
                            UserFile.FileFolder = currentTaxPayerEntity.TaxPayerId.ToString();
                            UserFile.FileStream = oFile.File.OpenRead();

                            taxPayerDocumentEntity = new TaxPayerDocumentEntity();
                            taxPayerDocumentEntity.TaxPayerDocumentBytes = UserFile.FileStream.Length;
                        }

                        UpdateChanged("StandBookEntity");
                    }
                }
                catch (Exception ex)
                {
                    NotifyWindow lNotifyWindow = new NotifyWindow("打开文件出错", "请检查文件是否被其他程序打开或者文件中是否有自定义数据类型，请与模板比对！");
                    lNotifyWindow.Show();
                }
                UpdateChanged("HasShowTemp");
            }
        }

        private void onOK()
        {
            //StandBookEntity.StandBook = new Web.Model.standbook();
            //StandBookEntity.DUpdate();
            //childWindow.DialogResult = true;
            ShowUpdate = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            UserFile.FinishUpdates += UserFile_FinishUpdates;
            UserFile.Upload(UserFile.FileFolder, childWindow.Dispatcher);
        }

        private bool canOK(Object aObject)
        {
            return CanInput && SelectFileTypeEntity != null;
        }

        private void UserFile_FinishUpdates(object sender, EventArgs e)
        {
            ShowProgress = Visibility.Collapsed;
            taxPayerDocumentEntity.TaxPayerDocumentName = UserFile.FileName;
            taxPayerDocumentEntity.TaxPayerDocumentTypeId = SelectFileTypeEntity.FileTypeId;
            taxPayerDocumentEntity.TaxPayerId = currentTaxPayerEntity.TaxPayerId;
            taxPayerDocumentEntity.TaxPayerDocumentDescript = "";
            taxPayerDocumentEntity.TaxPayerUpdateTime = DateTime.Now;
            App app = Application.Current as App;
            taxPayerDocumentEntity.TaxPayerUpdateUserId = app.MainPageViewModel.User.UserID;
            taxPayerDocumentEntity.TaxPayerDocument = new Web.Model.taxpayerdocument();
            taxPayerDocumentEntity.DUpdate();

            documentManagerContext.taxpayerdocuments.Add(taxPayerDocumentEntity.TaxPayerDocument);
            Log.AddLog(documentManagerContext, taxPayerDocumentEntity.ToString());

            StandBookEntity.GroupID = 1;

            StandBookEntity.StandBook = new Web.Model.standbook();
            StandBookEntity.DUpdate();
            documentManagerContext.standbooks.Add(StandBookEntity.StandBook);
            Log.AddLog(documentManagerContext, StandBookEntity.ToString());

            SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
            lSubmitOperation.Completed += SubOperation_Completed;

        }

        private void SubOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "上传失败 " + submitOperation.Error);
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("上传成功", "上传成功！");
                notifyWindow.Show();
            }
        }

        

        private void onCancel()
        {
            childWindow.DialogResult = false;
        }

        private bool ReadSheet(Worksheet aSheet)
        {
            foreach (KeyValuePair<int, Row> rowPair in aSheet.Cells.Rows)
            {
                if (rowPair.Key == 0) // 第一行 流水号检查 受理流水号
                {
                    Cell lCell = rowPair.Value.GetCell(0);
                    string lValue = lCell.Value.ToString().Trim();
                    if (lValue != "税务机关代开普通发票申请表")
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "打开的不是 '税务机关代开普通发票申请表' 或者 数据不在第一个工作表里。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 1) // 第二行 流水号检查 受理流水号
                {
                    try
                    {
                        Cell lCell = rowPair.Value.GetCell(0);
                        string lValue = lCell.Value.ToString();
                        StandBookEntity.StandBookID = lValue.Substring(lValue.IndexOf(':') + 1).Trim();
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误","读取第二行 '受理流水号' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 2) // 第三行 合同项目名称 合同总金额 本次拨款金额
                {
                    try
                    {
                        Cell lCellName = rowPair.Value.GetCell(0);
                        Cell lCellTotolMoney = rowPair.Value.GetCell(2);
                        Cell lCellThisMoney = rowPair.Value.GetCell(4);
                        string lValueName = lCellName.Value.ToString();
                        StandBookEntity.ProjectName = lValueName.Substring(lValueName.IndexOf(':') + 1).Trim();
                        string lValueTotolMoney = lCellTotolMoney.Value.ToString();
                        lValueTotolMoney = lValueTotolMoney.Substring(lValueTotolMoney.IndexOf(':') + 1).Trim();
                        StandBookEntity.TotalMoney = Convert.ToDecimal(lValueTotolMoney);
                        string lValueThisMoney = lCellThisMoney.Value.ToString();
                        lValueThisMoney = lValueThisMoney.Substring(lValueThisMoney.IndexOf(':') + 1).Trim();
                        StandBookEntity.ThisPartMoney = Convert.ToDecimal(lValueThisMoney);
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误","读取第三行 '合同项目名称 合同总金额 本次拨款金额' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 3) // 第四行 综合税率 合同总应缴税额
                {
                    try
                    {
                        Cell lCellTotalRate = rowPair.Value.GetCell(1);
                        string lValueTotalRate = lCellTotalRate.Value.ToString();
                        StandBookEntity.TotalTaxRate = Convert.ToDecimal(lValueTotalRate);
                    }
                    catch (Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第四行 '综合税率 合同总应缴税额' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 4) // 第五行 申请单位
                {
                    try
                    {
                        Cell lCellCpName = rowPair.Value.GetCell(1);
                        string lValueCpName = lCellCpName.Value.ToString().Trim();
                        StandBookEntity.TaxPayerName = lValueCpName;
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第五行 '申请单位' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 7) // 第八行 经济性质
                {
                    try
                    {
                        Cell lCellEconomicNature = rowPair.Value.GetCell(1);
                        string lValueEconomicNature = lCellEconomicNature.Value.ToString().Trim();
                        StandBookEntity.EconomicNature = lValueEconomicNature;

                        Cell lCellPhoneNumber = rowPair.Value.GetCell(3);
                        string lValuePhoneNumber = lCellPhoneNumber.Value.ToString().Trim();
                        StandBookEntity.PhoneNumber = lValuePhoneNumber;

                    }
                    catch (Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第八行 '经济性质' '联系电话' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }

                }

                if (rowPair.Key == 8) // 第九行 付款单位
                {
                    try
                    {
                        Cell lCellPlayCpName = rowPair.Value.GetCell(1);
                        string lValueCpName = lCellPlayCpName.Value.ToString().Trim();
                        StandBookEntity.CapitalConstruction = lValueCpName;
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第九行 '付款单位' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 13) // 第十四行 有无外管证
                {
                    try
                    {
                        Cell lCellHasVerify = rowPair.Value.GetCell(0);
                        string lValueHasVerify = lCellHasVerify.Value.ToString().Trim();
                        lValueHasVerify = lValueHasVerify.Substring(lValueHasVerify.IndexOf(':') + 1).Trim();
                        if (lValueHasVerify == "有")
                        {
                            StandBookEntity.HasOutVerify = true;
                        }
                        else if (lValueHasVerify == "无")
                        {
                            StandBookEntity.HasOutVerify = false;
                        }
                        else
                        {
                            NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第十四行 '有无外管证' 出错！请在':'后输入'有'或'无'。");
                            lNotifyWindow.Show();
                            return false;
                        }
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误","读取第十三行 '有无外管证' 出错！请检查输入文件。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 20) // 第二十一行 法人代表
                {
                    try
                    {
                        Cell lCellJridicalPerson = rowPair.Value.GetCell(3);
                        string lValueJridicalPerson = lCellJridicalPerson.Value.ToString().Trim();
                        lValueJridicalPerson = lValueJridicalPerson.Substring(lValueJridicalPerson.IndexOf(':') + 1).Trim();
                        StandBookEntity.TaxPayerPersonName = lValueJridicalPerson;
                        if(lValueJridicalPerson == string.Empty)
                        {
                            NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第二十一行 '法人代表' 出错！请在D列输入'法人姓名:XXX'。");
                            lNotifyWindow.Show();
                            return false;
                        }
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第二十行 '法人代表' 出错！请在D列输入'法人姓名:XXX'。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 21) // 第二十二行 日期
                {
                    try
                    {
                        Cell lCellDataTime = rowPair.Value.GetCell(0);
                        string lValueDataTime = lCellDataTime.Value.ToString().Trim();
                        lValueDataTime = lValueDataTime.Substring(lValueDataTime.IndexOf(':') + 1).Trim();
                        StandBookEntity.PayTime = Convert.ToDateTime(lValueDataTime);
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误","读取第二十二行 '日期' 出错！请在A列输入'日期:yyyy年mm月dd日'。");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 25             // 第二十六行 税
                    || rowPair.Key == 26          // 第二十七行 税
                    || rowPair.Key == 27          // 第二十八行 税
                    || rowPair.Key == 28          // 第二十九行 税
                    || rowPair.Key == 29          // 第三十行 税
                    || rowPair.Key == 30          // 第三十一行 税
                    )
                {
                    string lValueTaxType;
                    string lValueTax;
                    string lValueTaxItem = "";
                    string lValueTaxRate;
                    int lKeyRow = rowPair.Key + 1;
                    try
                    {
                        Cell lCellTaxType = rowPair.Value.GetCell(0);
                        lValueTaxType = lCellTaxType.Value.ToString().Trim();

                        Cell lCellTaxItem = rowPair.Value.GetCell(1);
                        if (lCellTaxItem.Value != null)
                        {
                            lValueTaxItem = lCellTaxItem.Value.ToString().Trim();
                        }

                        Cell lCellTaxRate = rowPair.Value.GetCell(4);
                        lValueTaxRate = lCellTaxRate.Value.ToString().Trim();

                        Cell lCellTax = rowPair.Value.GetCell(5);
                        lValueTax = lCellTax.Value.ToString().Trim();
                    }
                    catch(Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 出错！");
                        lNotifyWindow.Show();
                        return false;
                    }

                    bool lHasCorrectTaxType = false;
                    if (lValueTaxType != string.Empty)
                    {
                        if (lValueTaxType == "增值税")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate);
                                StandBookEntity.HasAddValueTax = true;
                                StandBookEntity.AddValueTax = lTax;
                                StandBookEntity.AddValueTaxRate = lTaxRate;
                                StandBookEntity.AddValueTaxItem = lValueTaxItem;
                            }
                            catch (Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '营业税' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (lValueTaxType == "营业税")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate);
                                StandBookEntity.HasBusinessTax = true;
                                StandBookEntity.BusinessTax = lTax;
                                StandBookEntity.BusinessTaxRate = lTaxRate;
                                StandBookEntity.BusinessTaxItem = lValueTaxItem;
                            }
                            catch(Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '营业税' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (lValueTaxType == "教育费附加")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate);
                                StandBookEntity.HasEducationalSurtax = true;
                                StandBookEntity.EducationalSurtaxTax = lTax;
                                StandBookEntity.EducationalSurtaxTaxRate = lTaxRate;
                                StandBookEntity.EducationalSurtaxTaxItem = lValueTaxItem;
                            }
                            catch(Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '教育费附加' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (lValueTaxType == "城建税")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate);
                                StandBookEntity.HasUrbanTax = true;
                                StandBookEntity.UrbanTax = lTax;
                                StandBookEntity.UrbanTaxRate = lTaxRate;
                                StandBookEntity.UrbanTaxItem = lValueTaxItem;
                            }
                            catch(Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '城建税' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (lValueTaxType == "地方教育附加")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate); 
                                StandBookEntity.HasLocalEducationalSurtax = true;
                                StandBookEntity.LocalEducationalSurtaxTax = lTax;
                                StandBookEntity.LocalEducationalSurtaxTaxRate = lTaxRate;
                                StandBookEntity.LocalEducationalSurtaxTaxItem = lValueTaxItem;
                            }
                            catch(Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '城建税' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (lValueTaxType == "印花税")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate);
                                StandBookEntity.HasStampTax = true;
                                StandBookEntity.StampTax = lTax;
                                StandBookEntity.StampTaxRate = lTaxRate;
                                StandBookEntity.StampTaxItem = lValueTaxItem;
                            }
                            catch(Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '印花税' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (lValueTaxType == "企业所得税" || lValueTaxType == "个人所得税")
                        {
                            lHasCorrectTaxType = true;
                            try
                            {
                                decimal lTax = Convert.ToDecimal(lValueTax);
                                decimal lTaxRate = Convert.ToDecimal(lValueTaxRate);
                                StandBookEntity.HasIncomeTax = true;
                                StandBookEntity.IncomeTax = lTax;
                                StandBookEntity.IncomeTaxRate = lTaxRate;
                                StandBookEntity.IncomeTaxItem = lValueTaxItem;
                            }
                            catch(Exception e)
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 '企业所得税(个人所得税)' 出错！");
                                lNotifyWindow.Show();
                                return false;
                            }
                        }

                        if (!lHasCorrectTaxType)
                        {
                            NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第" + lKeyRow + "行 应納税种 不符合规范！");
                            lNotifyWindow.Show();
                            return false;
                        }
                    }
                }

                if (rowPair.Key == 32) // 第三十三行 备注
                {
                    try
                    {
                        Cell lCellNote = rowPair.Value.GetCell(1);
                        if (lCellNote.Value != null)
                        {
                            string lValueCellNote = lCellNote.Value.ToString().Trim();
                            StandBookEntity.Note = lValueCellNote;
                        }
                    }
                    catch (Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第三十三行 '备注' 出错！");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 33) // 第三十四行 完税凭证字别 完税证号码
                {
                    try
                    {
                        //Cell lCellFinishID = rowPair.Value.GetCell(1);
                        //string lValueFinishID = lCellFinishID.Value.ToString().Trim();

                        Cell lCellFinishNumber = rowPair.Value.GetCell(4);
                        if (lCellFinishNumber.Value != null)
                        {
                            string lValueFinishNumber = lCellFinishNumber.Value.ToString().Trim();
                            StandBookEntity.TaxReceiptNumber = lValueFinishNumber;
                        }
                    }
                    catch (Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第三十四行 '完税凭证字别 完税证号码' 出错！");
                        lNotifyWindow.Show();
                        return false;
                    }
                }

                if (rowPair.Key == 34) // 第三十五行 完税凭证字别 发票号码
                {
                    try
                    {
                        //Cell lCellFinishID = rowPair.Value.GetCell(1);
                        //string lValueFinishID = lCellFinishID.Value.ToString().Trim();

                        Cell lCellInvoiceNumber = rowPair.Value.GetCell(4);
                        if (lCellInvoiceNumber.Value != null)
                        {
                            string lValueInvoiceNumber = lCellInvoiceNumber.Value.ToString().Trim();
                            StandBookEntity.InvoiceNumber = lValueInvoiceNumber;
                        }
                    }
                    catch (Exception e)
                    {
                        NotifyWindow lNotifyWindow = new NotifyWindow("读取错误", "读取第三十五行 '完税凭证字别 发票号码' 出错！");
                        lNotifyWindow.Show();
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckVaild()
        {
            bool isSuccess = true;

            // check the number
            string lStandBookID = StandBookEntity.StandBookID;
            StandBookEntity lStandBookEntity;
            if (StandBookEntityDirectory.TryGetValue(lStandBookID, out lStandBookEntity))
            {
                StandBookEntity.ErrorText += "此受理流水号已经导入，请检查是否受理流水号重复！";
                isSuccess = false;
            }

            // check the project
            TaxPayerEntity lCurrentTaxPayerEntity = null;
            string lProjectName = StandBookEntity.ProjectName;
            {
                bool lHasProject = false;
                
                foreach (TaxPayerEntity lTaxPayerEntity in  TaxPayerEntityDirectory.Values)
                {
                    if (lTaxPayerEntity.TaxPayerProject == lProjectName)
                    {
                        lHasProject = true;
                        lCurrentTaxPayerEntity = lTaxPayerEntity;
                        currentTaxPayerEntity = lCurrentTaxPayerEntity;
                    }
                    /*
                    string lTempProjectName = lTaxPayerEntity.TaxPayerProject;

                    byte[] byteArray1 = System.Text.Encoding.UTF8.GetBytes(lTempProjectName);
                    byte[] temp1 = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.BigEndianUnicode, byteArray1);
                    byte[] byteArray2 = System.Text.Encoding.UTF8.GetBytes(lProjectName);
                    byte[] temp2 = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.BigEndianUnicode, byteArray2);

                    if (temp1.Length != temp2.Length)
                    {
                        continue;
                    }

                    bool isEqu = true;
                    for (int i = 2; i < temp1.Length; i++)
                    {
                        if (temp1[i] != temp2[i])
                        {
                            isEqu = false;
                            break;
                        }
                    }

                    if (isEqu)
                    {
                        lHasProject = true;
                        lCurrentTaxPayerEntity = lTaxPayerEntity;
                        currentTaxPayerEntity = lCurrentTaxPayerEntity;
                        break;
                    }
                    */
                }

                if (!lHasProject)
                {
                    StandBookEntity.ErrorText += "该项目在系统纳税人信息中不存在，请先添加纳税人信息！";
                    isSuccess = false;
                }
                else
                {
                    if (lCurrentTaxPayerEntity != null && lCurrentTaxPayerEntity.TaxPayerName != StandBookEntity.TaxPayerName)
                    {
                        StandBookEntity.ErrorText += "该项目在系统纳税人信息, 项目与纳税人信息不匹配，请检查！";
                        isSuccess = false;
                    }
                }
            }

            // Check Old StandBook Info
            {
                foreach (StandBookEntity lStandBookEntityEm in StandBookEntityDirectory.Values)
                {
                    if (lStandBookEntityEm.ProjectName == StandBookEntity.ProjectName)
                    {
                        decimal lOldTotalMoney = lStandBookEntityEm.TotalMoney.GetValueOrDefault(0);
                        decimal lNewTotalMoney = StandBookEntity.TotalMoney.GetValueOrDefault(0);

                        if (lOldTotalMoney != lNewTotalMoney)
                        {
                            StandBookEntity.ErrorText += "该项目与以前导入金额不一致，请检查！";
                            isSuccess = false;
                        }
                    }
                }
            }

            if (isSuccess)
            {
                StandBookEntity.SuccessText = "可以导入！";
                StandBookEntity.ShowError = Visibility.Collapsed;
                StandBookEntity.ShowSuccess = Visibility.Visible;
            }
            else
            {
                StandBookEntity.ShowError = Visibility.Visible;
                StandBookEntity.ShowSuccess = Visibility.Collapsed;
            }

            return isSuccess;
        }
    }
}
