using BAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using BAV.Security;
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace BAV.Controllers
{
    //[SessionExpire]
    public class MemberController : Controller
    {
       public UsersContext db = new UsersContext();
       public CommonContext cdb = new CommonContext();
       public  MemberRegistrationContext mrbd = new MemberRegistrationContext();
       
        //
       // GET: /Member/PrintlistMember
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Register a Member
        /// </summary>
        /// <returns></returns>
        ///   [AllowAnonymous]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Personalinfo()
        {
            var s = User.Identity.Name;
            var userid = db.User.Where(x => x.UserName.Equals(s)).Select(x => x.Id).SingleOrDefault();
            var platun = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            int disid=Convert.ToInt16(Session["Distrcid"]);
            TempData["e"] = Session["usertype"];
            ViewBag.exam = new SelectList(cdb.Exam, "Id", "Name");
            ViewBag.Religion = new SelectList(cdb.Religion, "Id", "Name");
            ViewBag.countrylist = new SelectList(cdb.Distric, "Id", "Name");
            ViewBag.designationlist = new SelectList(cdb.Designation, "Id", "Name");
            ViewBag.dobpasslist = new SelectList(cdb.Type.OrderByDescending(x=>x.Id).ToList(), "Id", "Name");
            if (s == string.Empty)
            {
                ViewBag.platunname = new SelectList(db.Platun.Where(x=>x.IsActive.Equals(true )).ToList(), "Id", "PlatuneName");
            }
            else
            {
                if (platun > 0)
                {
                    ViewBag.platunname = new SelectList(db.Platun.Where(x => x.UserId.Equals(userid) && x.IsActive.Equals(true)), "Id", "PlatuneName");
                    ViewBag.subdistriclist = new SelectList(cdb.SubDistric.Where(x => x.countryid.Equals(disid)), "Id", "Name");
                }
                else
                {
                    var platunlist = from up in db.UserPlatun.ToList()
                                     join st in db.Platun.ToList() on up.PlatunId equals st.Id
                                     where (up.UserId.Equals(userid))
                                     select new
                                     {
                                         Id = st.Id,
                                         PlatuneName = st.PlatuneName
                                     };

                    ViewBag.platunname = new SelectList(platunlist.ToList(), "Id", "PlatuneName");
                    var subdistricidlist = from up in db.UserPlatun.ToList()
                                         join st in db.Platun.ToList() on up.PlatunId equals st.Id
                                         where (up.UserId.Equals(userid))
                                         group st by st.SubDistrcId into g
                                         select new
                                         {
                                           districid = g.Key,
                                         
                                         };
                    var subdistriclist = from sdi in subdistricidlist.ToList()
                                         join sd in cdb.SubDistric.ToList() on sdi.districid equals sd.Id
                                         select new
                                         {
                                             Id=sd.Id,
                                             Name=sd.Name
                                         };

                    ViewBag.subdistriclist = new SelectList(subdistriclist.ToList(), "Id", "Name");
                }
            }
          
           
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Personalinfo(PersonalInfo model)
        {
            var s = mrbd.PersonalInfo.Where(x => x.NID.Equals(model.NID) && x.NID != null).Count();
            var m = mrbd.PersonalInfo.Where(x => x.mobile.Equals(model.mobile)).Count();
            var jn = mrbd.PersonalInfo.Where(x => x.DOBSNo.Equals(model.DOBSNo) && x.DOBSNo != null).Count();
            if (s > 0 || jn>0)
            {
                TempData["eror"] = "নিবন্ধনভুক্ত সদস্য";
                return RedirectToAction("Personalinfo");
            }
            else
            {
                TempData["e"] = Session["usertype"];
                Session["Personalinfo"] = model;
                return RedirectToAction("AddressInfo");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddressInfo()
        {
          
            TempData["e"] = Session["usertype"];
            if(Session["Personalinfo"]==null)
            {
                return RedirectToAction("Personalinfo");
            }

            ViewBag.presdistrics = new SelectList(cdb.Distric, "Id", "Name");
            ViewBag.distric = new SelectList(cdb.Distric, "Id", "Name");
           
         
           return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddressInfo(Address ad)
        {
            TempData["e"] = Session["usertype"];
           Session["Addressinfo"] = ad;

            return RedirectToAction("BodyStractureinfo");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult BodyStractureinfo()
        {
         
            TempData["e"] = Session["usertype"];
            if (Session["Personalinfo"] == null && Session["Addressinfo"]==null)
            {
                return RedirectToAction("Personalinfo");
            }
            else if (Session["Personalinfo"] != null && Session["Addressinfo"] == null)
            {
                return RedirectToAction("AddressInfo");
            }
   
            ViewBag.eyecolor = new SelectList(cdb.EyeColor, "Id", "Name");
            ViewBag.BloodGroup = new SelectList(cdb.BloodGroup, "Id", "Name");
         
            ViewBag.Foot = new SelectList(cdb.Foot, "Id", "Name");
            ViewBag.Inch = new SelectList(cdb.Inch, "Id", "Name");
            var model = (BAV.Models.PersonalInfo)Session["Personalinfo"];
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult BodyStractureinfo(BodyStructure ps, string bid)
        {
            TempData["e"] = Session["usertype"];
            Session["BodyStractureinfo"] = ps;
            Session["bid"] = bid;

            return RedirectToAction("MemberImageInfo");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult MemberImageInfo()
        {
            TempData["e"] = Session["usertype"];
            if (Session["Personalinfo"] == null && Session["Addressinfo"] == null && Session["BodyStractureinfo"]==null)
            {
                return RedirectToAction("Personalinfo");
            }
            else if (Session["Personalinfo"] != null && Session["Addressinfo"] == null)
            {
                return RedirectToAction("AddressInfo");
            }
           
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(BAV.Models.Image img)
        {
            var cookee = User.Identity.Name;
            TempData["e"] = Session["usertype"];
            var userid=db.User.Where(x=>x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x=>x.Id).FirstOrDefault();
            var ps = (BAV.Models.PersonalInfo)Session["Personalinfo"];
            var p=(BAV.Models.Address) Session["Addressinfo"];
            var bs=(BAV.Models.BodyStructure)Session["BodyStractureinfo"];
            string bid =Convert.ToString(Session["bid"]);
            string nidname = "";
            string dobname = "";
            string picname = "";
            string signature = "";
            var s = mrbd.PersonalInfo.Where(x => x.NID.Equals(ps.NID) && x.NID!=null).Count();
            if (s > 0)
            {
                TempData["eror"] = "নিবন্ধনভুক্ত সদস্য";
            }

            else
            {

                if (img.NIDImages != null && img.NIDImages.ContentLength > 0)
                {

                    nidname = Guid.NewGuid() + "." + img.NIDImages.FileName.Split('.')[1];
                    string targetPath = Server.MapPath("../NIDImage//" + nidname);
                    Stream strm = img.NIDImages.InputStream;
                    var targetFile = targetPath;

                    GenerateThumbnails(0.5, strm, targetFile);

                }
                if (img.DOBSImaged != null && img.DOBSImaged.ContentLength > 0)
                {

                    dobname = Guid.NewGuid() + "." + img.DOBSImaged.FileName.Split('.')[1];
                    string targetPath = Server.MapPath("../DOBImage//" + dobname);
                    Stream strm = img.DOBSImaged.InputStream;
                    var targetFile = targetPath;

                    GenerateThumbnails(0.5, strm, targetFile);

                }
                if (img.pictures != null && img.pictures.ContentLength > 0)
                {

                    picname = Guid.NewGuid() + "." + img.pictures.FileName.Split('.')[1];
                    string targetPath = Server.MapPath("../VolantiarImage//" + picname);
                    Stream strm = img.pictures.InputStream;
                    var targetFile = targetPath;

                    GenerateThumbnails(0.5, strm, targetFile);

                }
                if (img.ImageofSigned != null && img.ImageofSigned.ContentLength > 0)
                {

                    signature = Guid.NewGuid() + "." + img.ImageofSigned.FileName.Split('.')[1];
                    string targetPath = Server.MapPath("../MemberSignature//" + signature);
                    Stream strm = img.ImageofSigned.InputStream;
                    var targetFile = targetPath;

                    GenerateThumbnails(0.5, strm, targetFile);

                }
                ps.Id = Guid.NewGuid();
              
                p.Id = Guid.NewGuid();
                int pd = p.PerDistric;
                int ped = p.PresDistric;
               if ((pd >= 3 && pd <= 13)||pd==74)
                {
                    p.PerPostCodeId = p.PerPostCodeId;
                    p.PerWard = p.PerWard;
                }
                else
                {
                    p.PerWard = p.PerWardM;
                    p.PerPostCodeId = p.PerPostCodeIdM;
                   
                }
             
               if ((ped >= 3 && ped <= 13) || pd == 74)
                {
                    p.PresWard = p.PresWard;
                    p.PresPostCodeId = p.PresPostCodeId;
                }
                else
               {
                   p.PresWard = p.PresWardM;
                    p.PresPostCodeId = p.PresPostCodeIdM;
                }

                p.MemberId = ps.Id;
                var member = new Member();
                member.Id = Guid.NewGuid();
                member.MemberId = ps.Id;
                member.Status = 1;
                member.MemberType = 1;
                member.IsActive = false;
                member.CheckBeforeApproveddate = DateTime.Now.Date;
                member.CheckBeforeApproved = userid;
                member.platunername = ps.platunername;
             
                member.ApprovedBy = Guid.Empty;
                var model = new BodyStructure();
                model.Id = Guid.NewGuid();
                model.MemberId = ps.Id;
                model.footid = bs.footid;
                model.inchid = bs.inchid;
                model.WeightKg = bs.WeightKg;

                if (bid == "")
                {
                    model.BloodGroupId = 0;
                }
                else
                {
                    model.BloodGroupId = int.Parse(bid);
                }

                model.bodycolor = bs.bodycolor;

                model.EyeColorId = bs.EyeColorId;

                model.GenderId = bs.GenderId;

                model.SanaktakaranSign = bs.SanaktakaranSign;

                var imge = new BAV.Models.Image();
                imge.Id = Guid.NewGuid();
                imge.MemberId = ps.Id;
                imge.DOBSImage = dobname;
                imge.NIDImage = nidname;
                imge.picture = picname;
                imge.ImageofBsign = signature;
                mrbd.Image.Add(imge);
                mrbd.BodyStructure.Add(model);
                mrbd.PersonalInfo.Add(ps);
                mrbd.Address.Add(p);
                mrbd.Member.Add(member);
                mrbd.SaveChanges();
                Session["Personalinfo"]=null;
                Session["Addressinfo"]=null;
                Session["BodyStractureinfo"]=null;
                Session["bid"]=null;
                img = null;
                TempData["eror"] = "সদস্য রেজিস্ট্রেশন সফল হয়েছে";
            }
            return RedirectToAction("Personalinfo");
        }
        /// <summary>
        /// Register a Member End
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MemberList(int? Page_No, int? id)
        {
            var st = User.Identity.Name;
            var userid = db.User.Where(x => x.UserName.Equals(st)).Select(x => x.Id).SingleOrDefault();
            int s = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.roleid).SingleOrDefault();
            TempData["e"]=Session["usertype"];

            if (s == 1 && id == null)
            {
                TempData["Paindding"] = "List";
                TempData["s"] = "pd";
            
            }
            else if(s==2 && id==null)
            {
                TempData["Paindding"] = "List";
                TempData["s"] = "pd";
              
            }
            else if(s==1 && id==2)
            {
                TempData["Approved"] = "List";
                TempData["s"] = "av";
              
            }
            else if (s ==2 && id == 2)
            {
                TempData["Approved"] = "List";
                TempData["s"] = "av";
              
            }
          
            ViewBag.Status = new SelectList(cdb.Status, "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult MemberList(int? Page_No,string Status)
        {
            TempData["e"] = Session["usertype"];
            var st = User.Identity.Name;
            var userid = db.User.Where(x => x.UserName.Equals(st)).Select(x => x.Id).SingleOrDefault();
            int s = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.roleid).SingleOrDefault();
           
            ViewBag.Status = new SelectList(cdb.Status, "Id", "Name");
            if (s == 1)
            {
                if (Status == "1")
                {
                    TempData["Paindding"] = "List";
                    TempData["s"] = "pd";
                    ViewBag.text = "l1";
                }
                else if (Status == "2")
                {
                    TempData["Approved"] = "List";
                    TempData["s"] = "av";
                    ViewBag.text = "l1";
                }
             
            }
            else if(s==2)
            {
                if (Status == "1")
                {
                    TempData["Paindding"] = "List";
                    TempData["s"] = "pd";
                    ViewBag.text = "l1";
                }
                else if (Status == "2")
                {
                    TempData["Approved"] = "List";
                    TempData["s"] = "av";
                    ViewBag.text = "l1";
                }
            }
            
            return View();
        }
        public ActionResult MemberDetailsList(int? Page_No,string id)
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            ViewBag.degisnationlist = new SelectList(cdb.Designation, "Id", "Name");
            TempData["e"] = Session["usertype"];
            int status;
            if (id == "pd")
            {
                status = 1;
               
            }
           else if (id == "av")
            {
                status = 2;
                TempData["s"] = "p";
                ViewBag.id = 2;
            }
            else
            {
                status = 3;
            }
         
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var count = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            int s = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.roleid).SingleOrDefault();
           
            if (count > 0)
            {
                ViewBag.platunname = new SelectList(db.Platun.Where(x => x.UserId.Equals(userid) && x.IsActive.Equals(true)), "Id", "PlatuneName");
            }
            else
            {
                var platunlist = from up in db.UserPlatun.ToList()
                                 join st in db.Platun.ToList() on up.PlatunId equals st.Id
                                 where (up.UserId.Equals(userid))
                                 select new
                                 {
                                     Id = st.Id,
                                     PlatuneName = st.PlatuneName
                                 };

                ViewBag.platunname = new SelectList(platunlist.ToList(), "Id", "PlatuneName");
            }
            if (count > 0)
            {
                
                var plalist = db.Platun.Where(x => x.UserId.Equals(userid)).ToList();
                if (status == 2)
                {
                    model = volantiarlist(status);
                    var vollist = from mo in model.ToList()
                                  join pla in plalist.ToList() on mo.pi.platunId equals pla.Id
                                  join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                                  select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, disignation=dig.Name };
                     model = vollist.ToList();
                }
                else if(status==1)
                {
                    model = volantiarlist(status);
                    var vollist = from mo in model.ToList()
                                  join pla in plalist.ToList() on mo.pi.platunId equals pla.Id
                                  join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                                  where !mo.m.CheckBeforeApproved.Equals(Guid.Empty)
                                  select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, disignation = dig.Name };
                    model = vollist.ToList();
                }
               
              
            }
            else
            {
                var plalist = from up in db.UserPlatun.ToList()
                              join st in db.Platun.ToList() on up.PlatunId equals st.Id
                              where (up.UserId.Equals(userid))
                              select new
                              {
                                  Id = st.Id,
                                  PlatuneName = st.PlatuneName,
                                  UserId = up.UserId
                              };
                if (s == 1)
                {
                    if (status == 2)
                    {
                        model = volantiarlist(status);
                        var vollist = from mo in model.ToList()
                                      join pla in plalist.ToList() on mo.pi.platunId equals pla.Id
                                      join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                                      select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, disignation = dig.Name };
                        model = vollist.ToList();
                    }
                    else if (status == 1)
                    {
                        model = volantiarlist(status);
                        var vollist = from mo in model.ToList()
                                      join pla in plalist.ToList() on mo.pi.platunId equals pla.Id
                                      join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                                      where !mo.m.CheckBeforeApproved.Equals(Guid.Empty)
                                      select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, disignation = dig.Name };
                        model = vollist.ToList();
                    }

                }
                else if (s == 2)
                {
                    if (status == 2)
                    {
                        model = volantiarlist(status);
                        var vollist = from mo in model.ToList()
                                      join pla in plalist.ToList() on mo.pi.platunId equals pla.Id
                                      join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                                      select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, disignation = dig.Name };
                    
                        model = vollist.ToList();
                    }
                    else if(status==1)
                    {
                        model = volantiarlist(status);
                        var vollist = from mo in model.ToList()
                                      join pla in plalist.ToList() on mo.pi.platunId equals pla.Id
                                      join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                                      where mo.m.CheckBeforeApproved.Equals(Guid.Empty)
                                      select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, disignation = dig.Name };
                  
                        model = vollist.ToList();
                    }
                }
            }
            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            return View(model);
            //return View(model.ToPagedList(No_Of_Page, Size_Of_Page));
        
         
          
        }
        private List<PersonalInfoMemberAddressBodyStructure> volantiarlist(int status)
        {
              var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                  join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                  join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                  join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                  orderby (pi.Date)
                                  where m.Status.Equals(status) && m.IsActive.Equals(false)
                                  select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi};
           
            return volantierlist.ToList();
             
        }
        public ActionResult PrasikkanDetails(Guid id)
        {
           TempData["e"] = Session["usertype"];
           var prname=new List<prname>();
           var prasikkanlist = from p in mrbd.Prasikkan.ToList()
                               join pn in cdb.PrasikkanName.ToList() on p.PraNameId equals pn.Id
                               where p.MemberId.Equals(id)
                               select new prname
                               {
                                  p=p,
                                  pn=pn
                                  
                               };
                              prname=prasikkanlist.ToList();
                              return View(prname);
        }
        public ActionResult EventDetailsList(Guid id)
        {
            TempData["e"] = Session["usertype"];
           var prname=new List<eve>();
           var eventlist = from em in mrbd.EventMember.ToList()
                           join e in mrbd.Event.ToList() on em.eventId equals e.Id
                           where em.memberId.Equals(id) && (em.workfor=="m" || em.workfor=="h" || em.workfor=="c")
                           select new
                               {
                                  even=e,
                                  EventMember=em,
                                  day = Convert.ToInt32((em.endtime - em.starttime).TotalDays),
                               };
          
           var eventlists = from e in eventlist.ToList()
                            select new eve
                            {
                                even=e.even,
                                EventMember=e.EventMember, 
                                month = e.day / 30,
                                day = e.day % 30
                            };

         
           prname = eventlists.ToList();
           return View(prname);
        }
        public ActionResult PlatunchangeHistry(Guid id)
        {
            TempData["e"] = Session["usertype"];
             var listplatun=db.Platun.ToList();
             var platunlist = from cmb in mrbd.MemberPlatunChange.ToList()
                              join p in listplatun.ToList() on cmb.PlatunId equals p.Id
                              where cmb.MemberId.Equals(id)
                              select new MemberPlatunChangeList
                              {
                                  PlatunName=p.PlatuneName,
                                  startdate=cmb.StartDate,
                                  enddate=cmb.EndDate
                              };
             return View(platunlist.ToList());
        }

        [HttpGet]
        public ActionResult MemberDetails(Guid id)
        {
            TempData["e"] = Session["usertype"];
            string role =Convert.ToString(Session["rolename"]);
            if (role == "SubAdmin")
            {
                ViewBag.type = "true";
            }
            PersonalInfoMemberAddressBodyStructureReligion model = new PersonalInfoMemberAddressBodyStructureReligion();
            List<PlatunList> platunlist = new List<PlatunList>();
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var count = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
           if(count>0)
           {
            var plalist = from p in db.Platun.ToList()
                            where p.UserId.Equals(userid)
                            select new PlatunList  
                            {
                                Id= p.Id,
                                UserId= p.UserId,
                                PlatunName= p.PlatuneName
                            };
            platunlist = plalist.ToList();
           }
            else
           {
             var plalist =   from up in db.UserPlatun.ToList()
                             join st in db.Platun.ToList() on up.PlatunId equals st.Id
                             where (up.UserId.Equals(userid))
                             select new PlatunList
                             {
                                 Id = st.Id,
                                 PlatunName = st.PlatuneName,
                                 UserId = up.UserId
                             };
             platunlist = plalist.ToList();

           }
           
            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist.ToList() on pi.platunId equals pla.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                join i in mrbd.Image.ToList() on pi.Id equals i.MemberId
                                where pi.Id.Equals(id)
                                select new PersonalInfoMemberAddressBodyStructure
                                {
                                    pi = pi,
                                    m = m,
                                    a = a,
                                    bs = bs,
                                    i=i,
                                  planame=pla.PlatunName
                                };
            var s = volantierlist.Select(x=>x.bs.BloodGroupId).SingleOrDefault();
          if (s == 0)
          {
              using (cdb = new CommonContext())
              {

                  var volantier = from vl in volantierlist.ToList()
                                  join dig in cdb.Designation.ToList() on vl.pi.DesignationId equals dig.Id
                                  join r in cdb.Religion on vl.pi.ReligionId equals r.Id
                                  join pt in cdb.SubDistric on vl.a.PresSubDistric equals pt.Id
                                  join pd in cdb.Distric on vl.a.PresDistric equals pd.Id
                                  join pet in cdb.SubDistric on vl.a.PerSubDistric equals pet.Id
                                  join ped in cdb.Distric on vl.a.PerDistric equals ped.Id
                                  join hi in cdb.Inch on vl.bs.inchid equals hi.Id
                                  join hf in cdb.Foot on vl.bs.footid equals hf.Id
                                  join g in cdb.Gender on vl.bs.GenderId equals g.Id
                                  join ec in cdb.EyeColor on vl.bs.EyeColorId equals ec.Id
                                
                                  select new PersonalInfoMemberAddressBodyStructureReligion
                                  {
                                      Id = vl.pi.Id,
                                      Imge = vl.i.picture,
                                      membersign = vl.i.ImageofBsign,
                                      platunname = vl.planame,
                                      bname = vl.pi.BanglaName,
                                      ename = vl.pi.EnglishName,
                                      bfname = vl.pi.BanglaFatherName,
                                      efname = vl.pi.EnglishFatherName,
                                      bmname = vl.pi.BanglaMotherName,
                                      emname = vl.pi.EnglishMotherName,
                                      designation = dig.Name,
                                      sandadno= vl.pi.dp,

                                      dob = vl.pi.DOB,
                                      maritalstatus = vl.pi.MaritalStatus,
                                      smname = vl.pi.WORHName,
                                      smpasa = vl.pi.WorHOccupation,
                                      nid = vl.pi.NID,
                                      dobno = vl.pi.DOBSNo,
                                      p=vl.pi.p,
                                      po=vl.pi.po,
                                      pos=vl.pi.pos,
                                      pot=vl.pi.pot,
                                      ps=vl.pi.ps,
                                      pst=vl.pi.pst,
                                      isactive=vl.m.IsActive,



                                      occupation = vl.pi.occupation,
                                      religion = r.Name,


                                      presaddress = vl.a.PerAddress,
                                      prespostcode = vl.a.PresPostCodeId,
                                      presunion = vl.a.PresWard,
                                      presthana = pt.Name,
                                      presdist = pd.Name,
                                      presunions = vl.a.PresUnion,


                                      paddress = vl.a.PresAddress,
                                      perthana = pet.Name,
                                      perdist = ped.Name,
                                      prpostcode = vl.a.PerPostCodeId,
                                      perunion = vl.a.PerWard,
                                      perunions=vl.a.PerUnion,


                                      height = hf.Name + " " + hi.Name,
                                      weight = vl.bs.WeightKg + " কেজি",
                                      zender = g.Name,
                                      cyecolor = ec.Name,
                                      bodycolor = vl.bs.bodycolor,

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
                                  join dig in cdb.Designation.ToList() on vl.pi.DesignationId equals dig.Id
                                  join r in cdb.Religion on vl.pi.ReligionId equals r.Id
                                  join pt in cdb.SubDistric on vl.a.PresSubDistric equals pt.Id
                                  join pd in cdb.Distric on vl.a.PresDistric equals pd.Id
                                  join pet in cdb.SubDistric on vl.a.PerSubDistric equals pet.Id
                                  join ped in cdb.Distric on vl.a.PerDistric equals ped.Id
                                  join hi in cdb.Inch on vl.bs.inchid equals hi.Id
                                  join hf in cdb.Foot on vl.bs.footid equals hf.Id
                                  join g in cdb.Gender on vl.bs.GenderId equals g.Id
                                  join ec in cdb.EyeColor on vl.bs.EyeColorId equals ec.Id
                                  join bg in cdb.BloodGroup on vl.bs.BloodGroupId equals bg.Id
                                  select new PersonalInfoMemberAddressBodyStructureReligion
                                  {
                                      Id = vl.pi.Id,
                                      Imge = vl.i.picture,
                                      membersign = vl.i.ImageofBsign,
                                      platunname = vl.planame,
                                      bname = vl.pi.BanglaName,
                                      ename = vl.pi.EnglishName,
                                      bfname = vl.pi.BanglaFatherName,
                                      efname = vl.pi.EnglishFatherName,
                                      bmname = vl.pi.BanglaMotherName,
                                      emname = vl.pi.EnglishMotherName,
                                      designation = dig.Name,
                                      dob = vl.pi.DOB,
                                      sandadno = vl.pi.dp,
                                      maritalstatus = vl.pi.MaritalStatus,
                                      smname = vl.pi.WORHName,
                                      smpasa = vl.pi.WorHOccupation,
                                      nid = vl.pi.NID,
                                      dobno = vl.pi.DOBSNo,
                                      isactive = vl.m.IsActive,
                                      p = vl.pi.p,
                                      po = vl.pi.po,
                                      pos = vl.pi.pos,
                                      pot = vl.pi.pot,
                                      ps = vl.pi.ps,
                                      pst = vl.pi.pst,
                                      occupation = vl.pi.occupation,
                                      religion = r.Name,


                                      presaddress = vl.a.PerAddress,
                                      prespostcode = vl.a.PresPostCodeId,
                                      presunion = vl.a.PresWard,
                                      presthana = pt.Name,
                                      presdist = pd.Name,
                                      presunions = vl.a.PresUnion,


                                      paddress = vl.a.PresAddress,
                                      perthana = pet.Name,
                                      perdist = ped.Name,
                                      prpostcode = vl.a.PerPostCodeId,
                                      perunion = vl.a.PerWard,
                                      perunions = vl.a.PerUnion,


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
           
           var t  = mrbd.Prasikkan.Where(x => x.MemberId.Equals(id)).Count();
            if(t>0)
            {
                ViewBag.c = "true";
            }
            else
            {
                ViewBag.c="false";
            }
            var em = mrbd.EventMember.Where(x => x.memberId.Equals(id)).Count();
            if(em>0)
            {
                ViewBag.e = "true";
            }
            else
            {
                ViewBag.e = "false";
            }
            var pc = mrbd.MemberPlatunChange.Where(x => x.MemberId.Equals(id)).Count();
            if (pc > 0)
            {
                ViewBag.pc = "true";
            }
            else
            {
                ViewBag.pc = "false";
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Approved(Guid id)
        {
            string idno;
            long lastProductId;
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Id).FirstOrDefault();
            int role = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.roleid).SingleOrDefault();
            if (role == 1)
            {
                var idcount = mrbd.Member.Where(x => x.MemberId.Equals(id)).Select(x => x.IDCardNo).FirstOrDefault();
                if (idcount != null)
                {
                    return RedirectToAction("MemberDetails", new { id = id });
                }
                else
                {
                    // for getting distric code
                    var platunid = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).Select(x => x.platunId).FirstOrDefault();
                    var subdistricid = db.Platun.Where(x => x.Id.Equals(platunid)).Select(x => x.SubDistrcId).FirstOrDefault();
                    var districid = cdb.SubDistric.Where(x => x.Id.Equals(subdistricid)).Select(x => x.countryid).FirstOrDefault();
                    int districcode = cdb.Distric.Where(x => x.Id.Equals(districid)).Select(x => x.code).FirstOrDefault();
                    var numbers = mrbd.Member.Where(x => x.mid.Equals(districcode)).Select(x => x.IDCardNo).ToList();
                    if (numbers.Count() == 0)
                    {
                        int i = 1;
                        idno = Convert.ToString(districcode) + i.ToString("D8");
                    }
                    else
                    {
                        var allnumbers = numbers.Select(Int64.Parse).ToList();
                        lastProductId = allnumbers.Max() + 1;
                        int svar = mrbd.Member.Where(x => x.IDCardNo.Equals(lastProductId.ToString())).Count();
                        if (svar > 0)
                        {
                            long value = lastProductId + 1;
                            idno = value.ToString();
                        }
                        else
                        {
                            idno = lastProductId.ToString();
                        }
                    }

                    // end for getting distric code
                    //  int lastProduct = mrbd.Member.Where(x => !x.IDCardNo.Equals(null) && x.Status.Equals(2) && x.mid.Equals(districcode)).Count();
                    //   int lastProductId = lastProduct + 1;
                    //    string membercode =  lastProductId.ToString();

                    var s = mrbd.Member.Where(x => x.MemberId.Equals(id)).SingleOrDefault();
                    s.ApporovedDate = DateTime.Now.Date;
                    s.platunchangedate = DateTime.Now.Date;
                    s.Status = 2;
                    s.mid = districcode;
                    s.ApprovedBy = userid;
                    s.IDCardNo = idno;
                    mrbd.Entry(s).State = EntityState.Modified;
                    mrbd.SaveChanges();
                    var count = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(2) && x.IsActive.Equals(true)).Count();
                    var counts = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(1) && x.IsActive.Equals(true)).Count();
                    if (count > 0 || counts > 0)
                    {
                        Session["pid"] = id;
                    }
                    else
                    {
                        TempData["comandar"] = "রেঞ্জ ও জোন কমান্ডার অর্ন্তভুক্ত করতে হবে।";
                       

                    }
                    return RedirectToAction("MemberDetails", new { id = id });
                }
               
            }
            else
            {
                var s = mrbd.Member.Where(x => x.MemberId.Equals(id)).SingleOrDefault();
                s.CheckBeforeApproved = userid;
                mrbd.Entry(s).State = EntityState.Modified;
                mrbd.SaveChanges();
                return RedirectToAction("MemberList");
            }
           
           
        }
        [HttpGet]
        public ActionResult RejectedMember(Guid id)
        {
            PersonalInfo pi = mrbd.PersonalInfo.Find(id);
            BAV.Models.Image i = mrbd.Image.Where(x=>x.MemberId.Equals(id)).FirstOrDefault();
            BodyStructure bs = mrbd.BodyStructure.Where(x => x.MemberId.Equals(id)).FirstOrDefault();
            Member m = mrbd.Member.Where(x=>x.MemberId.Equals(id)).FirstOrDefault();
            string volantiarFile = Server.MapPath("~/VolantiarImage//" + i.picture);
            string signimageFile = Server.MapPath("~/MemberSignature//" + i.ImageofBsign);
            string DOBSImageFile = Server.MapPath("~/DOBImage//" + i.DOBSImage);
            string NIDImageFile = Server.MapPath("~/NIDImage//" + i.NIDImage);
            mrbd.PersonalInfo.Remove(pi);
            mrbd.Image.Remove(i);
            mrbd.BodyStructure.Remove(bs);
            mrbd.Member.Remove(m);
            mrbd.SaveChanges();

            if (System.IO.File.Exists(volantiarFile))
            {
                try
                {
                    System.IO.File.Delete(volantiarFile);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }

            }
            if (System.IO.File.Exists(signimageFile))
            {
                try
                {
                    System.IO.File.Delete(signimageFile);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }

            }
            if (System.IO.File.Exists(DOBSImageFile))
            {
                try
                {
                    System.IO.File.Delete(DOBSImageFile);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }

            }
            if (System.IO.File.Exists(NIDImageFile))
            {
                try
                {
                    System.IO.File.Delete(NIDImageFile);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }

            }
            return RedirectToAction("MemberList");
          
        }
        public ActionResult MemberDetailsReport(Guid id)
        {
            TempData["e"] = Session["usertype"];
            string joncom;
                var platunid = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).Select(x => x.platunId).FirstOrDefault();
                var subdistricid = db.Platun.Where(x => x.Id.Equals(platunid)).Select(x => x.SubDistrcId).FirstOrDefault();
                var districid = cdb.SubDistric.Where(x => x.Id.Equals(subdistricid)).Select(x => x.countryid).FirstOrDefault();
                var count = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(2) && x.IsActive.Equals(true)).Count();
                var counts = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(1) && x.IsActive.Equals(true)).Count();
                if (count > 0 || counts > 0)
                {
                    string union = "";
                    string BloodGroupname = "";

                    var BloodGroupId = mrbd.BodyStructure.Where(x => x.MemberId.Equals(id)).Select(x => x.BloodGroupId).SingleOrDefault();
                    var genderid = mrbd.BodyStructure.Where(x => x.MemberId.Equals(id)).Select(x => x.GenderId).SingleOrDefault();
                    var disignationid = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).Select(x => x.DesignationId).FirstOrDefault();
                    var disignationname = cdb.Designation.Where(x => x.Id.Equals(disignationid)).Select(x => x.Name).FirstOrDefault();
                    var districname = cdb.Distric.Where(x => x.Id.Equals(districid)).Select(x => x.Name).FirstOrDefault();
                    /* for getting jon and rang comandar details */
                    if(districid==13 || districid==74)
                    {
                        joncom = "জোন কমান্ডার";
                        
                    }
                    else
                    {
                        joncom = "জেলা কমান্ড্যান্ট";
                    }

                  
                    var joncomandarname = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(1) && x.IsActive.Equals(true)).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    var joncomandar = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(1) && x.IsActive.Equals(true)).Select(x => x.SignatureImage).FirstOrDefault();
                    var joncomandarsignurl = Server.MapPath("~/ComandarSignImage/" + joncomandar);
                    /* for getting jon and rang comandar details end*/

                    union = mrbd.Address.Where(x => x.MemberId.Equals(id)).Select(x => x.PresUnion).FirstOrDefault();
                    if (union == null)
                    {
                        union = "";
                    }
                    else
                    {
                        union = union + ",";
                    }


                    var IDCardNo = mrbd.Member.Where(x => x.MemberId.Equals(id)).Select(x => x.IDCardNo).FirstOrDefault();
                    var Name = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).Select(x => x.BanglaName).SingleOrDefault();
                    var fathername = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).Select(x => x.BanglaFatherName).SingleOrDefault();
                    var mothername = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).Select(x => x.BanglaMotherName).SingleOrDefault();
                    if (BloodGroupId == 0)
                    { BloodGroupname = "নাই"; }
                    else { BloodGroupname = cdb.BloodGroup.Where(x => x.Id.Equals(BloodGroupId)).Select(x => x.Name).SingleOrDefault(); }
                    var platunname = db.Platun.Where(x => x.Id.Equals(platunid)).Select(x => x.PlatuneName).SingleOrDefault();
                    var gender = cdb.Gender.Where(x => x.Id.Equals(genderid)).Select(x => x.Name).SingleOrDefault();


                    var memberlist = from p in mrbd.PersonalInfo.ToList()
                                     join a in mrbd.Address.ToList() on p.Id equals a.MemberId
                                     join i in mrbd.Image.ToList() on p.Id equals i.MemberId
                                     where p.Id.Equals(id)
                                     select new
                                     {
                                         presa = a.PresAddress,
                                         presdis = a.PresDistric,
                                         pressubdis = a.PresSubDistric,
                                         prespost = a.PresPostCodeId,
                                         presward = a.PresWard,
                                         imgurl = Server.MapPath("~/VolantiarImage/" + i.picture)
                                     };
                    var imagelist = from i in memberlist.ToList()
                                    join prd in cdb.Distric.ToList() on i.presdis equals prd.Id
                                    join prsd in cdb.SubDistric.ToList() on i.pressubdis equals prsd.Id

                                    select new
                                    {
                                        imageurl = i.imgurl,
                                        presa = i.presa,
                                        presdis = prd.Name,
                                        prespost = i.prespost,
                                        presward = i.presward,
                                        pressubdis = prsd.Name
                                    };

                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Report"), "volantiarReport.rpt"));
                    rd.SetDataSource(imagelist);

                    rd.SetParameterValue("IDCardNo", IDCardNo.ToString());
                    rd.SetParameterValue("Name", Name.ToString());
                    rd.SetParameterValue("fathername", fathername.ToString());
                    rd.SetParameterValue("mothername", mothername.ToString());
                    rd.SetParameterValue("BloodGroupname", BloodGroupname.ToString());
                    rd.SetParameterValue("platunname", platunname.ToString());
                    rd.SetParameterValue("gender", gender.ToString());
                    rd.SetParameterValue("union", union.ToString());

                    rd.SetParameterValue("joncomandarname", joncomandarname.ToString());
                    rd.SetParameterValue("joncomandarsignurl", joncomandarsignurl.ToString());

                    rd.SetParameterValue("disignationname", disignationname.ToString());
                    rd.SetParameterValue("joncom", joncom.ToString());
                    rd.SetParameterValue("districname", districname.ToString());
                    
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        rd.Dispose();
                        rd.Close();
                        Session["pid"] = null;
                        return File(stream, "application/pdf");
                        
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                else
                {
                    TempData["error"] = "জোন কমান্ডার অর্ন্তভুক্ত করতে হবে।";
                    return RedirectToAction("ComandarCreation", "Common");

                }
        }

        public ActionResult PrintlistMember(Guid idd)
        {
            string joncom;
            TempData["e"] = Session["usertype"];
            var designation = cdb.Designation.ToList();
            var platunname = db.Platun.Where(x=>x.Id.Equals(idd)).Select(x=>x.PlatuneName).FirstOrDefault();
            var subdistricid = db.Platun.Where(x => x.Id.Equals(idd)).Select(x => x.SubDistrcId).FirstOrDefault();
            var districid = cdb.SubDistric.Where(x => x.Id.Equals(subdistricid)).Select(x => x.countryid).FirstOrDefault();
            if (districid == 13 || districid == 74)
            {
                joncom = "জোন অধিনায়ক এর কার্যালয়";

            }
            else
            {
                joncom = "জেলা কমান্ড্যান্ট এর কার্যালয়";
            }
            var memberlist = from pi in mrbd.PersonalInfo.ToList()
                             join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                             join d in designation.ToList() on pi.DesignationId equals d.Id
                             where m.IsActive == false && m.Status.Equals(2) && pi.platunId.Equals(idd)
                             select new
                             {
                                 membername = pi.BanglaName,
                                 designation = d.Name,
                                 membercode = m.IDCardNo,
                                 mobile = pi.mobile,
                                
                             };
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "volantiarListReport.rpt"));
            rd.SetDataSource(memberlist);
            rd.SetParameterValue("platunname", platunname.ToString());
            rd.SetParameterValue("joncom", joncom.ToString());
         
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //}
        /// <summary>
        /// Member Edit start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MemberEdit(Guid id)
        {
            var s = User.Identity.Name;
            var userid = db.User.Where(x => x.UserName.Equals(s)).Select(x => x.Id).SingleOrDefault();
            var platun = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            int disid = Convert.ToInt16(Session["Distrcid"]);
            if (platun > 0)
            {
                ViewBag.platunname = new SelectList(db.Platun.Where(x => x.UserId.Equals(userid) && x.IsActive.Equals(true)), "Id", "PlatuneName");
            }
            else
            {
                var platunlist = from up in db.UserPlatun.ToList()
                                 join st in db.Platun.ToList() on up.PlatunId equals st.Id
                                 where (up.UserId.Equals(userid))
                                 select new
                                 {
                                     Id = st.Id,
                                     PlatuneName = st.PlatuneName
                                 };

                ViewBag.platunname = new SelectList(platunlist.ToList(), "Id", "PlatuneName");
            }
            ViewBag.designationlist = new SelectList(cdb.Designation, "Id", "Name");
            var model = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).SingleOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult MemberEdit(PersonalInfo model)
        {
            var s = mrbd.PersonalInfo.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
            s.BanglaFatherName = model.BanglaFatherName;
            s.BanglaMotherName = model.BanglaMotherName;
            s.BanglaName = model.BanglaName;
            s.DesignationId = model.DesignationId;
            s.DOB = model.DOB;
            s.EnglishFatherName = model.EnglishFatherName;
            s.EnglishMotherName = model.EnglishMotherName;
            s.EnglishName = model.EnglishName;
            s.platunId = model.platunId;
         
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("MemberDetails", new  {id=model.Id });
        }
        [HttpGet]
        public ActionResult MemberDetailEdit(Guid id)
        {
            ViewBag.Religion = new SelectList(cdb.Religion, "Id", "Name");
            ViewBag.exam = new SelectList(cdb.Exam, "Id", "Name");
            ViewBag.dobpasslist = new SelectList(cdb.Type.OrderByDescending(x=>x.Id).ToList(), "Id", "Name");
            var model = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).SingleOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult MemberDetailEdit(PersonalInfo model)
        {
            var s = mrbd.PersonalInfo.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
            s.DOBSNo = model.DOBSNo;
            s.educationalQuId = model.educationalQuId;
            s.faormomobile = model.faormomobile;
            s.MaritalStatus = model.MaritalStatus;
            s.mobile = model.mobile;
            s.NID = model.NID;
            s.occupation = model.occupation;
            s.ReligionId = model.ReligionId;
            s.WORHName = model.WORHName;
            s.WorHOccupation = model.WorHOccupation;
            s.dp = model.dp;
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("MemberDetails", new { id = model.Id });
        }
        [HttpGet]
        public ActionResult AddressEdit(Guid id)
        {
           
            var model = mrbd.Address.Where(x => x.MemberId.Equals(id)).SingleOrDefault();

            ViewBag.presdistrics = new SelectList(cdb.Distric, "Id", "Name", model.PresDistric);
            ViewBag.presSubdistrics = new SelectList(cdb.SubDistric, "Id", "Name", model.PresSubDistric);
            ViewBag.distric = new SelectList(cdb.Distric, "Id", "Name");
            ViewBag.Subdistric = new SelectList(cdb.SubDistric, "Id", "Name");


            if ((model.PresDistric >= 3 && model.PresDistric <= 13) || model.PresDistric == 74)
            {
               
            }
            else
            {
                model.PresPostCodeIdM = model.PresPostCodeId;
                model.PresWardM = model.PresWard;
                model.PresPostCodeId = null;
                model.PresWard = null;
            }
            if ((model.PerDistric >= 3 && model.PerDistric <= 13) || model.PerDistric == 74)
            {

            }
            else
            {
                model.PerPostCodeIdM = model.PerPostCodeId;
                model.PerWardM = model.PerWard;
                model.PerPostCodeId = null;
                model.PerWard = null;
            }
           
            
            return View(model);
        }
        [HttpPost]
        public ActionResult AddressEdit(Address model)
        {

            var s = mrbd.Address.Where(x => x.MemberId.Equals(model.MemberId)).SingleOrDefault();
            s.PerAddress = model.PerAddress;
            s.PerDistric = model.PerDistric;
            int pd = model.PerDistric;
            int ped = model.PresDistric;
            if ((pd >= 3 && pd <= 13) || pd == 74)
            {
                s.PerPostCodeId = model.PerPostCodeId;
                s.PerWard = model.PerWard;
            }
            else
            {
                s.PerPostCodeId = model.PerPostCodeIdM;
                s.PerWard = model.PerWardM;
            }
           
            if ((ped >= 3 && ped <= 13) || ped == 74)
            {

                s.PresPostCodeId = model.PresPostCodeId;
                s.PresWard = model.PresWard;
            }
            else
            {
                s.PresPostCodeId = model.PresPostCodeIdM;
                s.PresWard = model.PresWardM;
            }
          
            s.PerSubDistric = model.PerSubDistric;
          
            s.PresAddress = model.PresAddress;
            s.PresDistric = model.PresDistric;
          
            s.PresSubDistric = model.PresSubDistric;
           
           
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("MemberDetails", new { id = model.MemberId });
        }
        [HttpGet]
        public ActionResult OthersEdit(Guid id)
        {
            ViewBag.eyecolor = new SelectList(cdb.EyeColor, "Id", "Name");
            ViewBag.BloodGroup = new SelectList(cdb.BloodGroup, "Id", "Name");
        
            ViewBag.Foot = new SelectList(cdb.Foot, "Id", "Name");
            ViewBag.Inch = new SelectList(cdb.Inch, "Id", "Name");
            var model = mrbd.BodyStructure.Where(x => x.MemberId.Equals(id)).SingleOrDefault();
            
            return View(model);
        }
        [HttpPost]
        public ActionResult OthersEdit(BodyStructure model)
        {
           
            var s = mrbd.BodyStructure.Where(x => x.MemberId.Equals(model.MemberId)).SingleOrDefault();
            s.inchid = model.inchid;
            s.BloodGroupId = model.BloodGroupId;
            s.bodycolor = model.bodycolor;
            s.EyeColorId = model.EyeColorId;
            s.footid = model.footid;
            s.GenderId = model.GenderId;
            s.HeightMC = model.HeightMC;
            s.SanaktakaranSign = model.SanaktakaranSign;
            s.WeightKg = model.WeightKg;
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("MemberDetails", new { id = model.MemberId });
        }
        [HttpGet]
        public ActionResult disgedit(Guid id)
        {
            string nidimage = mrbd.Image.Where(x => x.MemberId.Equals(id)).Select(x => x.ImageofBsign).FirstOrDefault();
            var model = new BAV.Models.Image();

            model.ImageofBsign = nidimage;
            model.MemberId = id;
            return View(model);
        }
        [HttpPost]
        public ActionResult disgedit(BAV.Models.Image model)
        {
            string picturename = "";

            if (model.ImageofSigned != null && model.ImageofSigned.ContentLength > 0)
            {

                picturename = Guid.NewGuid() + "." + model.ImageofSigned.FileName.Split('.')[1];
                string targetPath = Server.MapPath("~/MemberSignature//" + picturename);
                Stream strm = model.ImageofSigned.InputStream;
                var targetFile = targetPath;

                GenerateThumbnails(0.5, strm, targetFile);

            }
            var s = mrbd.Image.Where(x => x.MemberId.Equals(model.MemberId)).FirstOrDefault();
            s.ImageofBsign = picturename;
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();

            string destinationFile = Server.MapPath("~/MemberSignature//" + model.ImageofBsign);
            if (System.IO.File.Exists(destinationFile))
            {
                try
                {
                    System.IO.File.Delete(destinationFile);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }

            }


            return RedirectToAction("MemberDetails", new { id = model.MemberId });
        }
        [HttpGet]
        public ActionResult Imageedit(Guid id)
        {
            string nidimage = mrbd.Image.Where(x => x.MemberId.Equals(id)).Select(x=>x.picture).FirstOrDefault();
            var model = new BAV.Models.Image();

            model.NIDImage = nidimage;
            model.MemberId = id;
            return View(model);
        }
        [HttpPost]
        public ActionResult Imageedit(BAV.Models.Image model)
        {
            string picturename="";

            if (model.pictures != null && model.pictures.ContentLength > 0)
            {

                picturename = Guid.NewGuid() + "." + model.pictures.FileName.Split('.')[1];
                string targetPath = Server.MapPath("~/VolantiarImage//" + picturename);
                Stream strm = model.pictures.InputStream;
                var targetFile = targetPath;

                GenerateThumbnails(0.5, strm, targetFile);

            }
            var s = mrbd.Image.Where(x => x.MemberId.Equals(model.MemberId)).FirstOrDefault();
            s.picture = picturename;
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();

            string destinationFile = Server.MapPath("~/VolantiarImage//" + model.NIDImage);
            if (System.IO.File.Exists(destinationFile))
            {
                try
                {
                    System.IO.File.Delete(destinationFile);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
               
            }
            
          
            return RedirectToAction("MemberDetails", new { id = model.MemberId });
        }
        [HttpGet]
        public ActionResult QuestionsEdit(Guid id)
        {
            PersonalInfo model = mrbd.PersonalInfo.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult QuestionsEdit(PersonalInfo model)
        {
            var s = mrbd.PersonalInfo.Where(x => x.Id.Equals(model.Id)).FirstOrDefault();
            s.p = model.p;
            s.po = model.po;
            s.pos = model.pos;
            s.pot = model.pot;
            s.ps = model.ps;
            s.pst = model.pst;
            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("MemberDetails", new { id = model.Id });
        }
        /// <summary>
        /// Member Edit End
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult Deactive(Guid id)
        {
            var model = new Member();
            model.MemberId = id;
            var t = mrbd.Prasikkan.Where(x => x.MemberId.Equals(model.MemberId) && x.SNo == null && x.EndDate == null).Count();
            var em = mrbd.EventMember.Where(x => x.memberId.Equals(model.MemberId) && x.IsActive == true).Count();
            if (t > 0)
            {
                TempData["error"] = "Alrady He/She is in Prasikkan.";
              
            }
            else if (em > 0)
            {
                TempData["error"] = "Alrady He/She is in Event.";
              
            }
            else if (t > 0 && em > 0)
            {
                TempData["error"] = "Alrady He/She is in Event and Prasikkan.";
            }
            return View(model);
        
        }
        [HttpPost]
        public ActionResult Deactive(Member model)
        {
            var t = mrbd.Prasikkan.Where(x => x.MemberId.Equals(model.MemberId) && x.SNo==null && x.EndDate==null).Count();
            var em = mrbd.EventMember.Where(x => x.memberId.Equals(model.MemberId) && x.IsActive == true).Count();

            if (t > 0)
            {
                TempData["error"] = "Alrady He/She is in Prasikkan.";
                return RedirectToAction("MemberDetails", new {id=model.MemberId });
            }
           else if(em>0)
            {
                TempData["error"] = "Alrady He/She is in Event.";
                return RedirectToAction("MemberDetails", new { id = model.MemberId });
            }
            else
            {
                var s = mrbd.Member.Where(x => x.MemberId.Equals(model.MemberId)).SingleOrDefault();
                if (s.IsActive == false)
                {
                    s.IsActive = true;
                }
               else
                {
                    s.IsActive = false;
                }
                mrbd.Entry(s).State = EntityState.Modified;
                mrbd.SaveChanges();
                return RedirectToAction("MemberDetails", new { id = model.MemberId });
            }
          
          
        }

        [HttpGet]
        public ActionResult MemberSearch(int? Page_No)
        {
            ViewBag.degisnationlist = new SelectList(cdb.Designation, "Id", "Name");
           
            TempData["e"] = Session["usertype"];
            var educationlist = cdb.Exam.ToList();
            ViewBag.educationlist = new SelectList(educationlist.ToList(), "Id", "Name");
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var count = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            var platun = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            var disignation = cdb.Designation.ToList(); 
            if (platun > 0)
            {
                ViewBag.platunname = new SelectList(db.Platun.Where(x => x.UserId.Equals(userid) && x.IsActive.Equals(true)), "Id", "PlatuneName");
            }
            else
            {
                var platunlist = from up in db.UserPlatun.ToList()
                                 join st in db.Platun.ToList() on up.PlatunId equals st.Id
                                 where (up.UserId.Equals(userid))
                                 select new
                                 {
                                     Id = st.Id,
                                     PlatuneName = st.PlatuneName
                                 };

                ViewBag.platunname = new SelectList(platunlist.ToList(), "Id", "PlatuneName");
            }
            if (count > 0)
            {
                var plalist = db.Platun.Where(x => x.UserId.Equals(userid)).ToList();
              
                var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                    join pla in plalist.ToList() on pi.platunId equals pla.Id
                                    join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                    join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                    join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                    join ed in educationlist.ToList() on pi.educationalQuId equals ed.Id
                                    join dig in disignation.ToList() on pi.DesignationId equals dig.Id
                                    orderby (m.IDCardNo)
                                    where m.Status.Equals(2)&& m.IsActive.Equals(false)
                                    select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi, platunid = pla.Id, planame = pla.PlatuneName, ex = ed, disignation = dig.Name };
                int Size_Of_Page = 1000000;
                int No_Of_Page = (Page_No ?? 1);
                return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                var plalist = from up in db.UserPlatun.ToList()
                              join st in db.Platun.ToList() on up.PlatunId equals st.Id
                              where (up.UserId.Equals(userid))
                              select new
                              {
                                  Id = st.Id,
                                  PlatuneName = st.PlatuneName,
                                  UserId = up.UserId
                              };
                var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                    join pla in plalist.ToList() on pi.platunId equals pla.Id
                                    join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                    join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                    join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                    join ed in educationlist.ToList() on pi.educationalQuId equals ed.Id
                                    join dig in disignation.ToList() on pi.DesignationId equals dig.Id
                                    orderby (m.IDCardNo)
                                    where m.Status.Equals(2) && m.IsActive.Equals(false)
                                    select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi, platunid = pla.Id, planame = pla.PlatuneName, ex = ed, disignation=dig.Name  };
                int Size_Of_Page = 1000000;
                int No_Of_Page = (Page_No ?? 1);
                return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));

            }
        }
        public JsonResult GetArea(string Prefix)
        {

            var adress = new List<Models.Address>();
            adress = mrbd.Address.Where(x => x.PresAddress.Contains(Prefix.ToLower())).ToList();
        

            return Json(adress, JsonRequestBehavior.AllowGet);
        }
        public ActionResult deactivememberlist(int? Page_No)
        {
            TempData["e"] = Session["usertype"];
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();

          
              
   
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var count = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            if (count > 0)
            {

                var plalist = db.Platun.Where(x => x.UserId.Equals(userid)).ToList();
                ViewBag.platunname = new SelectList(plalist.ToList(), "Id", "PlatuneName");
                var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                    join pla in plalist.ToList() on pi.platunId equals pla.Id
                                    join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                    join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                    join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                    orderby (pi.Date)
                                    where m.Status.Equals(2) && m.IsActive == true
                                    select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi, planame = pla.PlatuneName };
                model = volantierlist.ToList();
                int Size_Of_Page = 20;
                int No_Of_Page = (Page_No ?? 1);
                return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                var plalist = from up in db.UserPlatun.ToList()
                              join st in db.Platun.ToList() on up.PlatunId equals st.Id
                              where (up.UserId.Equals(userid))
                              select new
                              {
                                  Id = st.Id,
                                  PlatuneName = st.PlatuneName,
                                  UserId = up.UserId
                              };
                ViewBag.platunname = new SelectList(plalist.ToList(), "Id", "PlatuneName");
                var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                    join pla in plalist.ToList() on pi.platunId equals pla.Id
                                    join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                    join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                    join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                    orderby (pi.Date)
                                    where m.Status.Equals(2) && m.IsActive == true
                                    select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi, planame = pla.PlatuneName };
                model = volantierlist.ToList();
                int Size_Of_Page = 20;
                int No_Of_Page = (Page_No ?? 1);
                return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));

            }
        }
        public ActionResult smssending(int? Page_No)
        {
            TempData["e"] = Session["usertype"];
             List<PlatunList> platunlist = new List<PlatunList>();
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var counst = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            if (counst > 0)
            {
                var plalist = from p in db.Platun.ToList()
                              where p.UserId.Equals(userid)
                              select new PlatunList
                              {
                                  Id = p.Id,
                                  UserId = p.UserId,
                                  PlatunName = p.PlatuneName
                              };
                platunlist = plalist.ToList();
                ViewBag.platunname = new SelectList(db.Platun.Where(x => x.UserId.Equals(userid) && x.IsActive.Equals(true)), "Id", "PlatuneName");
            }
            else
            {
                var plalist = from up in db.UserPlatun.ToList()
                              join st in db.Platun.ToList() on up.PlatunId equals st.Id
                              where (up.UserId.Equals(userid))
                              select new PlatunList
                              {
                                  Id = st.Id,
                                  PlatunName = st.PlatuneName,
                                  UserId = up.UserId
                              };
                platunlist = plalist.ToList();
                ViewBag.platunname = new SelectList(plalist.ToList(), "Id", "PlatunName");
            }
            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist.ToList() on pi.platunId equals pla.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                orderby (pi.Date)
                                where m.Status.Equals(2) && m.IsActive == false
                                select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi, planame= pla.PlatunName };
            int Size_Of_Page = 50000;
            int No_Of_Page = (Page_No ?? 1);
            return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [HttpPost]
        public ActionResult smssending(string[] ids, string pn)
        {
            TempData["e"] = Session["usertype"];
            if (pn == "" || ids == null)
            {
                TempData["error"] = "আপনি সঠিক ইনপুট দেন।";
            }
            else
            {
                Guid[] id = null;
                if (ids != null)
                {
                    id = new Guid[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        Guid.TryParse(i, out id[j++]);
                    }
                }
                if (id != null && id.Length > 0)
                {
                    List<PersonalInfo> pi = new List<PersonalInfo>();
                    pi = mrbd.PersonalInfo.Where(x => id.Contains(x.Id)).ToList();
                    foreach (var i in pi)
                    {
                        message dba = new message();
                        dba.Id = Guid.NewGuid();
                        dba.MemberId = i.Id;
                        dba.messages = pn;
                        dba.date = DateTime.Now;
                        convertBanglaDigitToEnglish(i.mobile);
                        mrbd.message.Add(dba);
                        mrbd.SaveChanges();

                    }

                }
            }
            return RedirectToAction("smssending");
        }
        private int convertBanglaDigitToEnglish(string mobile)
        {
            //string number = "1234567890";
            int parsed_number = 0;
            bool match = Regex.IsMatch(mobile, "^[a-zA-Z0-9]*$");
            //string bengali_text = string.Concat(number.ToString().Select(c => (char)('\u09E6' + c - '0'))); // "??????????"
            if (match==false)
            {
                string english_text = string.Concat(mobile.Select(c => (char)('0' + c - '\u09E6'))); // "1234567890"

                parsed_number = int.Parse(english_text); // 1234567890
            }
            else
            {
                parsed_number = int.Parse(mobile);
            }
            return parsed_number;

        }
        [HttpGet]
        [SessionExpire]
        [AuthorizeRoles("Admin")]
        public ActionResult PlatunChange()
        {
            TempData["e"] = Session["usertype"];
            List<PlatunList> platunlist = new List<PlatunList>();
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var counst = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            if (counst > 0)
            {
                var plalist = from p in db.Platun.ToList()
                              where p.UserId.Equals(userid) && p.IsActive.Equals(true)
                              select new PlatunList
                              {
                                  Id = p.Id,
                                  PlatunName = p.PlatuneName
                              };
                platunlist = plalist.ToList();
                ViewBag.platunlist = new SelectList(platunlist.ToList(), "Id", "PlatunName");
            }
            else
            {
                var plalist = from up in db.UserPlatun.ToList()
                              join st in db.Platun.ToList() on up.PlatunId equals st.Id
                              where (up.UserId.Equals(userid)) && st.IsActive.Equals(true)
                              select new PlatunList
                              {
                                  Id = st.Id,
                                  PlatunName = st.PlatuneName,

                              };
                platunlist = plalist.ToList();
                ViewBag.platunlist = new SelectList(platunlist.ToList(), "Id", "PlatunName");
            }
           

            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist.ToList() on pi.platunId equals pla.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                where m.Status.Equals(2) && m.IsActive == false 
                                select new PersonalInfoMemberAddressBodyStructure
                                {
                                    pi = pi,
                                    m = m,
                                    a = a,
                                    bs = bs,
                                    planame = pla.PlatunName

                                };


            int Size_Of_Page = 50000;
            int No_Of_Page = 1;
            return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));
            
        }
        [HttpPost]
        public ActionResult PlatunChange(Guid[] ids, Guid platunid)
        {
               TempData["e"] = Session["usertype"];

                    foreach (var i in ids)
                    {
                        var  ps = mrbd.PersonalInfo.Where(x=>x.Id.Equals(i)).FirstOrDefault();
                        var m = mrbd.Member.Where(x => x.MemberId.Equals(i)).FirstOrDefault();
                        var cmp = new MemberPlatunChange();
                        cmp.Id = Guid.NewGuid();
                        cmp.PlatunId = ps.platunId;
                        cmp.MemberId = i;
                        cmp.StartDate = m.platunchangedate;
                        cmp.EndDate = DateTime.Now.Date;
                        ps.platunId = platunid;
                        m.platunchangedate = DateTime.Now.Date;
                        mrbd.Entry(ps).State = EntityState.Modified;
                        mrbd.Entry(m).State = EntityState.Modified;
                        mrbd.MemberPlatunChange.Add(cmp);
                        mrbd.SaveChanges();
                    
                    }


                    return RedirectToAction("PlatunChange");
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetThana(string idd)
        {
            int s = int.Parse(idd);
      
            var thana = cdb.SubDistric.Where(x=>x.countryid.Equals(s)).ToList();
           
            SelectList objdata = new SelectList(thana.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult GetWard(string idd)
        {
            int s = int.Parse(idd);

            var ward = cdb.Ward.Where(x => x.SubDisticId.Equals(s)).ToList();

            SelectList objdata = new SelectList(ward.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult GetPostCode(string idd)
        {
            int s = int.Parse(idd);

            var ward = cdb.Ward.Where(x => x.Id.Equals(s)).Select(x=>x.Postid).SingleOrDefault();
            var postcode = cdb.PostCode.Where(x => x.Id.Equals(ward)).ToList();
            SelectList objdata = new SelectList(postcode.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetUnion(string idd)
        {
            int s = int.Parse(idd);

            var Union = cdb.Union.Where(x => x.SubDisId.Equals(s)).ToList();

            SelectList objdata = new SelectList(Union.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetWardbyUnion(string idd)
        {
            int s = int.Parse(idd);

            var ward = cdb.Ward.Where(x => x.UnionId.Equals(s)).ToList();

            SelectList objdata = new SelectList(ward.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult GetWardbyUnionPostcode(string idd)
        {
            int s = int.Parse(idd);

            var postid = cdb.Union.Where(x => x.Id.Equals(s)).Select(x=>x.PostId).SingleOrDefault();
            var postcode = cdb.PostCode.Where(x => x.Id.Equals(postid)).ToList();
            SelectList objdata = new SelectList(postcode.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //image Thumbnails
        private void GenerateThumbnails(double scaleFactor, Stream sourcePath, string targetPath)
        {
            using (var image = System.Drawing.Image.FromStream(sourcePath))
            {
                int newWidth;
                int newHeight;
              
                 newWidth = 512;
                 newHeight = 384;

                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);
                }
              
                thumbnailImg.Save(targetPath, image.RawFormat);
            }
        }
       
        //image Thumbnails end

        [AllowAnonymous]
        [HttpPost]
        public JsonResult platunlist(string id)
        {
            int s = int.Parse(id);

            var platunlist = db.Platun.Where(x => x.SubDistrcId.Equals(s) && x.IsActive.Equals(true)).ToList();

            SelectList objdata = new SelectList(platunlist.ToList(), "Id", "PlatuneName", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
      
	}

    
}