using BAV.Models;
using BAV.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BAV.Controllers
{
   
    public class HomeController : Controller
    {
        public UsersContext db = new UsersContext();
        public CommonContext cdb = new CommonContext();
        public MemberRegistrationContext mrbd = new MemberRegistrationContext();
        //
        // GET: /Home/
       
        public ActionResult Index()
        {
            TempData["e"] = Session["usertype"];
            List<PlatunList> platunlist = new List<PlatunList>();
           
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var counst = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            var username = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x=> x.UserName).SingleOrDefault();
          
            
            ViewBag.MemberCount = mrbd.Member.ToList().Count();
            

            var plavolanti = from p in platunlist.ToList()
                             join ps in mrbd.PersonalInfo.ToList() on p.Id equals ps.platunId
                             join m in mrbd.Member.ToList() on ps.Id equals m.MemberId
                             where m.Status.Equals(2)
                             group ps by new { ps.platunId,p.PlatunName } into g
                             select new PlatunList
                             {
                                 PlatunName = g.Key.PlatunName,
                                 total = g.Count(),

                                 
                             };
            ViewBag.volantiallist = plavolanti.ToList();

            /* for prsikkan list */

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

            ViewBag.prasikkan = prasikkanlist.OrderBy(x => x.CreationDate).ToList();
            ViewBag.TrainingCount = prasikkanlist.Count();


            var Platunlist = (from p in db.Platun.ToList()
                                 where p.UserId.Equals(adminid)
                              select new PlatunList
                                 {
                                     Id = p.Id,
                                     PlatunName = p.PlatuneName
                                 })
                              .Union
                              (from p in db.Platun.ToList()
                               join u in user.ToList() on p.UserId equals u.Id
                               select new PlatunList
                               {
                                   Id = p.Id,
                                   PlatunName = p.PlatuneName
                               });

            ViewBag.PlatunCount = Platunlist.Count();


            List<Distric> dislist = new List<Distric>();

            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new Distric
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };
            dislist = districtlist.ToList();
            ViewBag.DistrictList = districtlist.ToList();
            ViewBag.DistrictCount = districtlist.Count();
            /* for prsikkan list end */


            return View();
        }
       
        public ActionResult pdfdownload()
        {
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=UserGuide.pdf");

            // Write the file to the Response
            const int bufferLength = 10000;
            byte[] buffer = new Byte[bufferLength];
            int length = 0;
            Stream download = null;
            try
            {
                download = new FileStream(Server.MapPath("~/Download/ansar.doc.pdf"),FileMode.Open, FileAccess.Read);
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
            return View();
        }

        // convert bangla date to english .......
        private string convertbanglatoeng(string bangla)
        {
            string english_text = string.Concat(bangla.Select(c => (char)('0' + c - '\u09E6'))); 
            english_text= english_text.Replace('', '-').Trim();
            return english_text;
        }

        [HttpGet]
        public JsonResult getEvents()
        {
          
            //List<Event> eventlist = new List<Event>();
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
            var eventslist = (from e in mrbd.Event.ToList()
                              where e.UserId.Equals(adminid)
                              select new Event
                              {
                                  Id = e.Id,
                                  UserId = e.UserId,
                                  Area = e.Area,
                                  EventName = e.EventName,
                                  Date = convertbanglatoeng(e.Date),
                                  enddate =convertbanglatoeng(e.enddate),
                                  IsActive = e.IsActive
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
                                     Date =convertbanglatoeng( e.Date),
                                     enddate= convertbanglatoeng(e.enddate),
                                     IsActive = e.IsActive
                                 });
            
            var eventlist = eventslist.ToArray();
            
            
            return Json(eventlist, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DistrictList()
        {
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            List<Distric> dislist = new List<Distric>();

            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new Distric
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };

            dislist = districtlist.ToList();
            ViewBag.DistrictList = districtlist.ToList();

            return View();
        }

        public ActionResult ThanaList(int id)
        {
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            List<SubDistric> dislist = new List<SubDistric>();

            var SubDistrictlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.SubDistric.ToList() on rd.DistrictId equals d.countryid
                               where d.countryid.Equals(id)
                               select new SubDistric
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };


            ViewBag.SubDistrictList = SubDistrictlist.ToList();


            //var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            //var subdistricidlist = from up in db.UserPlatun.ToList()
            //                       join st in db.Platun.ToList() on up.PlatunId equals st.Id
            //                       where (up.UserId.Equals(userid))
            //                       group st by st.SubDistrcId into g
            //                       select new
            //                       {
            //                           districid = g.Key,

            //                       };
            //var subdistriclist = from sdi in subdistricidlist.ToList()
            //                     join sd in cdb.SubDistric.ToList() on sdi.districid equals sd.Id
            //                     select new
            //                     {
            //                         Id = sd.Id,
            //                         Name = sd.Name
            //                     };

            
            return View();
        }



	}
}