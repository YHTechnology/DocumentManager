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
using System.ServiceModel.DomainServices.Client;
using System.Collections.ObjectModel;
using DocumentManager.Model.Entities;
using DocumentManager.Controls;

namespace DocumentManager.ViewModels
{

    public class StandBookViewModel : NotifyPropertyChanged
    {
        private DocumentManager.Web.DocumentManagerDomainContext documentManagerContext;

        private DomainCollectionView<DocumentManager.Web.Model.standbook> standbookView;
        private DomainCollectionViewLoader<DocumentManager.Web.Model.standbook> standbookLoader;
        private EntityList<DocumentManager.Web.Model.standbook> standbookSource;

        public delegate void BeginLoading(object sender, EventArgs e);
        public event BeginLoading BeginLoadings;

        public delegate void FinishLoading(object sender, EventArgs e);
        public event FinishLoading FinishLoadings;

        public ObservableCollection<StandBookEntity> StandBookEntityList { get; set; }

        private TaxPayerEntity taxPayerEntity;
        public TaxPayerEntity TaxPayerEntity
        {
            set
            {
                innerStandBookEntity = null;
                if (taxPayerEntity != value)
                {
                    taxPayerEntity = value;
                    //if (taxPayerEntity != null)
                    {
                        using (standbookView.DeferRefresh())
                        {
                            standbookView.MoveToFirstPage();
                        }
                    }
                }
            }
        }

        private StandBookEntity innerStandBookEntity;
        public StandBookEntity InnerStandBookEntity
        {
            set
            {
                taxPayerEntity = null;
                if (innerStandBookEntity != value)
                {
                    innerStandBookEntity = value;
                    //if (taxPayerEntity != null)
                    {
                        using (standbookView.DeferRefresh())
                        {
                            standbookView.MoveToFirstPage();
                        }
                    }
                }
            }
        }

        public StandBookEntity FirstStandBookEntity { get; set; }

        public StandBookJAViewModel StandBookJAViewModel { get; set; }

        public StandBookViewModel()
        {
            documentManagerContext = new Web.DocumentManagerDomainContext();

            StandBookEntityList = new ObservableCollection<StandBookEntity>();

            standbookSource = new EntityList<Web.Model.standbook>(documentManagerContext.standbooks);
            standbookLoader = new DomainCollectionViewLoader<Web.Model.standbook>(
                LoadStandBook,
                LoadStandBook_Complete
                );
            standbookView = new DomainCollectionView<Web.Model.standbook>(standbookLoader, standbookSource);
        }

        private LoadOperation<Web.Model.standbook> LoadStandBook()
        {
            BeginLoadings(null, null);
            EntityQuery<Web.Model.standbook> lQuery = documentManagerContext.GetStandBookQuery();
            if (taxPayerEntity != null)
            {
                lQuery = lQuery.Where(c => c.projectname == taxPayerEntity.TaxPayerProject).OrderByDescending(c => c.paytime);
            }
            else if (innerStandBookEntity != null)
            {
                lQuery = lQuery.Where(c => c.projectname == innerStandBookEntity.ProjectName).OrderByDescending(c => c.paytime);
            }
            else
            {
                lQuery = lQuery.Where(c => c.projectname == "-1").OrderByDescending(c => c.paytime);
            }
            return documentManagerContext.Load(lQuery.SortAndPageBy(standbookView));
        }

        private void LoadStandBook_Complete(LoadOperation<Web.Model.standbook> sender)
        {
            StandBookEntityList.Clear();
            standbookSource.Source = sender.Entities;
            foreach (DocumentManager.Web.Model.standbook standbook in sender.Entities)
            {
                StandBookEntity standBookEntity = new StandBookEntity();
                standBookEntity.StandBook = standbook;
                standBookEntity.Update();
                StandBookEntityList.Add(standBookEntity);
            }

            if(StandBookEntityList.Count > 0)
            {
                FirstStandBookEntity = StandBookEntityList[0];
            }
            else
            {
                FirstStandBookEntity = null;
            }
            UpdateChanged("FirstStandBookEntity");
            UpdateChanged("StandBookEntityList");
            FinishLoadings(null, null);
        }

        public void DeleteStandBook(DocumentManager.Web.Model.standbook aStandBook)
        {
            documentManagerContext.standbooks.Remove(aStandBook);
            SubmitOperation lSubmitOperation = documentManagerContext.SubmitChanges();
            lSubmitOperation.Completed += SubOperation_Completed;
        }

        private void SubOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "删除失败 " + submitOperation.Error);
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("删除成功", "删除成功！");
                notifyWindow.Show();
            }

            StandBookJAViewModel.LoadData();

            //using (standbookView.DeferRefresh())
            //{
            //    standbookView.MoveToFirstPage();
            //}
        }
    }
}
