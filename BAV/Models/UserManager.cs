using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BAV.Models
{
    public class UserManager
    {
        public UsersContext db = new UsersContext();
        public string GetUserPassword(string loginName)
        {

            var user = db.User.Where(o => o.UserName.ToLower().Equals(loginName)).Select(x=>x.Password).SingleOrDefault();
            if (user.Any())
                //return user.FirstOrDefault().PasswordEncryptedText;

                return user.ToString();
            else
                return string.Empty;
          //
        }
        public bool IsUserInRole(string loginName, string roleName) {  
                  User  userId = db.User.Where(o => o.UserName.ToLower().Equals(loginName)).SingleOrDefault();

                  if (userId != null)
                  {
                    var roles = from q in db.UserRole
                                join r in db.Role on q.roleid equals r.Id
                                where q.UserId.Equals(userId.Id) && r.RoleName.Equals(roleName)
                                select q.roleid;

                    if (roles != null)
                    {
                        return roles.Any();
                    }
                }

                return false;
            }
      }
    
}