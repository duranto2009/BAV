using BAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BAV.Security;
using System.Data.Entity;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace BAV.Controllers
{
    public class TranningController : Controller
    {
        public UsersContext db = new UsersContext();
        public CommonContext cdb = new CommonContext();
        public MemberRegistrationContext mrbd = new MemberRegistrationContext();
       

        [HttpGet]
        public ActionResult Personalinfo()
        {
            var s = User.Identity.Name;
            var userid = db.User.Where(x => x.UserName.Equals(s)).Select(x => x.Id).SingleOrDefault();
            var platun = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            int disid = Convert.ToInt16(Session["Distrcid"]);
            TempData["e"] = Session["usertype"];
            ViewBag.exam = new SelectList(cdb.Exam, "Id", "Name");
            ViewBag.designationlist = new SelectList(cdb.Designation.Where(x => x.Id.Equals(15) || x.Id.Equals(16) || x.Id.Equals(17)), "Id", "Name");


            if (platun > 0)
            {

                ViewBag.subdistriclist = new SelectList(cdb.SubDistric.Where(x => x.countryid.Equals(disid)), "Id", "Name");
            }
            else
            {
                var subdistriclist = distictlist(userid).ToList();
                ViewBag.subdistriclist = new SelectList(subdistriclist.ToList(), "Id", "Name");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Personalinfo(AnsarInfo model)
        {

            var m = mrbd.AnsarInfo.Where(x => x.mobile.Equals(model.mobile)).Count();
            var mid = mrbd.AnsarInfo.Where(x => x.personno.Equals(model.personno)).Count();
            if (m > 0 || mid > 0)
            {
                TempData["eror"] = "নিবন্ধনভুক্ত সদস্য";
                return RedirectToAction("Personalinfo");
            }
            else
            {
                TempData["e"] = Session["usertype"];
                Session["Ansarinfo"] = model;
                return RedirectToAction("AddressInfo");
            }
        }

        [HttpGet]
        public ActionResult AddressInfo()
        {

            TempData["e"] = Session["usertype"];
            if (Session["Ansarinfo"] == null)
            {
                return RedirectToAction("Personalinfo");
            }

            ViewBag.presdistrics = new SelectList(cdb.Distric, "Id", "Name");
            ViewBag.distric = new SelectList(cdb.Distric, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult AddressInfo(AnsarAddress ad)
        {
            TempData["e"] = Session["usertype"];
            Session["Addressinfo"] = ad;

            return RedirectToAction("Prasikkan");
        }

        [HttpGet]
        public ActionResult Prasikkan()
        {
            TempData["e"] = Session["usertype"];
            var prasikkanList = prsikkandetails(User.Identity.Name).ToList();
            ViewBag.prasikkan = new SelectList(prasikkanList.ToList(), "Id", "Name");
            /* for prsikkan list end */
            return View();
        }

        [HttpPost]
        public ActionResult Register(Ansartranning pra)
        {
            TempData["e"] = Session["usertype"];

            var cookee = User.Identity.Name;

            var userid = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Id).FirstOrDefault();
            var ps = (BAV.Models.AnsarInfo)Session["Ansarinfo"];
            var p = (BAV.Models.AnsarAddress)Session["Addressinfo"];

            ps.Id = Guid.NewGuid();
            p.Id = Guid.NewGuid();
            int pd = p.PerDistric;
            int ped = p.PresDistric;

            if (pd >= 3 && pd <= 13)
            {
                p.PerPostCodeId = p.PerPostCodeId;
                p.PerWard = p.PerWard;
            }
            else
            {
                p.PerPostCodeId = p.PerPostCodeIdM;
                p.PerWard = p.PerWardM;

            }


            p.MemberId = ps.Id;
            pra.MemberId = ps.Id;
            pra.IsActive = true;
            ps.userId = userid;
            ps.creationdate = DateTime.Now.Date;
            pra.Id = Guid.NewGuid();
            mrbd.AnsarInfo.Add(ps);
            mrbd.AnsarAddress.Add(p);
            mrbd.Ansartranning.Add(pra);

            mrbd.SaveChanges();
            Session["Ansarinfo"] = null;
            Session["Addressinfo"] = null;

            TempData["eror"] = "সদস্য রেজিস্ট্রেশন সফল হয়েছে";
            return RedirectToAction("Personalinfo");


        }
        [HttpGet]
        public ActionResult AnsarSearch(int? Page_No)
        {
            TempData["e"] = Session["usertype"];
            int districid = Convert.ToInt16(Session["Distrcid"]);
            var upid = cdb.SubDistric.Where(x => x.countryid.Equals(districid)).ToList();
            ViewBag.degisnationlist = new SelectList(cdb.Designation.Where(x => x.Id.Equals(15) || x.Id.Equals(16) || x.Id.Equals(17)), "Id", "Name");
            var educationlist = cdb.Exam.ToList();
            ViewBag.educationlist = new SelectList(educationlist.ToList(), "Id", "Name");
            var prasikkanList = prsikkandetails(User.Identity.Name).ToList();
            ViewBag.prasikkan = new SelectList(prasikkanList.ToList(), "Id", "Name");
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            var disignation = cdb.Designation.ToList();

            var volantierlist = from pi in mrbd.AnsarInfo.ToList()
                                join a in mrbd.AnsarAddress.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.Ansartranning.ToList() on pi.Id equals bs.MemberId
                                join ed in educationlist.ToList() on pi.education equals ed.Id
                                join dig in disignation.ToList() on pi.Degisnation equals dig.Id
                                join up in upid.ToList() on pi.Upid equals up.Id
                                select new aninfoantrad { a = a, ai = pi, at = bs, digname = dig.Name, education = ed.Name };
            int Size_Of_Page = 1000000;
            int No_Of_Page = (Page_No ?? 1);
            return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));
            //return View(volantierlist.ToList());

        }

        [AuthorizeRoles("Admin")]
        public ActionResult TranningName()
        {
            TempData["e"] = Session["usertype"];

            TempData["Prasikkan"] = "List";

            return View();
        }
        [AuthorizeRoles("Admin")]
        [HttpPost]
        public ActionResult TranningName(Tranning model)
        {
            TempData["e"] = Session["usertype"];
            var userid = db.User.Where(x => x.UserName.ToLower().Equals(User.Identity.Name.ToLower())).Select(x => x.Id).SingleOrDefault();
            model.CreationDate = DateTime.Now.Date;
            model.UserId = userid;
            cdb.Tranning.Add(model);
            cdb.SaveChanges();
            TempData["Prasikkan"] = "List";
            return RedirectToAction("TranningName");
        }
        [AuthorizeRoles("Admin")]
        public ActionResult TranningEdit(int id)
        {
            var model = cdb.Tranning.Where(x => x.Id.Equals(id)).SingleOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult TranningEdit(Tranning model)
        {
            var s = cdb.Tranning.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
            s.Name = model.Name;
            s.CreationDate = model.CreationDate;
            cdb.Entry(s).State = EntityState.Modified;
            cdb.SaveChanges();
            return RedirectToAction("TranningName");

        }

        [AuthorizeRoles("Admin")]
        public ActionResult PrasikkanNameList(int? Page_No)
        {
            List<Tranning> prasikkan = new List<Tranning>();
            TempData["e"] = Session["usertype"];
            prasikkan = prsikkandetails(User.Identity.Name).ToList();
            int Size_Of_Page = 1000;
            int No_Of_Page = (Page_No ?? 1);
            return View(prasikkan.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [AuthorizeRoles("Admin")]
        [HttpGet]
        public ActionResult MemberDetails(Guid id)
        {
            TempData["e"] = Session["usertype"];
            string role = Convert.ToString(Session["rolename"]);
            if (role == "SubAdmin")
            {
                ViewBag.type = "true";
            }
            ansermember model = new ansermember();

            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();

            var volantierlist = from pi in mrbd.AnsarInfo.ToList()
                                join a in mrbd.AnsarAddress.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.Ansartranning.ToList() on pi.Id equals bs.MemberId
                                where pi.Id.Equals(id)
                                select new aninfoantrad { a = a, ai = pi, at = bs };

            var volantier = from vl in volantierlist.ToList()
                            join dig in cdb.Designation.ToList() on vl.ai.Degisnation equals dig.Id
                            join pt in cdb.SubDistric.ToList() on vl.a.PresSubDistric equals pt.Id
                            join pd in cdb.Distric.ToList() on vl.a.PresDistric equals pd.Id
                            join pet in cdb.SubDistric on vl.a.PerSubDistric equals pet.Id
                            join ped in cdb.Distric.ToList() on vl.a.PerDistric equals ped.Id
                            join edu in cdb.Exam.ToList() on vl.ai.education equals edu.Id
                            join pra in cdb.Tranning.ToList() on vl.at.PraNameId equals pra.Id

                            select new ansermember
                            {
                                Id = vl.ai.Id,
                                bname = vl.ai.Name,
                                faname = vl.ai.FatherName,
                                bmobile = vl.ai.mobile,
                                personno = vl.ai.personno,
                                designation = dig.Name,
                                education = edu.Name,
                                occupation = vl.ai.occupation,

                                praname = pra.Name,
                                prinstitude = vl.at.PraInstitudeName,
                                pstart = vl.at.StartDate,
                                pend = vl.at.EndDate,


                                presaddress = vl.a.PerAddress,

                                presthana = pt.Name,
                                presdist = pd.Name,


                                paddress = vl.a.PresAddress,
                                perthana = pet.Name,
                                perdist = ped.Name,
                                prpostcode = vl.a.PerPostCodeId,
                                perunion = vl.a.PerWard,
                                perunions = vl.a.PerUnion,
                            };
            model = volantier.SingleOrDefault();
            var t = mrbd.Ansartranning.Where(x => x.MemberId.Equals(id)).Count();
            if (t > 0)
            {
                ViewBag.c = "true";
            }
            else
            {
                ViewBag.c = "false";
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult MemberEdit(Guid id)
        {
            ViewBag.exam = new SelectList(cdb.Exam, "Id", "Name");
            ViewBag.designationlist = new SelectList(cdb.Designation.Where(x => x.Id.Equals(15) || x.Id.Equals(16) || x.Id.Equals(17)), "Id", "Name");
            var s = User.Identity.Name;
            var userid = db.User.Where(x => x.UserName.Equals(s)).Select(x => x.Id).SingleOrDefault();
            var platun = db.Platun.Where(x => x.UserId.Equals(userid)).Count();
            int disid = Convert.ToInt16(Session["Distrcid"]);

            if (platun > 0)
            {

                ViewBag.subdistriclist = new SelectList(cdb.SubDistric.Where(x => x.countryid.Equals(disid)), "Id", "Name");
            }
            else
            {
                var subdistriclist = distictlist(userid).ToList();
                ViewBag.subdistriclist = new SelectList(subdistriclist.ToList(), "Id", "Name");
            }
            var model = mrbd.AnsarInfo.Where(x => x.Id.Equals(id)).SingleOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult MemberEdit(AnsarInfo model)
        {
            var s = mrbd.AnsarInfo.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
            s.mobile = model.mobile;
            s.Name = model.Name;
            s.occupation = model.occupation;
            s.personno = model.personno;
            s.Upid = model.Upid;

            s.age = model.age;
            s.Degisnation = model.Degisnation;
            s.education = model.education;
            s.FatherName = model.FatherName;

            mrbd.Entry(s).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("MemberDetails", new { id = model.Id });
        }
        public ActionResult PrasikkanDetails(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var prname = new List<praname>();
            var prasikkanlist = from p in mrbd.Ansartranning.ToList()
                                join pn in cdb.Tranning.ToList() on p.PraNameId equals pn.Id
                                where p.MemberId.Equals(id)
                                select new praname
                                {
                                    at = p,
                                    t = pn

                                };
            prname = prasikkanlist.ToList();
            return View(prname);
        }

        [HttpGet]
        public ActionResult AddressEdit(Guid id)
        {

            var model = mrbd.AnsarAddress.Where(x => x.MemberId.Equals(id)).SingleOrDefault();

            ViewBag.presdistrics = new SelectList(cdb.Distric, "Id", "Name", model.PresDistric);
            ViewBag.presSubdistrics = new SelectList(cdb.SubDistric, "Id", "Name", model.PresSubDistric);
            ViewBag.distric = new SelectList(cdb.Distric, "Id", "Name");
            ViewBag.Subdistric = new SelectList(cdb.SubDistric, "Id", "Name");

            if (model.PerDistric >= 3 && model.PerDistric <= 13)
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
        public ActionResult AddressEdit(AnsarAddress model)
        {

            var s = mrbd.AnsarAddress.Where(x => x.MemberId.Equals(model.MemberId)).SingleOrDefault();
            s.PerAddress = model.PerAddress;
            s.PerDistric = model.PerDistric;
            int pd = model.PerDistric;
            int ped = model.PresDistric;
            if (pd >= 3 && pd <= 13)
            {
                s.PerPostCodeId = model.PerPostCodeId;
                s.PerWard = model.PerWard;
            }
            else
            {
                s.PerPostCodeId = model.PerPostCodeIdM;
                s.PerWard = model.PerWardM;

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
        public ActionResult Prassikkanedit(Guid id)
        {

            var model = mrbd.Ansartranning.Where(x => x.Id.Equals(id)).SingleOrDefault();

            return View(model);
        }
        [HttpPost]
        public ActionResult Prassikkanedit(Ansartranning model)
        {
            var mid = mrbd.Ansartranning.Where(x => x.SNo.Equals(model.SNo) && x.SNo != null).Count();
            if (mid > 0)
            {

            }
            else
            {
                var s = mrbd.Ansartranning.Where(x => x.Id.Equals(model.Id)).SingleOrDefault();
                s.SNo = model.SNo;
                s.IsActive = false;
                mrbd.Entry(s).State = EntityState.Modified;
                mrbd.SaveChanges();
            }
            return RedirectToAction("MemberDetails", new { id = model.MemberId });
        }

        public ActionResult PrintlistMember(int? did, int? eduId, int? trnId)
        {
            string jelncom;

            int disid = Convert.ToInt16(Session["Distrcid"]);
            var districname = cdb.Distric.Where(x => x.Id.Equals(disid)).Select(x => x.Name).FirstOrDefault();
            var joncomandarname = mrbd.ComandarSpecification.Where(x => x.DistricId.Equals(disid) && x.comandartype.Equals(1) && x.IsActive.Equals(true)).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
            if (disid == 13 || disid == 74)
            {
                jelncom = "জোন অধিনায়ক এর কার্যালয়";

            }
            else
            {
                jelncom = "জেলা কমান্ড্যান্ট এর কার্যালয়";

            }
            ReportDocument rd = new ReportDocument();
            if (trnId == null)
            {
                var volantierlist = from ml in memberList().ToList()
                                    select new
                                    {
                                        name = ml.ai.Name,
                                        fathername = ml.ai.FatherName,

                                        education = ml.education,
                                        digisnation = ml.digname,
                                        mobile = ml.ai.mobile,
                                        sno = ml.at.SNo,

                                        address = ml.a.PerAddress,
                                        postcode = ml.a.PerPostCodeId,
                                        disname = ml.disname,
                                        sudisname = ml.sudisname,

                                    };
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "crt-AnsarList.rpt"));
                rd.SetDataSource(volantierlist);

            }
            else if (trnId != null)
            {
                string prasinkkanname = cdb.Tranning.Where(x => x.Id == trnId).Select(x => x.Name).FirstOrDefault();
                var volantierlist = from ml in memberList().Where(x => x.at.PraNameId == trnId).ToList()
                                    select new
                                    {
                                        name = ml.ai.Name,
                                        fathername = ml.ai.FatherName,

                                        education = ml.education,
                                        digisnation = ml.digname,
                                        mobile = ml.ai.mobile,
                                        sno = ml.at.SNo,

                                        address = ml.a.PerAddress,
                                        postcode = ml.a.PerPostCodeId,
                                        disname = ml.disname,
                                        sudisname = ml.sudisname,

                                    };
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "crt-TrannedAnsarList.rpt"));
                rd.SetDataSource(volantierlist);
                rd.SetParameterValue("prasinkkanname", prasinkkanname.ToString());

            }



            rd.SetParameterValue("districname", districname.ToString());
            rd.SetParameterValue("jelncom", jelncom.ToString());

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

        private List<aninfoantrad> memberList()
        {
            int districid = Convert.ToInt16(Session["Distrcid"]);
            var upid = cdb.SubDistric.Where(x => x.countryid.Equals(districid)).ToList();
            var educationlist = cdb.Exam.ToList();
            var disignation = cdb.Designation.ToList();
            var volantierlist = from pi in mrbd.AnsarInfo.ToList()
                                join a in mrbd.AnsarAddress.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.Ansartranning.ToList() on pi.Id equals bs.MemberId
                                join ed in educationlist.ToList() on pi.education equals ed.Id
                                join dig in disignation.ToList() on pi.Degisnation equals dig.Id
                                join dis in cdb.Distric.ToList() on a.PerDistric equals dis.Id
                                join sdis in cdb.SubDistric.ToList() on a.PerSubDistric equals sdis.Id
                                join up in upid.ToList() on pi.Upid equals up.Id
                                select new aninfoantrad { a = a, ai = pi, at = bs, digname = dig.Name, education = ed.Name, disname = dis.Name, sudisname = sdis.Name };
            return volantierlist.ToList();
        }

        private List<Tranning> prsikkandetails(string username)
        {
            var Createdby = "";
            var adminid = Guid.Empty;

            var count = db.User.Where(x => x.Createdby.ToLower().Equals(username.ToLower())).Count();

            if (count > 0)
            {
                Createdby = username;
                adminid = db.User.Where(x => x.UserName.ToLower().Equals(Createdby.ToLower())).Select(x => x.Id).SingleOrDefault();
            }
            else
            {
                Createdby = db.User.Where(x => x.UserName.ToLower().Equals(username.ToLower())).Select(x => x.Createdby).FirstOrDefault();
                adminid = db.User.Where(x => x.UserName.ToLower().Equals(Createdby.ToLower())).Select(x => x.Id).SingleOrDefault();

            }
            var user = db.User.Where(x => x.Createdby.ToLower().Equals(Createdby.ToLower())).ToList();
            var prasikkanlist = (from p in cdb.Tranning.ToList()
                                 where p.UserId.Equals(adminid)
                                 select new Tranning
                                 {
                                     Id = p.Id,
                                     UserId = p.UserId,
                                     CreationDate = p.CreationDate,
                                     Name = p.Name
                                 })
                                .Union
                                (from p in cdb.Tranning.ToList()
                                 join u in user.ToList() on p.UserId equals u.Id
                                 select new Tranning
                                 {
                                     Id = p.Id,
                                     UserId = p.UserId,
                                     CreationDate = p.CreationDate,
                                     Name = p.Name
                                 });
            return prasikkanlist.ToList();

        }

        private List<SubDistric> distictlist(Guid userid)
        {


            var platunlist = from up in db.UserPlatun.ToList()
                             join st in db.Platun.ToList() on up.PlatunId equals st.Id
                             where (up.UserId.Equals(userid))
                             select new
                             {
                                 Id = st.Id,
                                 PlatuneName = st.PlatuneName
                             };

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
                                 select new SubDistric
                                 {
                                     Id = sd.Id,
                                     Name = sd.Name
                                 };


            return subdistriclist.ToList();
        }

    }
}