using BAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Data.Entity;
using BAV.Security;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BAV.Controllers
{
    //[SessionExpire]
    public class CommonController : Controller
    {
        public CommonContext cdb = new CommonContext();
        public  MemberRegistrationContext mrbd = new MemberRegistrationContext();
        public UsersContext db = new UsersContext();
        //
        // GET: /Common/
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles("Admin")]
        public ActionResult Prasikkan()
        {
            TempData["e"] = Session["usertype"];
         
            TempData["Prasikkan"] = "List";
            
            return View();
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult Prasikkan(PrasikkanName model)
        {
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Id).SingleOrDefault();
            model.CreationDate = DateTime.Now.Date;
            model.UserId = userid;
            cdb.PrasikkanName.Add(model);
            cdb.SaveChanges();
            TempData["Prasikkan"] = "List";
            return RedirectToAction("Prasikkan");
        }
        [AuthorizeRoles("Admin")]
        public ActionResult PrasikkanNameList(int? Page_No)
        {
            List<PrasikkanName> prasikkan = new List<PrasikkanName>();
            TempData["e"] = Session["usertype"];
            var Createdby = "";
            var adminid = Guid.Empty;
          
            var count = db.User.Where(x => x.Createdby.ToLower().Equals(User.Identity.Name.ToLower())).Count();

            if (count > 0)
            {
                Createdby = User.Identity.Name;
                adminid = db.User.Where(x => x.UserName.ToLower().Equals(Createdby.ToLower())).Select(x => x.Id).SingleOrDefault();
            }
            else
            {
                Createdby = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Createdby).FirstOrDefault();
                adminid = db.User.Where(x => x.UserName.ToLower().Equals(Createdby.ToLower())).Select(x => x.Id).SingleOrDefault();

            }
            var user = db.User.Where(x => x.Createdby.ToLower().Equals(Createdby.ToLower())).ToList();
            var prasikkanlist = (from p in cdb.PrasikkanName.ToList()
                                 where p.UserId.Equals(adminid)
                                 select new PrasikkanName
                                 {
                                     Id = p.Id,
                                     UserId = p.UserId,
                                     CreationDate = p.CreationDate,
                                     Name = p.Name
                                 })
                                .Union
                                (from p in cdb.PrasikkanName.ToList()
                                 join u in user.ToList() on p.UserId equals u.Id
                                 select new PrasikkanName
                                 {
                                     Id = p.Id,
                                     UserId = p.UserId,
                                     CreationDate = p.CreationDate,
                                     Name = p.Name
                                 });
            prasikkan = prasikkanlist.ToList();        

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(prasikkan.ToPagedList(No_Of_Page, Size_Of_Page));
        }
         [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult CreateEvent()
        {
            TempData["e"] = Session["usertype"];
            var s = 0;
            if (s == 1)
            {
                TempData["r"] = "true";
            }
            else
            {
                TempData["r"] = "false";
            }
            TempData["event"] = "List";
            return View();
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult CreateEvent(Event model)
        {
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Id).FirstOrDefault();
            if(ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.IsActive = true;
                model.UserId = userid;
                if (model.enddate == null)
                {
                    model.enddate = model.Date;
                }
                mrbd.Event.Add(model);
                mrbd.SaveChanges();
                TempData["event"] = "List";
            }
            return RedirectToAction("CreateEvent");
        }

        [AuthorizeRoles("Admin")]
        public ActionResult EventList(int? Page_No)
        {
            TempData["e"] = Session["usertype"];
            var Createdby = "";
            var adminid = Guid.Empty;
            List<Event> eventlist = new List<Event>();
            
            var count = db.User.Where(x => x.Createdby.ToLower().Equals(User.Identity.Name.ToLower())).Count();

            if (count > 0)
            {
                Createdby = User.Identity.Name;
                adminid = db.User.Where(x => x.UserName.ToLower().Equals(Createdby.ToLower())).Select(x => x.Id).SingleOrDefault();
            }
            else
            {
                Createdby = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Createdby).FirstOrDefault();
                adminid = db.User.Where(x => x.UserName.ToLower().Equals(Createdby.ToLower())).Select(x => x.Id).SingleOrDefault();

            }
            var user = db.User.Where(x => x.Createdby.ToLower().Equals(Createdby.ToLower())).ToList();
            var eventslist = (from e in mrbd.Event.ToList()
                                 where e.UserId.Equals(adminid)
                              select new Event
                                 {
                                     Id = e.Id,
                                     UserId = e.UserId,
                                     Area=e.Area,
                                      EventName=e.EventName,
                                       Date=e.Date,
                                       enddate= e.enddate,
                                       IsActive=e.IsActive
                                 })
                                .Union
                                (from e in mrbd.Event.ToList()
                                 join u in user.ToList() on e.UserId equals u.Id
                                 select new Event
                                 {
                                     Id = e.Id,
                                     UserId = e.UserId,
                                     Area = e.Area,
                                     EventName = e.EventName,
                                     Date = e.Date,
                                     enddate = e.enddate,
                                     IsActive = e.IsActive
                                 });
            eventlist = eventslist.ToList();

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(eventlist.ToPagedList(No_Of_Page, Size_Of_Page));
            
        }
        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult EventEdit(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = new Event();
            var s = mrbd.Event.Where(x => x.Id.Equals(id)).SingleOrDefault();
            model.Id = s.Id;
            model.Area = s.Area;
            model.Date = s.Date;
            model.IsActive = s.IsActive;
            model.EventName = s.EventName;
            return View(model);
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult EventEdit(Event model)
        {
            TempData["e"] = Session["usertype"];
           var s = mrbd.Event.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
           s.Id = model.Id;
           s.Area= model.Area;
           s.Date= model.Date;
           s.IsActive= model.IsActive;
           s.enddate = model.enddate;
           s.EventName= model.EventName;
           mrbd.Entry(s).State = EntityState.Modified;
            if(model.IsActive==false)
            {
                var eventmemberlist = mrbd.EventMember.Where(x => x.eventId.Equals(model.Id) && x.IsActive.Equals(true)).ToList();
                foreach( var i in eventmemberlist)
                {
                    var evm = new EventMember();
                    evm = mrbd.EventMember.Where(x => x.Id.Equals(i.Id)).SingleOrDefault();
                    evm.endtime = DateTime.Now.Date;
                    evm.IsActive = false;
                 
                     if(evm.workfor=="m")
                     {
                         var mbm = new MonthBasedMember();
                         mbm = mrbd.MonthBasedMember.Where(x => x.EventMemberId.Equals(i.Id)).SingleOrDefault();
                         mbm.dutyend = DateTime.Now.Date;
                         mrbd.Entry(mbm).State = EntityState.Modified;
                     }
                     else if (evm.workfor == "h")
                     {
                         var hbm = new HourlyBasedMember();
                         hbm = mrbd.HourlyBasedMember.Where(x => x.EventMemberId.Equals(i.Id)).SingleOrDefault();
                         hbm.Enddate = DateTime.Now.Date;
                         mrbd.Entry(hbm).State = EntityState.Modified;
                     }
                     else if (evm.workfor == "c")
                     {
                         var cbm = new ContactBasedMember();
                         cbm = mrbd.ContactBasedMember.Where(x => x.EventMemberId.Equals(i.Id)).SingleOrDefault();
                         cbm.EndDate = DateTime.Now.Date;
                         mrbd.Entry(cbm).State = EntityState.Modified;
                     }
                     mrbd.Entry(evm).State = EntityState.Modified;
                }
            }

           mrbd.SaveChanges();
          
           return RedirectToAction("CreateEvent");
        }
        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult ComandarCreation()
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
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "জেলা কমান্ডার", Value = "1" });
            
            //items.Add(new SelectListItem { Text = "রেঞ্জ কমান্ডার", Value = "2" });

            ViewBag.comandar = items;

            TempData["comandarlist"] = "List";
            return View();
        }
        [HttpPost]
        public ActionResult ComandarCreation(ComandarSpecification model)
        {
                string picname = "";
                var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
                var count = mrbd.ComandarSpecification.Where(x => x.comandartype.Equals(model.comandartype) && x.IsActive.Equals(true)&& x.DistricId.Equals(model.DistricId)).Count();
                if (count > 0)
                {
                    TempData["error"] = "ডাটাবেজে অর্ন্তভুক্ত আছে।";
                }
                else
                {
                    if (model.signimage != null && model.signimage.ContentLength > 0)
                    {

                        picname = Guid.NewGuid() + "." + model.signimage.FileName.Split('.')[1];
                        string targetPath = Server.MapPath("../ComandarSignImage//" + picname);
                        Stream strm = model.signimage.InputStream;
                        var targetFile = targetPath;

                        GenerateThumbnails(strm, targetFile);

                    }
                    model.SignatureImage = picname;
                    model.CreationDate = DateTime.Now.Date;
                    model.IsActive = true;
                    model.UserId = userid;
                    mrbd.ComandarSpecification.Add(model);
                    mrbd.SaveChanges();
                    TempData["error"] = "ডাটাবেজে অর্ন্তভুক্ত হয়েছে।";
                    TempData["comandarlist"] = "List";
                }

                return RedirectToAction("ComandarCreation");
        }
        [AuthorizeRoles("Admin")]
        public ActionResult ComandarList()
        {
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var districid = db.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.DistricId).FirstOrDefault();
            var comandarlist = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid)).ToList();
            return View(comandarlist.ToList());
        }
        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult InactiveComandar(int id)
        {
            var model = mrbd.ComandarSpecification.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(model);
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult InactiveComandar(ComandarSpecification model)
        {
            var s = mrbd.ComandarSpecification.Where(x => x.Id.Equals(model.Id)).FirstOrDefault();
            if (s.IsActive == true)
            {
                s.IsActive = false;
                s.InactiveRemarks = model.InactiveRemarks;
            }
            else
            {
                s.IsActive = true;

            }

            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("ComandarCreation");
         
        }


        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult PlatunCreation()
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
            TempData["platunlist"] = "List";
            return View();
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult PlatunCreation(Platun model)
        {
            if(ModelState.IsValid)
            {
                var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
                model.Id = Guid.NewGuid();
                model.CreationDate = DateTime.Now.Date;
                model.IsActive = true;
                model.UserId = userid;
                db.Platun.Add(model);
                db.SaveChanges();
                TempData["platunlist"] = "List";
            }
            return RedirectToAction("PlatunCreation");
        }
        [AuthorizeRoles("Admin")]
        public ActionResult PlatunList(int? Page_No)
        {
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();

            var platun = from p in db.Platun.ToList()
                         join ur in db.UserRole.ToList() on p.UserId equals ur.UserId
                         where p.UserId.Equals(userid)
                         select new
                         {
                             p.PlatuneName,
                             p.SubDistrcId,
                             ur.DistricId,
                             p.CreationDate,
                             p.Id,
                             p.IsActive
                         };
            var platundis = from p in platun.ToList()
                            join d in cdb.Distric.ToList() on p.DistricId equals d.Id
                            join ur in cdb.SubDistric.ToList() on p.SubDistrcId equals ur.Id
                            select new PlaDistric
                            {
                                PlatuneName = p.PlatuneName,
                                creationdte=p.CreationDate,
                                disnane=d.Name,
                                subname=ur.Name ,
                                PlatunId=p.Id,
                                Isactive=p.IsActive
                            };
       
            return View(platundis.ToList());
        }
        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult InactivePlatun(Guid id)
        {
            var model = db.Platun.Where(x => x.Id.Equals(id)).SingleOrDefault();
            if (model.IsActive == false)
            {
                return View(model);
            }
            else
            {
                var memberlist = from p in mrbd.PersonalInfo.ToList()
                                 join m in mrbd.Member.ToList() on p.Id equals m.MemberId
                                 where p.platunId.Equals(id) && m.IsActive.Equals(false)
                                 select new
                                 {
                                     m.MemberId
                                 };

                var count = memberlist.ToList().Count();
                if (count > 0)
                {
                    ViewBag.message = 1;
                    return View();
                }
                else
                {
                    return View(model);
                }
            }
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult InactivePlatun(Platun model)
        {
            var s = db.Platun.Where(x=>x.Id.Equals(model.Id)).SingleOrDefault();
            if (s.IsActive == true)
            {
                s.InActiveDate = DateTime.Now.Date;
                s.IsActive = false;
                s.InactiveRemarks = model.InactiveRemarks;
            }
            else
            {
                s.IsActive = true;

            }

            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PlatunCreation");
        }
        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult EditPlatun(Guid id)
        {
            var model = db.Platun.Where(x => x.Id.Equals(id)).SingleOrDefault();
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var ud = db.UserDistic.Where(x => x.UserId.Equals(userid)).ToList();
            
            TempData["presd"] = Session["Distrcid"];
            var pressubdis = from sd in cdb.SubDistric.ToList()
                             where (from g in ud.ToList() select g.SubdisticId).Contains(sd.Id)
                             select new
                             {
                                 Id = sd.Id,
                                 Name = sd.Name
                             };
            ViewBag.presSubdistrics = new SelectList(pressubdis.ToList(), "Id", "Name",model.SubDistrcId);
            return View(model);
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult EditPlatun(Platun model)
        {
            var s = db.Platun.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
            s.PlatuneName = model.PlatuneName;
            s.SubDistrcId = model.SubDistrcId;
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PlatunCreation");
        }



        //image Thumbnails double scaleFactor, 
        private void GenerateThumbnails(Stream sourcePath, string targetPath)
        {
            using (var image = System.Drawing.Image.FromStream(sourcePath))
            {
                int newWidth;
                int newHeight;

                newWidth = 312;
                newHeight = 184;

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
	}
}