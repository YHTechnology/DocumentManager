using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.DomainServices.Server;
using DocumentManager.Web.Model;
using System.ServiceModel.DomainServices.Hosting;

namespace DocumentManager.Web
{
    [EnableClientAccess()]
    public class OperationDomainService : DomainService
    {
        public bool ModifyPassword(int aUserID, String aNewPassword)
        {
            using (documentmanagerEntities entities = new documentmanagerEntities())
            {
                var lUserList = from r in entities.user where r.user_id == aUserID select r;
                if (lUserList.Count() > 0)
                {
                    user lUser = lUserList.First();
                    lUser.user_password = aNewPassword;
                    entities.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}