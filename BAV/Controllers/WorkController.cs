using BAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using BAV.Security;
namespace BAV.Controllers
{
    //[SessionExpire]
    public class WorkController : Controller
    {
        public CommonContext cdb = new CommonContext();
        public MemberRegistrationContext mrbd = new MemberRegistrationContext();
        public UsersContext db = new UsersContext();
        TimeSpan time;
        //
        // GET: /Work/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MemberEvent(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = mrbd.Event.Where(x => x.Id.Equals(id) && x.IsActive.Equals(true)).SingleOrDefault();
            ViewBag.idevent = model.Id;
            ViewBag.name = model.EventName;
            ViewBag.date = model.Date;
            ViewBag.area = model.Area;
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
            ViewBag.platunname = new SelectList(platunlist.ToList(), "Id", "PlatunName");
            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist.ToList() on pi.platunId equals pla.Id
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                orderby (pi.Date)
                                where m.Status.Equals(2) && m.IsActive == false && !(from g in mrbd.EventMember where g.IsActive.Equals(true) select g.memberId).Contains(pi.Id)
                                select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi, planame=pla.PlatunName };
            int Size_Of_Page = 1000000;
            int No_Of_Page = 1;
            return View(volantierlist.ToPagedList(No_Of_Page, Size_Of_Page));
          
            
        }
        [HttpPost]
        public ActionResult MemberEvent(string[] ids, Guid idevent)
        {
            TempData["e"] = Session["usertype"];
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
                        EventMember dba = new EventMember();
                        dba.Id = Guid.NewGuid();
                        dba.memberId = i.Id;
                        dba.eventId =idevent;
                        dba.starttime = DateTime.Now.Date;
                        dba.IsActive = true;
                        dba.IsPaid = false;
                        mrbd.EventMember.Add(dba);
                        mrbd.SaveChanges();

                    }

                }
            

            return RedirectToAction("CreateEvent","Common");
        }
        public ActionResult Payment()
        {
            TempData["e"] = Session["usertype"];
            string s = "p";
            List<Event> eventlist = eventname(s);
         
            ViewBag.eventname = new SelectList(eventlist.ToList(), "Id", "EventName");
            return View();
        }
        [HttpPost]
        public ActionResult Payment(Event model)
        {
            TempData["e"] = Session["usertype"];
            if(model.Id!=null)
            {
                TempData["list"] = "List";
            }
            string s = "p";
            List<Event> eventlist = eventname(s);
          
            ViewBag.eventname = new SelectList(eventlist.ToList(), "Id", "EventName");
            return View();
        }

        public ActionResult distribution()
        {
            TempData["e"] = Session["usertype"];

            string s = "d";
            List<Event> eventlist = eventname(s);
        
            ViewBag.eventname = new SelectList(eventlist.ToList(), "Id", "EventName");
         
            return View();
        }
        [HttpPost]
        public ActionResult distribution(Event model)
        {
            TempData["e"] = Session["usertype"];
            if (model.Id != null)
            {
                TempData["list"] = "List";
            }

            string s = "d";
            List<Event> eventlist = eventname(s);
           
            ViewBag.eventname = new SelectList(eventlist.ToList(), "Id", "EventName");
            return View();
        }
        public ActionResult MemberEventList(int? Page_No,Event model,string s)
        {
            TempData["e"] = Session["usertype"];
            TempData["eventid"] = model.Id;
           
            if (s == "true")
            {
               
                var memberlist = from pi in mrbd.PersonalInfo.ToList()
                                 join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                 join me in mrbd.EventMember.ToList() on pi.Id equals me.memberId
                                 join e in mrbd.Event.ToList() on me.eventId equals e.Id
                                 where me.eventId.Equals(model.Id) && m.IsActive == false && !String.IsNullOrEmpty(me.workfor) && me.IsPaid.Equals(false)
                                 select new Personevent
                                 {
                                     pi = pi,
                                     em = me,
                                     e = e,
                                     m = m,
                                   
                                 };
                ViewBag.s = "1";

                int Size_Of_Page = 10;
                int No_Of_Page = (Page_No ?? 1);
                return View(memberlist.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                var memberlist = from pi in mrbd.PersonalInfo.ToList()
                                 join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                 join me in mrbd.EventMember.ToList() on pi.Id equals me.memberId
                                 join e in mrbd.Event.ToList() on me.eventId equals e.Id
                                 where me.eventId.Equals(model.Id) && m.IsActive == false && String.IsNullOrEmpty(me.workfor) && me.IsActive.Equals(true)
                                 select new Personevent
                                 {
                                     pi = pi,
                                     em = me,
                                     e = e,
                                     m = m
                                 };

                int Size_Of_Page = 10;
                int No_Of_Page = (Page_No ?? 1);
                return View(memberlist.ToPagedList(No_Of_Page, Size_Of_Page));

            }
        }

        public ActionResult EventMemberEdit(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var model = new Personevent();
            var platunlist = db.Platun.ToList();
            var memberlist = from pi in mrbd.PersonalInfo.ToList()
                             join p in platunlist.ToList() on pi.platunId equals p.Id
                             join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                             join me in mrbd.EventMember.ToList() on pi.Id equals me.memberId
                             join e in mrbd.Event.ToList() on me.eventId equals e.Id
                             where me.Id.Equals(id) && m.IsActive==false 
                             select new Personevent
                             {
                                 pi = pi,
                                 em = me,
                                 e = e,
                                 m = m,
                                 planame=p.PlatuneName

                             };
            
            model = memberlist.SingleOrDefault();
            if(model.em.workfor=="m")
            {
                ViewBag.c = "Monthly Based";
            }
            else if (model.em.workfor == "h")
            {
                ViewBag.c = "Hourly Based";
            }
            else if (model.em.workfor == "c")
            {
                ViewBag.c = "Contract Based";
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EventMemberEdit(Personevent model)
        {
            TempData["e"] = Session["usertype"];
            if(model.em.workfor=="m")
            {
                var eventmember = new EventMember();
                var mbm = new MonthBasedMember();
                eventmember = mrbd.EventMember.Where(x => x.Id.Equals(model.em.Id)).SingleOrDefault();
                mbm = mrbd.MonthBasedMember.Where(x => x.EventMemberId.Equals(model.em.Id)).SingleOrDefault();
                if (mbm.dutystart < DateTime.Now.Date)
                {
                    eventmember.IsActive = false;
                    eventmember.endtime = DateTime.Now.Date;
                    mrbd.Entry(eventmember).State = EntityState.Modified;

                    mbm.dutyend = DateTime.Now.Date;
                    mrbd.Entry(mbm).State = EntityState.Modified;
                }
                else
                {
                    eventmember.workfor = null;
                    mrbd.Entry(eventmember).State = EntityState.Modified;
                    mrbd.MonthBasedMember.Remove(mbm);
                }
                mrbd.SaveChanges();
            }
            else if (model.em.workfor == "h")
            {
                var eventmember = new EventMember();
                var hbm = new HourlyBasedMember();
                eventmember = mrbd.EventMember.Where(x => x.Id.Equals(model.em.Id)).SingleOrDefault();
                hbm = mrbd.HourlyBasedMember.Where(x => x.EventMemberId.Equals(model.em.Id)).SingleOrDefault();
                if (hbm.Startingdate < DateTime.Now.Date)
                {
                    eventmember.IsActive = false;
                    eventmember.endtime = DateTime.Now.Date;
                    mrbd.Entry(eventmember).State = EntityState.Modified;

                    hbm.Enddate = DateTime.Now.Date;
                    mrbd.Entry(hbm).State = EntityState.Modified;
                }
                else
                {
                    eventmember.workfor = null;
                    mrbd.Entry(eventmember).State = EntityState.Modified;
                    mrbd.HourlyBasedMember.Remove(hbm);

                }
                mrbd.SaveChanges();
            }
            else if (model.em.workfor == "c")
            {
                var eventmember = new EventMember();
                var cbm = new ContactBasedMember();
                eventmember = mrbd.EventMember.Where(x => x.Id.Equals(model.em.Id)).SingleOrDefault();
                cbm = mrbd.ContactBasedMember.Where(x => x.EventMemberId.Equals(model.em.Id)).SingleOrDefault();
                if (cbm.StartDate < DateTime.Now.Date)
                {
                    eventmember.IsActive = false;
                    eventmember.endtime = DateTime.Now.Date;
                    mrbd.Entry(eventmember).State = EntityState.Modified;
                    cbm = mrbd.ContactBasedMember.Where(x => x.EventMemberId.Equals(model.em.Id)).SingleOrDefault();

                    cbm.EndDate = DateTime.Now.Date;
                    mrbd.Entry(cbm).State = EntityState.Modified;
                }
                else
                {
                    eventmember.workfor = null;
                    mrbd.Entry(eventmember).State = EntityState.Modified;
                    mrbd.ContactBasedMember.Remove(cbm);
                }
                mrbd.SaveChanges();
            }
            return RedirectToAction("Payment");
        }
        public ActionResult PaySalaryToVolantier(Guid id)
        {
            TempData["e"] = Session["usertype"];
            var workfor = mrbd.EventMember.Where(x => x.Id.Equals(id)).Select(x => x.workfor).SingleOrDefault();
            var end= mrbd.EventMember.Where(x => x.Id.Equals(id)).Select(x => x.IsActive).SingleOrDefault();
             var ispaid= mrbd.EventMember.Where(x => x.Id.Equals(id)).Select(x => x.IsPaid).SingleOrDefault();
            if (workfor == "m")
            {
                ViewBag.workfor = "m";
                var model = mrbd.MonthBasedMember.Where(x => x.EventMemberId.Equals(id)).SingleOrDefault();
                if (end == false)
                {
                    time = model.dutyend - model.dutystart;
                }
                else
                {
                    time = DateTime.Now.Date - model.dutystart;
                }
             
                double Days = Convert.ToDouble(time.TotalDays);
                int day = Convert.ToInt32(Days);
                int months;
                ViewBag.id = id;
                ViewBag.stdate = model.dutystart.ToString("MM-dd-yyyy");
                ViewBag.hmgs = model.hmgs;
                if (day >= 30)
                {
                    months = day / 30;
                    ViewBag.month = months;
                    ViewBag.gsm = months - model.hmgs;
                    int days = day % 30;
                    ViewBag.day = days;
                    if (end == false && ispaid==false)
                    {
                        ViewBag.salaryaterendevent = (((months - model.hmgs)*model.salary)+(days*model.perdaycost));   
                    }
                    else
                    {
                        ViewBag.salaryaterendevent = 0;
                    }
                }
                if(day<30)
                {
                    ViewBag.month = 0;
                    ViewBag.day = day;
                    ViewBag.gsm = 0;
                    if (end == false && ispaid == false)
                    {
                        ViewBag.salaryaterendevent = (day * model.perdaycost);

                    }
                    else
                    {
                        ViewBag.salaryaterendevent = 0;
                    }

                }
                ViewBag.salary = model.salary;
                
            }
            else if(workfor=="h")
            { 
                var model = mrbd.HourlyBasedMember.Where(x => x.EventMemberId.Equals(id)).SingleOrDefault();
                ViewBag.workfor = "h";
                if(end==false)
                {
                    time = model.Enddate - model.Startingdate;
                }
               else
                {
                    time = DateTime.Now.Date - model.Startingdate;
                }
                double Days = Convert.ToDouble(time.TotalDays);
                ViewBag.id = id;
                ViewBag.stdate = model.Startingdate.ToString("MM-dd-yyyy");
                ViewBag.salary = model.amount;
                ViewBag.month = model.perdayduty*Days;
                ViewBag.totalsalry = model.amount * (Convert.ToDecimal(model.perdayduty * Days));
                ViewBag.hmgs = model.hmhhw;
                ViewBag.gsmm = (model.amount * (Convert.ToDecimal(model.perdayduty * Days)))-model.hmhhw;
  
            }

            else if (workfor == "c")
            {
                var model = mrbd.ContactBasedMember.Where(x => x.EventMemberId.Equals(id)).SingleOrDefault();
                ViewBag.workfor = "c";
               
                ViewBag.id = id;
                ViewBag.stdate = model.StartDate.ToString("MM-dd-yyyy");
                ViewBag.salary = model.amount;
                ViewBag.hmgs = "0.00"+" Tk";
        
            }

          
            return View();
        }

        [HttpPost]
        public ActionResult PaySalaryToVolantier(Guid id, string gsm, string workfor, string gsmm)
        {
            TempData["e"] = Session["usertype"];
            decimal amount=0;
            if (workfor == "m" || workfor == "h")
            {
                if (workfor == "m")
                {
                    var model = mrbd.MonthBasedMember.Where(x => x.EventMemberId.Equals(id)).SingleOrDefault();
                    var em = mrbd.EventMember.Where(x => x.Id.Equals(id)).SingleOrDefault();
                    if (em.IsActive == false && em.IsPaid == false)
                    {
                        em.IsPaid = true;
                        mrbd.Entry(em).State = EntityState.Modified;
                      
                        time = model.dutyend - model.dutystart;
                        double Days = Convert.ToDouble(time.TotalDays);
                        int day = Convert.ToInt32(Days);
                        int months;
                        if (day >= 30)
                        {
                            months = day / 30;
                            int days = day % 30;
                            amount = (((months - model.hmgs) * model.salary) + (days * model.perdaycost));
                           
                        }
                        if (day < 30)
                        {
                          
                          
                              amount= (day * model.perdaycost);

                        }
                       
                    }
                    else
                    {
                        model.hmgs += Convert.ToInt32(gsm);
                        mrbd.Entry(model).State = EntityState.Modified;
                        amount=Convert.ToInt32(gsm)*model.salary;
                    }
                   
                    mrbd.SaveChanges();
                    
                }
                else if (workfor == "h")
                {
                    var model = mrbd.HourlyBasedMember.Where(x => x.EventMemberId.Equals(id)).SingleOrDefault();
                    var em = mrbd.EventMember.Where(x => x.Id.Equals(id)).SingleOrDefault();
                    if (em.IsActive == false && em.IsPaid == false)
                    {
                        em.IsPaid = true;
                        mrbd.Entry(em).State = EntityState.Modified;
                    }
                    else
                    {
                        model.hmhhw += Convert.ToDecimal(gsmm);
                        mrbd.Entry(model).State = EntityState.Modified;
                        
                    }
                    amount = Convert.ToDecimal(gsmm);
                    mrbd.SaveChanges();
                }
              
                return RedirectToAction("volantierpayment", new { am=amount,evid=id});
            }
            else if (workfor == "c")
            {
                var model = mrbd.ContactBasedMember.Where(x => x.EventMemberId.Equals(id)).SingleOrDefault();
                var em = mrbd.EventMember.Where(x => x.Id.Equals(id)).SingleOrDefault();
                em.IsPaid = true;
                mrbd.Entry(em).State = EntityState.Modified;
                mrbd.SaveChanges();
                return RedirectToAction("volantierpayment", new { am = model.amount, evid = id });
            }
            
            return RedirectToAction("Payment");
        }
    
        public ActionResult volantierpayment(decimal am,Guid evid)
        {
            TempData["e"] = Session["usertype"];
            var memberid = mrbd.EventMember.Where(x => x.Id.Equals(evid)).Select(x => x.memberId).FirstOrDefault();
            var eventid = mrbd.EventMember.Where(x => x.Id.Equals(evid)).Select(x => x.eventId).SingleOrDefault();
            
            var membername = mrbd.PersonalInfo.Where(x => x.Id.Equals(memberid)).Select(x => x.BanglaName).SingleOrDefault();
            var memberno = mrbd.Member.Where(x => x.MemberId.Equals(memberid)).Select(x => x.IDCardNo).SingleOrDefault();
            var eventname = mrbd.Event.Where(x => x.Id.Equals(eventid)).Select(x => x.EventName).SingleOrDefault();
            
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "memberpaymentrpt.rpt"));
          
            rd.SetParameterValue("membername", membername.ToString());
            rd.SetParameterValue("memberno", memberno.ToString());
            rd.SetParameterValue("eventname", eventname.ToString());
            rd.SetParameterValue("amount", am);
            rd.SetParameterValue("date", DateTime.Now.Date);

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
        public ActionResult Workdistribution(Guid id)
        {
            TempData["e"] = Session["usertype"];
            TempData["id"] = id;
            return View();
        }
        [HttpPost]
        public ActionResult Workdistribution(monthlyhourlycontact model,Guid id,string m)
        {
            TempData["e"] = Session["usertype"];
            var eventmember = new EventMember();
            eventmember = mrbd.EventMember.Where(x => x.Id.Equals(id)).SingleOrDefault();
            if(m=="1")
            {
                var mbm=new MonthBasedMember();
                mbm.Id = Guid.NewGuid();
                mbm.EventMemberId = id;
                mbm.dutystart = model.mbs.dutystart;
                mbm.perdaycost = model.mbs.perdaycost;
                mbm.salary = model.mbs.salary;
                mbm.settime = model.mbs.settime;
                eventmember.workfor = "m";
                eventmember.starttime = model.mbs.dutystart;
                mrbd.MonthBasedMember.Add(mbm);
                mrbd.SaveChanges();
            }
            if(m=="2")
            {
                var hbm = new HourlyBasedMember();
                hbm.Id = Guid.NewGuid();
                hbm.EventMemberId = id;
                hbm.settime = model.hbm.settime;
                hbm.Startingdate = model.hbm.Startingdate;
                hbm.amount = model.hbm.amount;
                hbm.perdayduty = model.hbm.perdayduty;
                eventmember.workfor = "h";
                eventmember.starttime = model.hbm.Startingdate;
                mrbd.HourlyBasedMember.Add(hbm);
                mrbd.SaveChanges();
            }
            if(m=="3")
            {
                var cbm = new ContactBasedMember();
                cbm.Id = Guid.NewGuid();
                cbm.StartDate = model.cbm.StartDate;
                cbm.dutitime = model.cbm.dutitime;
                cbm.EventMemberId = id;
                cbm.amount = model.cbm.amount;
                eventmember.workfor = "c";
                eventmember.starttime = model.cbm.StartDate;
                mrbd.ContactBasedMember.Add(cbm);
                mrbd.SaveChanges();
            }
            mrbd.Entry(eventmember).State = EntityState.Modified;
            mrbd.SaveChanges();
            return RedirectToAction("Payment");
        }

        /// <summary>
        /// print event volunteer list
        /// </summary>
        /// <param name="eventid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult eventvolantiarlistprint(Guid eventid)
        {
            List<eventmemberlist> evlist = new List<eventmemberlist>();
            TempData["e"] = Session["usertype"];
            var designation = cdb.Designation.ToList();
            var platun = db.Platun.ToList();
            var eventname = mrbd.Event.Where(x => x.Id.Equals(eventid)).Select(x => x.EventName).FirstOrDefault();
            var eventdate = mrbd.Event.Where(x => x.Id.Equals(eventid)).Select(x => x.Date).FirstOrDefault();
            var emlist = mrbd.EventMember.Where(x => x.eventId.Equals(eventid) && x.IsActive == true && x.IsPaid == false).ToList();
            var memberlist = from em in emlist.ToList()
                             join e in mrbd.Event.ToList() on em.eventId equals e.Id
                             join pi in mrbd.PersonalInfo.ToList() on em.memberId equals pi.Id
                             join m in mrbd.Member.ToList() on em.memberId equals m.MemberId 
                             join d in designation.ToList() on pi.DesignationId equals d.Id
                             join p in platun.ToList() on pi.platunId equals p.Id
                             select new eventmemberlist
                             {
                                 membername = pi.BanglaName,
                                 designation =d.Name,
                                 membercode=m.IDCardNo,
                                 mobile=pi.mobile,
                                 platun=p.PlatuneName

                             };
            evlist = memberlist.ToList();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "EventvolantiarReport.rpt"));
            rd.SetDataSource(memberlist);
            rd.SetParameterValue("eventname", eventname.ToString());
            rd.SetParameterValue("eventdate", eventdate.ToString());
         
        
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

        private List<Event> eventname(string s)
        {
            List<Event> eventlist = new List<Event>();
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
            if(s=="p")
            {
                var eventslist = (from p in mrbd.Event.ToList()
                                  where p.UserId.Equals(adminid)
                                  select new Event
                                  {
                                      Id = p.Id,
                                      EventName = p.EventName
                                  })
                             .Union
                             (from p in mrbd.Event.ToList()
                              join u in user.ToList() on p.UserId equals u.Id
                             
                              select new Event
                              {
                                  Id = p.Id,
                                  EventName = p.EventName
                              });
                eventlist = eventslist.ToList();
            }
            else if (s=="d")
            {
                var eventslist = (from p in mrbd.Event.ToList()
                                  where p.UserId.Equals(adminid) && p.IsActive.Equals(true)
                                  select new Event
                                  {
                                      Id = p.Id,
                                      EventName = p.EventName
                                  })
                              .Union
                              (from p in mrbd.Event.ToList()
                               join u in user.ToList() on p.UserId equals u.Id
                               where p.IsActive.Equals(true)
                               select new Event
                               {
                                   Id = p.Id,
                                   EventName = p.EventName
                               });
                eventlist = eventslist.ToList();
                
            }

            return eventlist.ToList();
        }
	}
}