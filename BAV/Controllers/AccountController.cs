using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using BAV.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using BAV.Security;
using System.Data.Entity;
using CrystalDecisions.CrystalReports.Engine;
using System.Net;

namespace BAV.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public UsersContext db = new UsersContext();
        public CommonContext cdb = new CommonContext();
        public MemberRegistrationContext mrbd = new MemberRegistrationContext();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(User model, string returnUrl)
        {
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.UserName))
                TempData["login_error"] = "The User login or password provided is incorrect.";
            else
            {
                if (ModelState.IsValid)
                {

                    UserManager UM = new UserManager();
                    var password = db.User.Where(o => o.UserName.ToLower().Equals(model.UserName) && o.IsActive.Equals(true)).Select(x => x.Password).SingleOrDefault();
                    if (string.IsNullOrEmpty(password))
                        TempData["login_error"] = "The User login or password provided is incorrect.";

                    else
                    {
                        var uid = db.User.Where(o => o.UserName.ToLower().Equals(model.UserName) && o.IsActive.Equals(true)).Select(x=>x.Id).SingleOrDefault();
                        var cid = db.UserRole.Where(o => o.UserId.Equals(uid)).Select(x => x.DistricId).SingleOrDefault();
                        if(cid>0)
                        {
                            password = Decrypt(password);
                        }
                        else
                        {
                            password = DecryptAdmin(password);
                        }
                      
                        if (model.Password.Equals(password))
                        {

                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            var userid = db.User.Where(x => x.UserName.Equals(model.UserName)).Select(x => x.Id).SingleOrDefault();
                            var rolename = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(s => s.roleid).SingleOrDefault();
                            Session["rolename"] = rolename;
                            if (rolename == 1)
                            {
                                TempData["e"] = "true";
                                Session["rolename"] = rolename;
                                var superadmin = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.DistricId).FirstOrDefault();
                                if (superadmin > 0)
                                {
                                    Session["superadmin"] = "true";
                                    int disid = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.DistricId).SingleOrDefault();
                                    if(disid==13)
                                    {
                                        Session["disid"] = "true";
                                      
                                    }
                                    else
                                    {
                                        Session["disid"] = "false";
                                      
                                    }
                                    Session["Distrcid"] = disid; 
                                    var subadmin = db.UserPlatun.Where(x => x.UserId.Equals(userid)).Count();
                                    var admin = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
                                    if(subadmin==0 && admin==0)
                                    {
                                        return RedirectToAction("PlatunCreation", "Common");
                                    }
                                    else
                                    {
                                        Session["Ptrue"] = "True";
                                    }
                                }
                                else
                                {
                                    Session["superadmin"] = "false";
                                }
                            }
                           else if(rolename==4)
                            {
                                TempData["e"] = "true";
                                Session["superadmin"] = "false";
                            }
                           else if(rolename==3)
                            {
                                //TempData["e"] = "true";
                            }
                            else
                            {
                                
                                var subadmin = db.UserPlatun.Where(x => x.UserId.Equals(userid)).Count();
                                TempData["e"] = "false";
                                if (subadmin > 0)
                                {
                                    Session["Ptrue"] = null;
                                    Session["superadmin"] = "true";
                                    Session["rolename"] = rolename;
                                }
                                else
                                {
                                    Session["superadmin"] = "false";
                                }
                              
                            }
                            Session["usertype"] = TempData["e"];
                            return RedirectToLocal(returnUrl);
                            //return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            //ModelState.AddModelError("", "The User login or password provided is incorrect.");
                            TempData["login_error"] = "The User login or password provided is incorrect.";
                        }
                    }
                }
            }

            return View(model);
        }

         [HttpGet]
        [AllowAnonymous]
        public ActionResult Report(int id)
        {
            Response.ContentType = "application/pdf";
            if (id == 1)
            {
                Response.AppendHeader("Content-Disposition", "attachment; filename=Education form.pdf");
            }
            else if (id == 2)
            {
                Response.AppendHeader("Content-Disposition", "attachment; filename=Death form.pdf");
            }
            else if (id == 3)
            {
                Response.AppendHeader("Content-Disposition", "attachment; filename=Kaizan from.pdf");

            }
            else if (id == 4)
            {
                Response.AppendHeader("Content-Disposition", "attachment; filename= VDP From.pdf");

            }

            const int bufferLength = 10000;
            byte[] buffer = new Byte[bufferLength];
            int length = 0;
            Stream download = null;
            try
            {
                if (id == 1)
                {
                    download = new FileStream(Server.MapPath("~/Download/Education-From.pdf"), FileMode.Open, FileAccess.Read);
                }
                else if (id == 2)
                {
                    download = new FileStream(Server.MapPath("~/Download/Death-From.pdf"), FileMode.Open, FileAccess.Read);
                }
                else if (id == 3)
                {
                    download = new FileStream(Server.MapPath("~/Download/Kaizan-From.pdf"), FileMode.Open, FileAccess.Read);
                }
                else if (id == 4)
                {
                    download = new FileStream(Server.MapPath("~/Download/Toto-From-VDP.pdf"), FileMode.Open, FileAccess.Read);
                }
                do
                {
                    if (Response.IsClientConnected)
                    {
                        length = download.Read(buffer, 0, bufferLength);
                        Response.OutputStream.Write(buffer, 0, length);
                        buffer = new Byte[bufferLength];
                    }
                    else
                    {
                        length = -1;
                    }
                }
                while (length > 0);
                Response.Flush();
                Response.End();
            }
            finally
            {
                if (download != null)
                    download.Close();
            }
            return RedirectToAction("Login");
    
        }
        //
        // POST: /Account/LogOff
        [SessionExpire]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
           
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            FormsAuthentication.SignOut();
           
            return RedirectToAction("Login", "Account");
        }
        [AllowAnonymous]
        public ActionResult MemberSearch(string returnUrl)
        
        {
           
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MemberSearch(Member model, string returnUrl)
        {  
            var s=mrbd.Member.Where(x=>x.IDCardNo.Equals(model.IDCardNo)).Count();
            if (s > 0 && model.IDCardNo!=null)
            {
                return RedirectToAction("Memberdetail", new { id = model.IDCardNo });
            }
            else
            {
                TempData["error"]= "He/She is not Member.";
               
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
        public  async Task<ActionResult> Memberdetail(string id)
        {
            PersonalInfoMemberAddressBodyStructureReligion model = new PersonalInfoMemberAddressBodyStructureReligion();
            var memberid = mrbd.Member.Where(x => x.IDCardNo.Equals(id.Trim())).Select(x => x.MemberId).FirstOrDefault();
            var platunlist = db.Platun.ToList();
            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join st in platunlist.ToList() on pi.platunId equals st.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                join i in mrbd.Image.ToList() on pi.Id equals i.MemberId
                                where pi.Id.Equals(memberid)
                                select new PersonalInfoMemberAddressBodyStructure
                                {
                                    pi = pi,
                                    m = m,
                                    a = a,
                                    bs = bs,
                                    i = i,
                                    planame=st.PlatuneName

                                };

            var s = volantierlist.Select(x => x.bs.BloodGroupId).SingleOrDefault();
            if (s == 0)
            {
                using (cdb = new CommonContext())
                {

                    var volantier = from vl in volantierlist.ToList()
                                    join r in cdb.Religion on vl.pi.ReligionId equals r.Id
                                  
                                    join pt in cdb.SubDistric on vl.a.PresSubDistric equals pt.Id
                                    join pd in cdb.Distric on vl.a.PresDistric equals pd.Id
                                 
                                    join hi in cdb.Inch on vl.bs.inchid equals hi.Id
                                    join hf in cdb.Foot on vl.bs.footid equals hf.Id
                                    join g in cdb.Gender on vl.bs.GenderId equals g.Id
                                    join ec in cdb.EyeColor on vl.bs.EyeColorId equals ec.Id

                                    select new PersonalInfoMemberAddressBodyStructureReligion
                                    {
                                        Id = vl.pi.Id,
                                        Imge = vl.i.picture,
                                        platunname=vl.planame,
                                        bname = vl.pi.BanglaName,
                                        ename = vl.pi.EnglishName,
                                        bfname = vl.pi.BanglaFatherName,
                                        efname = vl.pi.EnglishFatherName,
                                        bmname = vl.pi.BanglaMotherName,
                                        emname = vl.pi.EnglishMotherName,

                                        dob = vl.pi.DOB,
                                        maritalstatus = vl.pi.MaritalStatus,
                                        smname = vl.pi.WORHName,
                                        smpasa = vl.pi.WorHOccupation,
                                        nid = vl.pi.NID,
                                        dobno = vl.pi.DOBSNo,


                                        occupation = vl.pi.occupation,
                                        religion = r.Name,


                                        presaddress = vl.a.PresAddress,
                                        prespostcode = vl.a.PresPostCodeId,
                                        presunion = vl.a.PresUnion,
                                        presthana = pt.Name,
                                        presdist = pd.Name,

                                        height = hf.Name + " " + hi.Name,
                                        weight = vl.bs.WeightKg + " কেজি",
                                        zender = g.Name,
                                        cyecolor = ec.Name,
                                        bodycolor = vl.bs.bodycolor,
                                        bloodgroup = "",
                                        sanaktakarn = vl.bs.SanaktakaranSign,


                                        statusid = vl.m.Status,
                                        membercode = vl.m.IDCardNo,
                                        mobile = vl.pi.mobile

                                    };
                    model = volantier.SingleOrDefault();
                }
            }
            else
            {
                using (cdb = new CommonContext())
                {

                    var volantier = from vl in volantierlist.ToList()
                                    join r in cdb.Religion on vl.pi.ReligionId equals r.Id
                                  
                                    join pt in cdb.SubDistric on vl.a.PresSubDistric equals pt.Id
                                    join pd in cdb.Distric on vl.a.PresDistric equals pd.Id
                              
                                    join hi in cdb.Inch on vl.bs.inchid equals hi.Id
                                    join hf in cdb.Foot on vl.bs.footid equals hf.Id
                                    join g in cdb.Gender on vl.bs.GenderId equals g.Id
                                    join ec in cdb.EyeColor on vl.bs.EyeColorId equals ec.Id
                                    join bg in cdb.BloodGroup on vl.bs.BloodGroupId equals bg.Id
                                    select new PersonalInfoMemberAddressBodyStructureReligion
                                    {
                                        Id = vl.pi.Id,
                                        Imge = vl.i.picture,
                                        platunname = vl.planame,
                                        bname = vl.pi.BanglaName,
                                        ename = vl.pi.EnglishName,
                                        bfname = vl.pi.BanglaFatherName,
                                        efname = vl.pi.EnglishFatherName,
                                        bmname = vl.pi.BanglaMotherName,
                                        emname = vl.pi.EnglishMotherName,

                                        dob = vl.pi.DOB,
                                        maritalstatus = vl.pi.MaritalStatus,
                                        smname = vl.pi.WORHName,
                                        smpasa = vl.pi.WorHOccupation,
                                        nid = vl.pi.NID,
                                        dobno = vl.pi.DOBSNo,


                                        occupation = vl.pi.occupation,
                                        religion = r.Name,


                                        presaddress = vl.a.PerAddress,
                                        prespostcode = vl.a.PresPostCodeId,
                                        presunion = vl.a.PresUnion,
                                      
                                        presthana = pt.Name,
                                        presdist = pd.Name,

                                        height = hf.Name + " " + hi.Name,
                                        weight = vl.bs.WeightKg + " কেজি",
                                        zender = g.Name,
                                        cyecolor = ec.Name,
                                        bodycolor = vl.bs.bodycolor,
                                        bloodgroup = bg.Name,
                                        sanaktakarn = vl.bs.SanaktakaranSign,

                                        statusid = vl.m.Status,
                                        membercode = vl.m.IDCardNo,
                                        mobile = vl.pi.mobile
                                    };
                    model = volantier.SingleOrDefault();
                }
            }
          
            return View(model);
        }
        //
        // GET: /Account/Register
        [HttpGet]
        [SessionExpire]
        [AuthorizeRoles("SuperAdmin")]
        public ActionResult RangRegister()
        {
           
            TempData["e"] = Session["usertype"];
          
               
                var s = from d in cdb.Distric.ToList()
                        where !(from g in db.RangDistrict.ToList() select g.DistrictId).Contains(d.Id)
                        select new
                        {
                            Id = d.Id,
                            Name = d.Name
                        };

                ViewBag.districts = new SelectList(s.ToList(), "Id", "Name");
               
               
        
            return View();
        }
        [HttpPost]
        [SessionExpire]
        [AuthorizeRoles("SuperAdmin")]
        public async Task<ActionResult>RangRegister(RegisterViewModel model, int[] cked)
        {
            TempData["e"] = Session["usertype"];
            var s = db.User.Where(x => x.UserName.Equals(model.UserName.ToLower())).Count();
            if (s > 0)
            {
                TempData["error"] = "UserName alrady Exit.";
            }
            else if (cked == null)
            {
                TempData["error"] = "Select District.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var user = new User() { UserName = model.UserName };
                    user.Password = Encrypt(model.Password);
                    user.Createdby = User.Identity.Name;
                    user.Id = Guid.NewGuid();
                    user.UserName = model.UserName;
                    user.IsActive = true;
                    var userrole = new UserRole();
                    userrole.Id = Guid.NewGuid();
                    userrole.UserId = user.Id;
                    userrole.DistricId = 900;
                    userrole.roleid = 3;
                        foreach (var i in cked)
                        {
                            var ud = new  RangDistrict();
                            ud.Id = Guid.NewGuid();
                            ud.UserId = user.Id;
                            ud.DistrictId = i;
                            db.RangDistrict.Add(ud);
                            db.SaveChanges();
                        }

                    db.User.Add(user);
                    db.UserRole.Add(userrole);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["success"] = "Registration ";

                }
            }
            return RedirectToAction("RangRegister");
        }
        [HttpGet]
        [SessionExpire]
        [AuthorizeRoles("SuperAdmin","Admin")]
        public ActionResult Register()
        {
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var count = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.DistricId).FirstOrDefault();
            TempData["e"] = Session["usertype"];
            if (count > 0)
            {
                var ud = db.UserDistic.Where(x => x.UserId.Equals(userid)).ToList();
                var s = from sd in cdb.SubDistric.ToList()
                        where (from g in ud.ToList() select g.SubdisticId).Contains(sd.Id)
                        select new
                        {
                            Id = sd.Id,
                            Name = sd.Name
                        };
               
                ViewBag.districs = new SelectList(cdb.Distric, "Id", "Name");
                ViewBag.Subdistrics = new SelectList(s.ToList(), "Id", "Name");
                ViewBag.platunlist = new SelectList(db.Platun.Where(x => x.UserId.Equals(userid)), "Id", "PlatuneName");
            }
            else
            {
                var ud = db.UserDistic.Where(x => x.UserId.Equals(userid)).ToList();
                var s = from sd in cdb.SubDistric.ToList()
                        where !(from g in ud.ToList() select g.SubdisticId).Contains(sd.Id)
                        select new
                        {
                            Id = sd.Id,
                            Name = sd.Name
                        };
                TempData["view"] = "s";
                ViewBag.districs = new SelectList(cdb.Distric, "Id", "Name");
                ViewBag.Subdistrics = new SelectList(s.ToList(), "Id", "Name");
            }

            List<SelectListItem> identityname = new List<SelectListItem>();
            identityname.Add(new SelectListItem { Text = "Admin", Value = "1" });
            identityname.Add(new SelectListItem { Text = "SubAdmin", Value = "2" });
            ViewBag.pslist = new SelectList(identityname.ToList(), "Value", "Text");
          
            return View();
        }
        //
        // POST: /Account/Register
        [SessionExpire]
        [AuthorizeRoles("SuperAdmin","Admin")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, int[] cked, Guid[] platun)
        {
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Id).FirstOrDefault();
            var superadmin = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            var s = db.User.Where(x => x.UserName.Equals(model.UserName.ToLower())).Count();
            if (s > 0)
            {
                TempData["error"] = "UserName alrady Exit.";
            }
           else if(cked==null && superadmin==0)
            {
                TempData["error"] = "Select SubDistric.";
            }
            else if (platun == null && superadmin > 0)
            {
                TempData["error"] = "Select Platun.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var user = new User() { UserName = model.UserName };
                    user.Password = Encrypt(model.Password);
                    user.Createdby = User.Identity.Name;
                    user.Id = Guid.NewGuid();
                    user.UserName = model.UserName;
                    user.IsActive = true;
                    var userrole = new UserRole();
                    userrole.Id = Guid.NewGuid();
                    userrole.UserId = user.Id;
                    userrole.roleid = model.role;
                    if(model.distric==0)
                    {
                        userrole.DistricId =Convert.ToInt16(Session["Distrcid"]);
                    }
                    else
                    {
                        userrole.DistricId = model.distric;
                    }
                    if (superadmin == 0)
                    {
                        foreach (var i in cked)
                        {
                            var ud = new UserDistic();
                            ud.UserId = user.Id;
                            ud.SubdisticId = i;
                            db.UserDistic.Add(ud);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        foreach (var i in platun)
                        {
                            var ud = new UserPlatun();
                            ud.UserId = user.Id;
                            ud.PlatunId= i;
                            db.UserPlatun.Add(ud);
                            db.SaveChanges();
                        }
                    }
                    db.User.Add(user);
                    db.UserRole.Add(userrole);
                    db.SaveChanges();
                    ModelState.Clear();

                }
            }

            TempData["e"] = Session["usertype"];
            return RedirectToAction("Register");
        }

        [HttpGet]
        [SessionExpire]
        [AuthorizeRoles("SuperAdmin","Admin")]
        public ActionResult Inactive()
        {
          
            TempData["e"] = Session["usertype"];
            var dislist=cdb.Distric.ToList();
            var userlist = from u in db.User.ToList()
                           join ur in db.UserRole.ToList() on u.Id equals ur.UserId
                           join r in db.Role.ToList() on ur.roleid equals r.Id
                           join dl in dislist.ToList() on ur.DistricId equals dl.Id
                           where !u.UserName.ToUpper().Equals(User.Identity.Name.ToUpper()) && u.Createdby.ToUpper().Equals(User.Identity.Name.ToUpper())
                           select new uurd
                           {
                                u=u,
                                ur=ur,
                                d=dl,
                                r=r,
                                password = Decrypt(u.Password)
                           };
          
            return View(userlist.ToList());
        }

        [HttpGet]
        [AuthorizeRoles("SuperAdmin","Admin")]
        [SessionExpire]
        public ActionResult deactive(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = new User();
            var role = db.UserRole.Where(x => x.UserId.Equals(id)).Select(x=>x.roleid).SingleOrDefault();
            model = db.User.Where(x => x.Id.Equals(id)).SingleOrDefault();
            if(role==1)
            {
                var username = db.User.Where(x => x.Id.Equals(id)).Select(x => x.UserName).SingleOrDefault();
                var userlist = db.User.Where(x => x.Createdby.Equals(username)).Select(x=>x.Id).ToList();
                foreach(var i in userlist)
                {
                    var s = db.User.Where(x => x.Id.Equals(i)).SingleOrDefault();
                    s.IsActive = false;
                    db.Entry(s).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
           
             model.IsActive = false;
          
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            var roleid = db.UserRole.Where(x => x.UserId.Equals(id)).Select(x => x.roleid).FirstOrDefault();
            if (roleid == 3)
            {
                return RedirectToAction("RangCommandarList");
            }
            else
            {

                return RedirectToAction("Inactive");
            }
        }
        [HttpGet]
        [AuthorizeRoles("SuperAdmin", "Admin")]
        [SessionExpire]
        public ActionResult activate(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = new User();
           
            model = db.User.Where(x => x.Id.Equals(id)).SingleOrDefault();
          
                model.IsActive = true;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                var roleid = db.UserRole.Where(x => x.UserId.Equals(id)).Select(x => x.roleid).FirstOrDefault();
                if (roleid == 3)
                {
                    return RedirectToAction("RangCommandarList");
                }
                else
                {

                    return RedirectToAction("Inactive");
                }
        }
        [HttpGet]
        [SessionExpire]
        [AuthorizeRoles("Admin")]
        public ActionResult userplatunedit(Guid id)
         {
             TempData["e"] = Session["usertype"];
             var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
             var ud = db.UserDistic.Where(x => x.UserId.Equals(userid)).ToList();
             var presdis = from sd in cdb.Distric.ToList()
                           where sd.Id.Equals(Session["Distrcid"])
                           select new
                           {
                               Id = sd.Id,
                               Name = sd.Name
                           };
             ViewBag.presdistrics = new SelectList(presdis.ToList(), "Id", "Name");
             TempData["presd"] = Session["Distrcid"];
             var pressubdis = from sd in cdb.SubDistric.ToList()
                              where (from g in ud.ToList() select g.SubdisticId).Contains(sd.Id)
                              select new
                              {
                                  Id = sd.Id,
                                  Name = sd.Name
                              };
             ViewBag.presSubdistrics = new SelectList(pressubdis.ToList(), "Id", "Name");
             var plalist = from up in db.UserPlatun.ToList()
                           join st in db.Platun.ToList() on up.PlatunId equals st.Id
                           where (up.UserId.Equals(id))
                              select new 
                              {
                                  Id = up.Id,
                                  PlatunName = st.PlatuneName,
                                 
                              };
             ViewBag.platunlist = new SelectList(plalist.ToList(), "Id", "PlatunName");
             ViewBag.userid = id;
             ViewBag.username = db.User.Where(x => x.Id == id).Select(x => x.UserName).FirstOrDefault();
             return View();
         }
        [HttpGet]
        [AuthorizeRoles("SuperAdmin")]
        [SessionExpire]
        public ActionResult RangCommandarList()
        {
            TempData["e"] = Session["usertype"];
          
            var userlist = from u in db.User.ToList()
                           join ur in db.UserRole.ToList() on u.Id equals ur.UserId
                           join r in db.Role.ToList() on ur.roleid equals r.Id
                           where !u.UserName.ToUpper().Equals(User.Identity.Name.ToUpper()) && u.Createdby.ToUpper().Equals(User.Identity.Name.ToUpper()) && r.Id==3
                           select new uurd
                           {
                               u = u,
                               ur = ur,
                               r = r,
                               password = Decrypt(u.Password)
                           };

            return View(userlist.ToList());
            
        }
        [HttpGet]
        [AuthorizeRoles("SuperAdmin")]
        [SessionExpire]
        public ActionResult userdistrictedit(Guid id)
        {
            ViewBag.userid = id;
             var ud = db.RangDistrict.Where(x => x.UserId.Equals(id)).ToList();
            var districtlist = from dis in cdb.Distric.ToList()
                               where !(from g in ud.ToList() select g.DistrictId).Contains(dis.Id)
                               select new
                               {
                                  Id = dis.Id,
                                  Name = dis.Name
                               };
            ViewBag.dislist = new SelectList(districtlist.ToList(), "Id", "Name");
            var districlist = from rdis in db.RangDistrict.ToList()
                              join dis in cdb.Distric.ToList() on rdis.DistrictId equals dis.Id
                              where rdis.UserId.Equals(id)
                              select new
                              {
                               Id=rdis.Id,
                               Name=dis.Name
                              };
            ViewBag.districtlist = new SelectList(districlist.ToList(),"Id","Name");
            ViewBag.username = db.User.Where(x => x.Id.Equals(id)).Select(x=>x.UserName).FirstOrDefault();
            return View();
        }

        [HttpGet]
        [SessionExpire]
        public ActionResult adminSubdistriclist(int id)
        {
            TempData["e"] = Session["usertype"];
            var ud = db.UserDistic.ToList(); 
            var s = from sd in cdb.SubDistric.ToList()
                    where sd.countryid.Equals(id) && !(from g in ud.ToList() select g.SubdisticId).Contains(sd.Id)
                    select new
                    {
                        Id = sd.Id,
                        Name = sd.Name
                    };
      
            ViewBag.Subdistrics = new SelectList(s.ToList(), "Id", "Name");
       
            return PartialView("adminSubdistric");
        }
        private string Encrypt(string clearText)
        {
            
            string EncryptionKey = "Zencon626272";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "Zencon626272";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        private string DecryptAdmin(string cipherText)
        {
            string EncryptionKey = "Zencon626272Admin";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

       [HttpGet]
       public ActionResult PasswordChange()
        {
            TempData["e"] = Session["usertype"];
            return View();
        }
       [HttpPost]
       public ActionResult PasswordChange(ManageUserViewModel model)
        {
            TempData["e"] = Session["usertype"];
            var s = User.Identity.Name;
            var user = db.User.Where(x => x.UserName.Equals(s)).SingleOrDefault();
            user.Password =Encrypt(model.NewPassword);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PasswordChange");
        }

       [HttpPost]
       public JsonResult GetPassword(string idd)
        {
            string data = "";
      
            string password = db.User.Where(x=>x.UserName.Equals(User.Identity.Name)).Select(x=>x.Password).SingleOrDefault();
            string s = Decrypt(password);
           if(s==idd)
           {
               data = "true";
           }
           else
           {
               data = "false";
           }
          
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
       public ActionResult GetPlatun(string idd, Guid userid)
       {
           int s = int.Parse(idd);

           var platunname = from p in db.Platun.ToList()
                            where p.SubDistrcId.Equals(s) && p.IsActive.Equals(true) && !(from g in db.UserPlatun.ToList() where g.UserId.Equals(userid) select g.PlatunId).Contains(p.Id)
                            select new
                            {
                                Id=p.Id,
                                PlatuneName=p.PlatuneName
                            };

           SelectList objdata = new SelectList(platunname.ToList(), "Id", "PlatuneName", 0);

           return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
       }
       public ActionResult AddPlatun(Guid plaid,Guid userid)
       {
           UserPlatun up = new UserPlatun();
           up.PlatunId = plaid;
           up.UserId = userid;
           db.UserPlatun.Add(up);
           db.SaveChanges();
           var plalist = from usp in db.UserPlatun.ToList()
                         join st in db.Platun.ToList() on usp.PlatunId equals st.Id
                         where (usp.UserId.Equals(userid))
                         select new
                         {
                             Id = usp.Id,
                             PlatunName = st.PlatuneName,

                         };

           SelectList objdata = new SelectList(plalist.ToList(), "Id", "PlatunName", 0);

           return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
       }
       public JsonResult AddDistrict(int dis,Guid userid)
       {
           RangDistrict up = new RangDistrict();
           up.Id =Guid.NewGuid();
           up.DistrictId = dis;
           up.UserId = userid;
           db.RangDistrict.Add(up);
           db.SaveChanges();
           var plalist = from usp in db.RangDistrict.ToList()
                         join st in cdb.Distric.ToList() on usp.DistrictId equals st.Id
                         where (usp.UserId.Equals(userid))
                         select new
                         {
                             Id = usp.Id,
                             Name = st.Name,

                         };

           SelectList objdata = new SelectList(plalist.ToList(), "Id", "Name", 0);

           return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
       }
       public JsonResult Removedistrict(Guid id, Guid userid)
       {
      
               RangDistrict up = db.RangDistrict.Find(id);
               db.RangDistrict.Remove(up);
               db.SaveChanges();
               var ud = db.RangDistrict.Where(x => x.UserId.Equals(userid)).ToList();
               var districtlist = from dis in cdb.Distric.ToList()
                                  where !(from g in ud.ToList() select g.DistrictId).Contains(dis.Id)
                                  select new
                                  {
                                      Id = dis.Id,
                                      Name = dis.Name
                                  };
               SelectList objdata = new SelectList(districtlist.ToList(), "Id", "Name", 0);

           return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
       }
       public ActionResult RemovePlatun(int id)
       {
           try
           {
               UserPlatun up = db.UserPlatun.Find(id);
               db.UserPlatun.Remove(up);
               db.SaveChanges();
               return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
           }
           catch
           {
               return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
           }
       }
       private ActionResult RedirectToLocal(string returnUrl)
       {
           if (Url.IsLocalUrl(returnUrl))
           {
               return Redirect(returnUrl);
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }

    
    }
}