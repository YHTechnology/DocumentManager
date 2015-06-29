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
using Microsoft.Windows.Data.DomainServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DocumentManager.Model.Entities;
using System.ServiceModel.DomainServices.Client;

namespace DocumentManager.ViewModels
{
    public class HomeViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;
        private DomainCollectionView<DocumentManager.Web.Model.taxpayer> taxPayerView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.taxpayer> taxPayerLoader;
        private EntityList<DocumentManager.Web.Model.taxpayer> taxPayerSource;

        public ObservableCollection<TaxPayerCalEntity> TaxPayerTotal { get; set; }

        public ObservableCollection<TaxPayerCalEntity> TaxPayerZZTax { get; set; }

        public ObservableCollection<TaxPayerCalEntity> TaxPayerYYTax { get; set; }

        public string DocumentTotal { get; set; }
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { if (isBusy != value) { isBusy = value; UpdateChanged("IsBusy"); } }
        }

        private Dictionary<string, TaxPayerEntity> JATaxPayerEntity;
        private Dictionary<string, TaxPayerEntity> ZPTaxPayerEntity;
        private Dictionary<string, TaxPayerEntity> PPTaxPayerEntity;
        private Dictionary<string, TaxPayerEntity> GDTaxPayerEntity;

        public HomeViewModel()
        {
            documentManagerContext = new DocumentManager.Web.DocumentManagerDomainContext();
            TaxPayerTotal = new ObservableCollection<TaxPayerCalEntity>();
            TaxPayerZZTax = new ObservableCollection<TaxPayerCalEntity>();
            TaxPayerYYTax = new ObservableCollection<TaxPayerCalEntity>();

            JATaxPayerEntity = new Dictionary<string, TaxPayerEntity>();
            ZPTaxPayerEntity = new Dictionary<string, TaxPayerEntity>();
            PPTaxPayerEntity = new Dictionary<string, TaxPayerEntity>();
            GDTaxPayerEntity = new Dictionary<string, TaxPayerEntity>();
        }

        public void LoadData()
        {
            //documentManagerContext
            IsBusy = true;
            InvokeOperation<int> lDocumentTotal = documentManagerContext.GetTaxpayerDocumentCount();
            lDocumentTotal.Completed += DocumentTotal_Completed;
        }

        private void DocumentTotal_Completed(object sender, EventArgs e)
        {
            IsBusy = false;
            var lValue = (System.ServiceModel.DomainServices.Client.InvokeOperation<int>)sender;

            DocumentTotal = "档案总数 ： " + lValue.Value.ToString();

            UpdateChanged("DocumentTotal");

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
            EntityQuery<DocumentManager.Web.Model.taxpayer> lQuery = documentManagerContext.GetTaxpayerQuery();
            return documentManagerContext.Load(lQuery.SortAndPageBy(this.taxPayerView));
        }

        private void loadOperation_Completed(LoadOperation<DocumentManager.Web.Model.taxpayer> sender)
        {
            IsBusy = false;
            taxPayerSource.Source = sender.Entities;
            int lTotal = 0;
            int lJATotal = 0;
            int lPPTotal = 0;
            int lZPTotal = 0;
            int lGDTotal = 0;

            foreach (DocumentManager.Web.Model.taxpayer taxpayer in sender.Entities)
            {
                TaxPayerEntity taxPayerEntity = new TaxPayerEntity();
                taxPayerEntity.TaxPayer = taxpayer;
                taxPayerEntity.Update();

                switch (taxPayerEntity.TaxPayerGroupId.GetValueOrDefault(-1))
                {
                    case 0:
                        {
                            TaxPayerEntity taxPayerEntitytemp;
                            if (!GDTaxPayerEntity.TryGetValue(taxPayerEntity.TaxPayerName, out taxPayerEntitytemp))
                            {
                                GDTaxPayerEntity.Add(taxPayerEntity.TaxPayerName, taxPayerEntity);
                            }
                        }
                        break;
                    case 1:
                        {
                            TaxPayerEntity taxPayerEntitytemp;
                            if(!JATaxPayerEntity.TryGetValue(taxPayerEntity.TaxPayerName, out taxPayerEntitytemp))
                            {
                                JATaxPayerEntity.Add(taxPayerEntity.TaxPayerName, taxPayerEntity);
                            }
                        }
                        break;
                    case 2:
                        {
                            TaxPayerEntity taxPayerEntitytemp;
                            if (!PPTaxPayerEntity.TryGetValue(taxPayerEntity.TaxPayerName, out taxPayerEntitytemp))
                            {
                                PPTaxPayerEntity.Add(taxPayerEntity.TaxPayerName, taxPayerEntity);
                            }
                        }
                        break;
                    case 3:
                        {
                            TaxPayerEntity taxPayerEntitytemp;
                            if (!ZPTaxPayerEntity.TryGetValue(taxPayerEntity.TaxPayerName, out taxPayerEntitytemp))
                            {
                                ZPTaxPayerEntity.Add(taxPayerEntity.TaxPayerName, taxPayerEntity);
                            }
                        }
                        break;
                }
            }

            DocumentTotal = "总户数 ： " + (ZPTaxPayerEntity.Count + GDTaxPayerEntity.Count + JATaxPayerEntity.Count + PPTaxPayerEntity.Count).ToString();

            TaxPayerCalEntity lJATotalEntity = new TaxPayerCalEntity();
            lJATotalEntity.Key = "建安代开";
            lJATotalEntity.Value = JATaxPayerEntity.Count;
            TaxPayerTotal.Add(lJATotalEntity);
            TaxPayerCalEntity lPPTotalEntity = new TaxPayerCalEntity();
            lPPTotalEntity.Key = "普票代开";
            lPPTotalEntity.Value = PPTaxPayerEntity.Count;
            TaxPayerTotal.Add(lPPTotalEntity);
            TaxPayerCalEntity lZPTotalEntity = new TaxPayerCalEntity();
            lZPTotalEntity.Key = "专票代开";
            lZPTotalEntity.Value = ZPTaxPayerEntity.Count;
            TaxPayerTotal.Add(lZPTotalEntity);
            TaxPayerCalEntity lGDTotalEntity = new TaxPayerCalEntity();
            lGDTotalEntity.Key = "固定户";
            lGDTotalEntity.Value = GDTaxPayerEntity.Count;
            TaxPayerTotal.Add(lGDTotalEntity);

            int lZZFree = 0;
            int lZZOther = 0;
            int lYYFree = 0;
            int lYYOther = 0;

            foreach (TaxPayerEntity lTaxPayerEntity in GDTaxPayerEntity.Values)
            {
                if ("增值税纳税户" == lTaxPayerEntity.TaxPayerProject)
                {
                    if (lTaxPayerEntity.TaxPayerIsFree.HasValue && lTaxPayerEntity.TaxPayerIsFree.Value)
                    {
                        lZZFree++;
                    }
                    else
                    {
                        lZZOther++;
                    }
                }

                if ("营业税纳税户" == lTaxPayerEntity.TaxPayerProject)
                {
                    if (lTaxPayerEntity.TaxPayerIsFree.HasValue && lTaxPayerEntity.TaxPayerIsFree.Value)
                    {
                        lYYFree++;
                    }
                    else
                    {
                        lYYOther++;
                    }
                }
            }

            TaxPayerCalEntity lZZFreeTotalEntity = new TaxPayerCalEntity();
            lZZFreeTotalEntity.Key = "增值免税户";
            lZZFreeTotalEntity.Value = lZZFree;
            TaxPayerZZTax.Add(lZZFreeTotalEntity);
            TaxPayerCalEntity lZZOtherTotalEntity = new TaxPayerCalEntity();
            lZZOtherTotalEntity.Key = "增值纳税户";
            lZZOtherTotalEntity.Value = lZZOther;
            TaxPayerZZTax.Add(lPPTotalEntity);

            TaxPayerCalEntity lYYFreeTotalEntity = new TaxPayerCalEntity();
            lYYFreeTotalEntity.Key = "营业免税户";
            lYYFreeTotalEntity.Value = lYYFree;
            TaxPayerYYTax.Add(lYYFreeTotalEntity);
            TaxPayerCalEntity lYYOtherTotalEntity = new TaxPayerCalEntity();
            lYYOtherTotalEntity.Key = "营业纳税户";
            lYYOtherTotalEntity.Value = lYYOther;
            TaxPayerYYTax.Add(lYYOtherTotalEntity);





            UpdateChanged("TaxPayerTotal");
            UpdateChanged("DocumentTotal");
            UpdateChanged("TaxPayerZZTax");
            UpdateChanged("TaxPayerYYTax");
            IsBusy = false;
        }                  
    }                      
}
