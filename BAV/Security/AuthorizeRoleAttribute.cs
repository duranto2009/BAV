using BAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BAV.Security
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {

        private readonly string[] userAssignedRoles;
        public AuthorizeRolesAttribute(params string[] roles) {
            this.userAssignedRoles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            bool authorize = false;
          
                UserManager UM = new UserManager();
                foreach (var roles in userAssignedRoles) {
                    authorize = UM.IsUserInRole(httpContext.User.Identity.Name, roles);
                    if (authorize)
                        return authorize;
                }
            
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            filterContext.Result = new RedirectResult("~/Home/Index");
            //  protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            //filterContext.Result = new RedirectResult("~/Home/Unauthoried access");
        }
    }
}