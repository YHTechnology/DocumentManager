
namespace DocumentManager.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using DocumentManager.Web.Model;


    // Implements application logic using the documentmanagerEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class DocumentManagerDomainService : LinqToEntitiesDomainService<documentmanagerEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'action' query.
        [Query]
        public IQueryable<action> GetAction()
        {
            return this.ObjectContext.action;
        }

        public void InsertAction(action action)
        {
            if ((action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(action, EntityState.Added);
            }
            else
            {
                this.ObjectContext.action.AddObject(action);
            }
        }

        public void UpdateAction(action currentaction)
        {
            this.ObjectContext.action.AttachAsModified(currentaction, this.ChangeSet.GetOriginal(currentaction));
        }

        public void DeleteAction(action action)
        {
            if ((action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(action, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.action.Attach(action);
                this.ObjectContext.action.DeleteObject(action);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'filetype' query.
        public IQueryable<filetype> GetFiletype()
        {
            return this.ObjectContext.filetype;
        }

        public void InsertFiletype(filetype filetype)
        {
            if ((filetype.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(filetype, EntityState.Added);
            }
            else
            {
                this.ObjectContext.filetype.AddObject(filetype);
            }
        }

        public void UpdateFiletype(filetype currentfiletype)
        {
            this.ObjectContext.filetype.AttachAsModified(currentfiletype, this.ChangeSet.GetOriginal(currentfiletype));
        }

        public void DeleteFiletype(filetype filetype)
        {
            if ((filetype.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(filetype, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.filetype.Attach(filetype);
                this.ObjectContext.filetype.DeleteObject(filetype);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'systemlog' query.
        public IQueryable<systemlog> GetSystemlog()
        {
            return this.ObjectContext.systemlog;
        }

        public void InsertSystemlog(systemlog systemlog)
        {
            if ((systemlog.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(systemlog, EntityState.Added);
            }
            else
            {
                this.ObjectContext.systemlog.AddObject(systemlog);
            }
        }

        public void UpdateSystemlog(systemlog currentsystemlog)
        {
            this.ObjectContext.systemlog.AttachAsModified(currentsystemlog, this.ChangeSet.GetOriginal(currentsystemlog));
        }

        public void DeleteSystemlog(systemlog systemlog)
        {
            if ((systemlog.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(systemlog, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.systemlog.Attach(systemlog);
                this.ObjectContext.systemlog.DeleteObject(systemlog);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'taxpayer' query.
        public IQueryable<taxpayer> GetTaxpayer()
        {
            return this.ObjectContext.taxpayer;
        }

        [Invoke]
        public int GetTaxpayerGropCount(int aGropID)
        {
            return this.ObjectContext.taxpayer.Where(r => r.taxpayer_group_id == aGropID).Count();
        }
        
        public void InsertTaxpayer(taxpayer taxpayer)
        {
            if ((taxpayer.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taxpayer, EntityState.Added);
            }
            else
            {
                this.ObjectContext.taxpayer.AddObject(taxpayer);
            }
        }

        public void UpdateTaxpayer(taxpayer currenttaxpayer)
        {
            this.ObjectContext.taxpayer.AttachAsModified(currenttaxpayer, this.ChangeSet.GetOriginal(currenttaxpayer));
        }

        public void DeleteTaxpayer(taxpayer taxpayer)
        {
            if ((taxpayer.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taxpayer, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.taxpayer.Attach(taxpayer);
                this.ObjectContext.taxpayer.DeleteObject(taxpayer);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'taxpayerdocument' query.
        public IQueryable<taxpayerdocument> GetTaxpayerdocument()
        {
            return this.ObjectContext.taxpayerdocument;
        }

        [Query]
        public IQueryable<taxpayerdocument> GetTaxpayerdocumentByArray(int groupid , string[] taxPayer)
        {
            string lTaxPayerCode = taxPayer[0] !=null ? taxPayer[0] : "";
            string lTaxPayerName = taxPayer[1] != null ? taxPayer[1] : "";
            string lTaxPayerRegyear = taxPayer[2] != null ? taxPayer[2] : "";
            string lTaxPayerProject = taxPayer[3] != null ? taxPayer[3] : "";

            var lRet = from c in this.ObjectContext.taxpayerdocument
                       from d in this.ObjectContext.taxpayer
                       where d.taxpayer_id == c.taxpayer_id
                             && d.taxpayer_group_id == groupid
                             && d.taxpayer_code.Contains(lTaxPayerCode)
                             && d.taxpayer_name.Contains(lTaxPayerName)
                             && d.taxpayer_project.Contains(lTaxPayerProject)
                             && d.taxpayer_regyear.Contains(lTaxPayerRegyear)
                       select c;
            return lRet;
        }

        [Query]
        public IQueryable<taxpayerdocument> GetTaxpayerdocumentGDByArray(int groupid, string[] taxPayer)
        {
            string lTaxPayerCode = taxPayer[0] != null ? taxPayer[0] : "";
            string lTaxPayerName = taxPayer[1] != null ? taxPayer[1] : "";
            string lTaxPayerRegyear = taxPayer[2] != null ? taxPayer[2] : "";
            string lTaxPayerProject = taxPayer[3] != null ? taxPayer[3] : "";

            var lRet = from c in this.ObjectContext.taxpayerdocument
                       from d in this.ObjectContext.taxpayer
                       where d.taxpayer_id == c.taxpayer_id
                             && d.taxpayer_group_id == groupid
                             && d.taxpayer_code.Contains(lTaxPayerCode)
                             && d.taxpayer_name.Contains(lTaxPayerName)
                             && d.taxpayer_project.Contains(lTaxPayerProject)
                       select c;
            return lRet;
        }

        [Invoke]
        public int GetTaxpayerDocumentCount()
        {
            return this.ObjectContext.taxpayerdocument.Count();
        }

        public void InsertTaxpayerdocument(taxpayerdocument taxpayerdocument)
        {
            if ((taxpayerdocument.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taxpayerdocument, EntityState.Added);
            }
            else
            {
                this.ObjectContext.taxpayerdocument.AddObject(taxpayerdocument);
            }
        }

        public void UpdateTaxpayerdocument(taxpayerdocument currenttaxpayerdocument)
        {
            this.ObjectContext.taxpayerdocument.AttachAsModified(currenttaxpayerdocument, this.ChangeSet.GetOriginal(currenttaxpayerdocument));
        }

        public void DeleteTaxpayerdocument(taxpayerdocument taxpayerdocument)
        {
            if ((taxpayerdocument.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taxpayerdocument, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.taxpayerdocument.Attach(taxpayerdocument);
                this.ObjectContext.taxpayerdocument.DeleteObject(taxpayerdocument);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'taxpayertype' query.
        public IQueryable<taxpayertype> GetTaxpayertype()
        {
            return this.ObjectContext.taxpayertype;
        }

        public void InsertTaxpayertype(taxpayertype taxpayertype)
        {
            if ((taxpayertype.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taxpayertype, EntityState.Added);
            }
            else
            {
                this.ObjectContext.taxpayertype.AddObject(taxpayertype);
            }
        }

        public void UpdateTaxpayertype(taxpayertype currenttaxpayertype)
        {
            this.ObjectContext.taxpayertype.AttachAsModified(currenttaxpayertype, this.ChangeSet.GetOriginal(currenttaxpayertype));
        }

        public void DeleteTaxpayertype(taxpayertype taxpayertype)
        {
            if ((taxpayertype.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taxpayertype, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.taxpayertype.Attach(taxpayertype);
                this.ObjectContext.taxpayertype.DeleteObject(taxpayertype);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'user' query.
        public IQueryable<user> GetUser()
        {
            return this.ObjectContext.user;
        }

        public void InsertUser(user user)
        {
            if ((user.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user, EntityState.Added);
            }
            else
            {
                this.ObjectContext.user.AddObject(user);
            }
        }

        public void UpdateUser(user currentuser)
        {
            this.ObjectContext.user.AttachAsModified(currentuser, this.ChangeSet.GetOriginal(currentuser));
        }

        public void DeleteUser(user user)
        {
            if ((user.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.user.Attach(user);
                this.ObjectContext.user.DeleteObject(user);
            }
        }



        public IQueryable<useraction> GetUserAction()
        {
            return this.ObjectContext.useraction;
        }

        public void InsertUserAction(useraction useraction)
        {
            if ((useraction.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(useraction, EntityState.Added);
            }
            else
            {
                this.ObjectContext.useraction.AddObject(useraction);
            }
        }

        public void UpdateUserAction(useraction currentuseraction)
        {
            this.ObjectContext.useraction.AttachAsModified(currentuseraction, this.ChangeSet.GetOriginal(currentuseraction));
        }

        public void DeleteUserAction(useraction useraction)
        {
            if ((useraction.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(useraction, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.useraction.Attach(useraction);
                this.ObjectContext.useraction.DeleteObject(useraction);
            }
        }

        [Query]
        public IQueryable<useraction> GetUserActionByUserID(int aUserID)
        {
            return this.ObjectContext.useraction.Where(c => c.user_id == aUserID);
        }

        [Query]
        public IQueryable<standbook> GetStandBook()
        {
            return this.ObjectContext.standbook;
        }

        public void InsertStandBook(standbook standbook)
        {
            if ((standbook.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(standbook, EntityState.Added);
            }
            else
            {
                this.ObjectContext.standbook.AddObject(standbook);
            }
        }

        public void UpdateStandBook(standbook currentstandbook)
        {
            this.ObjectContext.standbook.AttachAsModified(currentstandbook, this.ChangeSet.GetOriginal(currentstandbook));
        }

        public void DeleteStandBook(standbook standbook)
        {
            if ((standbook.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(standbook, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.standbook.Attach(standbook);
                this.ObjectContext.standbook.DeleteObject(standbook);
            }
        }
    }
}


