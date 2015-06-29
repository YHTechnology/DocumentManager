using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.Server.ApplicationServices;
using DocumentManager.Web.Model;
using System.Security.Principal;

namespace DocumentManager.Web
{
    [EnableClientAccess]
    public class DocumentManagerUserDomainService : AuthenticationBase<User>
    {
        // To enable Forms/Windows Authentication for the Web Application, edit the appropriate section of web.config file.
        protected override User GetAuthenticatedUser(IPrincipal principal)
        {
            using (documentmanagerEntities documentmanagerEntities = new documentmanagerEntities())
            {
                User user = new User();

                LocalServerService lLocalService = new LocalServerService();
                user.ExpireDay = lLocalService.GetExpireDay(); 
                try
                {
                    var result = from r in documentmanagerEntities.user where r.user_name == principal.Identity.Name select r;
                    if (result.Count() > 0)
                    {
                        DocumentManager.Web.Model.user lUser = result.First();
                        user.UserID = lUser.user_id;
                        user.Name = lUser.user_name;
                        user.Password = lUser.user_password;
                        user.UserName = lUser.user_cname;
                        user.RightDictionary = new Dictionary<int, bool>();
                        if (user.UserID != 1)
                        {
                            var resultaction = from r in documentmanagerEntities.useraction where r.user_id == lUser.user_id select r;
                            if (resultaction.Count() > 0)
                            {
                                foreach (DocumentManager.Web.Model.useraction useraction in resultaction)
                                {
                                    user.RightDictionary.Add(useraction.action_id.Value, useraction.hasRight.Value);
                                }
                            }
                        }
                        else
                        {
                            var resultaction = from r in documentmanagerEntities.action select r;
                            if (resultaction.Count() > 0)
                            {
                                foreach (DocumentManager.Web.Model.action action in resultaction)
                                {
                                    user.RightDictionary.Add(action.action_id, true);
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                }
                return user;
            }
        }
    }

    public class User : UserBase
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public bool IsFreeze { get; set; }
        public string Password { get; set; }
        public Dictionary<int, bool> RightDictionary { get; set; }

        public int ExpireDay { get; set; }
    }
}