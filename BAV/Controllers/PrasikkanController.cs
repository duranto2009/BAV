using BAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using BAV.Security;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace BAV.Controllers
{
    //[SessionExpire]
    public class PrasikkanController : Controller
    {
        public CommonContext cdb = new CommonContext();
        public MemberRegistrationContext mrbd = new MemberRegistrationContext();
        public UsersContext db = new UsersContext();
        string er;
        //
        // GET: /Prasikkan/
        public ActionResult Index(Guid id)
        {
            return View();
        }
        [HttpGet]
        public ActionResult Prasikkan(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = new Prasikkan();
            var s = mrbd.Prasikkan.Where(x => x.MemberId.Equals(id)).SingleOrDefault();
            if (s == null)
            {
                model.MemberId = id;
                model.Id =Guid.Empty;
            }
            else
            {
                model.Id = s.Id;
                model.MemberId = id;
                model.PraNameId = s.PraNameId;
                model.PraInstitudeName = s.PraInstitudeName;
                model.StartDate = s.StartDate;
                model.EndDate = s.EndDate;
                model.SNo=s.SNo;
            }
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

            ViewBag.prasikkan = new SelectList(prasikkanlist.ToList(), "Id", "Name");
            /* for prsikkan list end */
            return View(model);
        }
        [HttpPost]
        public ActionResult Prasikkan(Prasikkan model)
        {
        TempData["e"] = Session["usertype"];
        var sno=mrbd.Prasikkan.Where(x => x.SNo.Equals(model.SNo)).Count();
        if (sno > 0)
        {
            ViewBag.error = "This snad no alrady exit.";
        }
        else
        {
            var count=mrbd.Prasikkan.Where(x => x.PraNameId.Equals(model.PraNameId) && x.MemberId.Equals(model.MemberId)).Count();
            if (count > 0)
            {
                var s = mrbd.Prasikkan.Where(x => x.PraNameId.Equals(model.PraNameId) && x.MemberId.Equals(model.MemberId)).FirstOrDefault();
                s.EndDate = model.EndDate;
                s.IsActive = false;
                s.SNo = model.SNo;
                mrbd.Entry(s).State = EntityState.Modified;
            }
            else
            {
                model.Id = Guid.NewGuid();

                mrbd.Prasikkan.Add(model);
            }


            mrbd.SaveChanges();
        }
              return RedirectToAction("MemberList","Member");
        }
        [HttpGet]
        public ActionResult PrasikkanUpdate(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = new Prasikkan();
            var s = mrbd.Prasikkan.Where(x => x.Id.Equals(id)).SingleOrDefault();
          
                model.Id = id;
                model.MemberId = s.MemberId;
                model.PraNameId = s.PraNameId;
                model.PraName = cdb.PrasikkanName.Where(x => x.Id.Equals(s.PraNameId)).Select(x => x.Name).FirstOrDefault();
                model.PraInstitudeName = s.PraInstitudeName;
                model.StartDate = s.StartDate;
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

                ViewBag.prasikkan = new SelectList(prasikkanlist.ToList(), "Id", "Name");
                /* for prsikkan list end */
         
            return View(model);
        }
        [HttpPost]
        public ActionResult PrasikkanUpdate(Prasikkan model)
        {
            TempData["e"] = Session["usertype"];
            var s = mrbd.Prasikkan.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
            var sno=mrbd.Prasikkan.Where(x => x.SNo.Equals(model.SNo)).Count();
        if (sno > 0)
        {
            er = "1";
        }
        else
        {
            s.EndDate = model.EndDate;
            s.IsActive = false;
            s.SNo = model.SNo;
            mrbd.Entry(s).State = EntityState.Modified;

            mrbd.SaveChanges();
            return RedirectToAction("SanadPrint", "Prasikkan", new { Id=model.Id});
        }
        return RedirectToAction("PrasikkanSearch", "Prasikkan", new { Page_No = "", e = er });
     
        }
        public ActionResult SanadPrint(Guid Id)
        {
            string jelncom;
            string joncom;
            var memberid = mrbd.Prasikkan.Where(x=>x.Id.Equals(Id)).ToList().Select(x => x.MemberId).FirstOrDefault();
            var platunid = mrbd.PersonalInfo.Where(x => x.Id.Equals(memberid)).Select(x => x.platunId).FirstOrDefault();
            var subdistricid = db.Platun.Where(x => x.Id.Equals(platunid)).Select(x => x.SubDistrcId).FirstOrDefault();
            var districid = cdb.SubDistric.Where(x => x.Id.Equals(subdistricid)).Select(x => x.countryid).FirstOrDefault();
            var districname = cdb.Distric.Where(x => x.Id.Equals(districid)).Select(x=>x.Name).FirstOrDefault();
            var joncomandarname = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(districid) && x.comandartype.Equals(1) && x.IsActive.Equals(true)).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
            if (districid == 13 || districid == 74)
            {
                jelncom = "জোন অধিনায়ক এর কার্যালয়";
                joncom = "জোন কমান্ডার";
            }
            else
            {
                jelncom = "জেলা কমান্ড্যান্ট এর কার্যালয়";
                joncom = "জেলা কমান্ড্যান্ট";
            }
         
            var memberPrasikkanlist = from p in mrbd.Prasikkan.ToList()
                                      join pn in cdb.PrasikkanName.ToList() on p.PraNameId equals pn.Id
                                      join pi in mrbd.PersonalInfo.ToList() on p.MemberId equals pi.Id
                                      join pa in mrbd.Address.ToList() on pi.Id equals pa.MemberId
                                      join dis in cdb.Distric.ToList() on pa.PresDistric equals dis.Id
                                      join sd in cdb.SubDistric.ToList() on dis.Id equals sd.countryid
                                      where p.Id.Equals(Id)
                                      select new {
                                           memname=pi.BanglaName,
                                           mfname= pi.BanglaFatherName,
                                           bmn= pi.BanglaMotherName,
                                           pname= pn.Name,
                                           sd=p.StartDate,
                                           ed=p.EndDate,
                                           sn= p.SNo,
                                           pw= pa.PresWard,
                                           pu= pa.PresUnion,
                                           ppc= pa.PresPostCodeId,
                                           sdname= sd.Name,
                                           dname=dis.Name
                                           
                                      };
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "Pratayanpatra.rpt"));
            rd.SetDataSource(memberPrasikkanlist);
            rd.SetParameterValue("joncom", joncom.ToString());
            rd.SetParameterValue("districname", districname.ToString());
            rd.SetParameterValue("jelncom", jelncom.ToString());
            rd.SetParameterValue("joncomandarname", joncomandarname.ToString());
            
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
        public ActionResult PrasikkanMember()
        {
            var Createdby = "";
            var adminid = Guid.Empty ;
            TempData["e"] = Session["usertype"];
            List<PrasikkanName> prasikkan = new List<PrasikkanName>();
           
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
                                         Name = p.Name
                                     })
                                    .Union
                                    (from p in cdb.PrasikkanName.ToList()
                                     join u in user.ToList() on p.UserId equals u.Id
                                     select new PrasikkanName
                                     {
                                         Id = p.Id,
                                         Name = p.Name
                                     });
                ViewBag.prasikkan = new SelectList(prasikkanlist.ToList(), "Id", "Name");

                return View();
         
        }
        [HttpPost]
        public ActionResult PrasikkanMember(string[] ids, string prasikkan,string pn,string sdate)
        {
            TempData["e"] = Session["usertype"];
            if (prasikkan == "" || ids == null || pn == "" || sdate == "")
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
                        Prasikkan dba = new Prasikkan();
                        dba.Id = Guid.NewGuid();
                        dba.MemberId = i.Id;
                        dba.IsActive = true;
                        dba.PraNameId =int.Parse(prasikkan);
                        dba.PraInstitudeName = pn;
                        dba.StartDate = sdate;

                        mrbd.Prasikkan.Add(dba);
                        mrbd.SaveChanges();

                    }

                }
            }

            return RedirectToAction("PrasikkanMember");
        }
        public ActionResult MemberPrasikkanDetailsList( int pid)
        {
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

            }

            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist.ToList() on pi.platunId equals pla.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                where m.Status.Equals(2) && m.IsActive == false && !(from g in mrbd.Prasikkan where g.PraNameId.Equals(pid) select g.MemberId).Contains(pi.Id)
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
        public ActionResult PrasikkanSearch(int? Page_No,string e)
        {
            var Createdby = "";
            var adminid = Guid.Empty;
            TempData["e"] = Session["usertype"];
            if (!String.IsNullOrEmpty(e))
            {
                ViewBag.error = "This sanad no alrady exit.";
            }
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
            List<PrasikkanName> prasikkan = new List<PrasikkanName>();

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
                                     Name = p.Name
                                 })
                                .Union
                                (from p in cdb.PrasikkanName.ToList()
                                 join u in user.ToList() on p.UserId equals u.Id
                                 select new PrasikkanName
                                 {
                                     Id = p.Id,
                                     Name = p.Name
                                 });
            ViewBag.prasikkan = new SelectList(prasikkanlist.ToList(), "Id", "Name");

            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist.ToList() on pi.platunId equals pla.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                join p in mrbd.Prasikkan on pi.Id equals p.MemberId
                                where m.Status.Equals(2) && m.IsActive == false && p.IsActive == true && p.SNo == null
                                select new PersonalInfoMemberAddressBodyStructure
                                {
                                    pi = pi,
                                    m = m,
                                    a = a,
                                    bs = bs,
                                    platunid=pla.Id,
                                    planame = pla.PlatunName,
                                    p = p
                                };
         
       
            int Size_Of_Page = 50000;
            int No_Of_Page = (Page_No ?? 1);
            return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));

       
         
        }
        public ActionResult PrasikkanSearchList()
        {
            var Createdby = "";
            var adminid = Guid.Empty;
            TempData["e"] = Session["usertype"];
            List<PlatunList> platunlist = new List<PlatunList>();
            List<PrasikkanName> prasikkan = new List<PrasikkanName>();
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
                                     Name = p.Name
                                 })
                                .Union
                                (from p in cdb.PrasikkanName.ToList()
                                 join u in user.ToList() on p.UserId equals u.Id
                                 select new PrasikkanName
                                 {
                                     Id = p.Id,
                                     Name = p.Name
                                 });
            ViewBag.prasikkan = new SelectList(prasikkanlist.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult PrasikkanSearchList(int prasikkanid, Guid Platun)
        {
            var Createdby = "";
            var adminid = Guid.Empty;
            TempData["e"] = Session["usertype"];
            List<PlatunList> platunlist = new List<PlatunList>();
            List<PrasikkanName> prasikkan = new List<PrasikkanName>();
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
                                     Name = p.Name
                                 })
                                .Union
                                (from p in cdb.PrasikkanName.ToList()
                                 join u in user.ToList() on p.UserId equals u.Id
                                 select new PrasikkanName
                                 {
                                     Id = p.Id,
                                     Name = p.Name
                                 });
            ViewBag.prasikkan = new SelectList(prasikkanlist.ToList(), "Id", "Name");
            if (prasikkanid >0 && Platun != null)
            {
                TempData["list"] = "List";
                
            }
            else
            {
                TempData["list"] = null;
            }
            return View();
        }
        public ActionResult PrasikkanMemberreport(int prasikkanid, Guid Platun)
        {
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
                                join p in mrbd.Prasikkan on pi.Id equals p.MemberId
                                where m.Status.Equals(2) && m.IsActive == false 
                                       && p.PraNameId.Equals(prasikkanid) && pi.platunId.Equals(Platun) 
                                select new PersonalInfoMemberAddressBodyStructure
                                {
                                    pi = pi,
                                    m = m,
                                    a = a,
                                    bs = bs,
                                    platunid = pla.Id,
                                    planame = pla.PlatunName,
                                    p = p
                                };
            ViewBag.prasikkanid = prasikkanid;
            ViewBag.platunid = Platun;
            return View(volantierlist.ToList());
        }
        public ActionResult PrasikkanMemberSearchreportPrint(int prasikkanid,Guid Platun)
        {
            string joncom;
            var platunname = db.Platun.Where(x => x.Id.Equals(Platun)).Select(x => x.PlatuneName).FirstOrDefault();
            var prasikkanname = cdb.PrasikkanName.Where(x => x.Id.Equals(prasikkanid)).Select(x => x.Name).FirstOrDefault();
            var subdistricid = db.Platun.Where(x => x.Id.Equals(Platun)).Select(x => x.SubDistrcId).FirstOrDefault();
            var districid = cdb.SubDistric.Where(x => x.Id.Equals(subdistricid)).Select(x => x.countryid).FirstOrDefault();
            if (districid == 13 || districid == 74)
            {
                joncom = "জোন অধিনায়ক এর কার্যালয়";

            }
            else
            {
                joncom = "জেলা কমান্ড্যান্ট এর কার্যালয়";
            }
            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId 
                                join p in mrbd.Prasikkan on pi.Id equals p.MemberId
                                where m.Status.Equals(2) && m.IsActive == false
                                      && p.PraNameId.Equals(prasikkanid) && pi.platunId.Equals(Platun)
                                select new 
                                {
                                    memberno=m.IDCardNo,
                                    membername=pi.BanglaName,
                                    mobile=pi.mobile,
                                    address=a.PresAddress+" "+a.PresPostCodeId+" "+a.PresWard+" "+a.PresUnion
                                   
                                };
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "volantiarprasikkanReport.rpt"));
            rd.SetDataSource(volantierlist);

            rd.SetParameterValue("platunname", platunname.ToString());
            rd.SetParameterValue("prasikkanname", prasikkanname.ToString());
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
       //&& p.IsActive == true && p.SNo == null
	}
}