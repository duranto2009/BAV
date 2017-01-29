using BAV.Models;
using BAV.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BAV.Controllers
{
    [SessionExpire]
    [AuthorizeRoles("RangCommander")]
    public class CommanderController : Controller
    {
        public UsersContext db = new UsersContext();
        public CommonContext cdb = new CommonContext();
        public MemberRegistrationContext mrbd = new MemberRegistrationContext();
        // GET: /Commander/
        public ActionResult Index()
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            var userid=db.User.Where(x=>x.UserName.Equals(User.Identity.Name)).Select(x=>x.Id).FirstOrDefault();
            var districtlist=from rd in db.RangDistrict.ToList()
                             join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                             where rd.UserId.Equals(userid)
                             select new
                             {
                                Id=d.Id,
                                Name=d.Name
                             };
            ViewBag.districlist = new SelectList(districtlist.ToList(), "Id", "Name");
  
            var subdistrictlist = from rd in db.RangDistrict.ToList()
                                  join sd in cdb.SubDistric.ToList() on rd.DistrictId equals sd.countryid
                                  where rd.UserId.Equals(userid)
                                  select new
                                  {
                                   Id =sd.Id,
                                   Name = sd.Name
                                 };
            ViewBag.subdistriclist = new SelectList(subdistrictlist.ToList(), "Id", "Name");

            var degisnationlist = cdb.Designation.ToList();
            ViewBag.degisnationslist = new SelectList(degisnationlist.ToList(), "Id", "Name");
            var vollist = from mo in volantiarlist(0).ToList()
                          join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                          join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                          select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
            model = vollist.ToList();
            return View(model);
        }
        public ActionResult AppliedMember()
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };
            ViewBag.districlist = new SelectList(districtlist.ToList(), "Id", "Name");
            var subdistrictlist = from rd in db.RangDistrict.ToList()
                                  join sd in cdb.SubDistric.ToList() on rd.DistrictId equals sd.countryid
                                  where rd.UserId.Equals(userid)
                                  select new
                                  {
                                      Id = sd.Id,
                                      Name = sd.Name
                                  };
            ViewBag.subdistriclist = new SelectList(subdistrictlist.ToList(), "Id", "Name");

            var degisnationlist = cdb.Designation.ToList();
            ViewBag.degisnationslist = new SelectList(degisnationlist.ToList(), "Id", "Name");
            var vollist = from mo in volantiarlist(1).ToList()
                          join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                          join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                          select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
            model = vollist.ToList();
            return View(model);
        }
        public ActionResult PlatunDetails()
        {
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new
                               {
                                   Id = d.Id,
                                   Name = d.Name
                                   
                               };
            ViewBag.districlist = new SelectList(districtlist.ToList(), "Id", "Name");
           
            var subdistrictlist = from rd in db.RangDistrict.ToList()
                                  join sd in cdb.SubDistric.ToList() on rd.DistrictId equals sd.countryid
                                  where rd.UserId.Equals(userid)
                                  select new
                                  {
                                      Id = sd.Id,
                                      Name = sd.Name
                                  };
            ViewBag.subdistriclist = new SelectList(subdistrictlist.ToList(), "Id", "Name");
           var plalist = platunlist(User.Identity.Name).ToList();
            return View(plalist);
        }
        public ActionResult platunlist(int? cid , int? sdid)
        {
            if (cid == null)
            {
                var plalist = platunlist(User.Identity.Name).Where(x => x.subdistrictid.Equals(sdid)).ToList();
                return View(plalist);
            }
            else
            {
                var plalist = platunlist(User.Identity.Name).Where(x => x.districtid.Equals(cid)).ToList();
                return View(plalist);
            }
           
        }
        public ActionResult MemberPrasikkan()
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
          
            ViewBag.prasikkanlist = new SelectList(prasikkanlist(User.Identity.Name).ToList(), "Id", "Name");
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };
            ViewBag.districlist = new SelectList(districtlist.ToList(), "Id", "Name");
            var vollist = from mo in volantiarlist(0).ToList()
                          join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                          join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                          select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
            model = vollist.ToList();
            return View(model);
        }
        public ActionResult MemberEvent()
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            ViewBag.eventslist = new SelectList(eventlist(User.Identity.Name).ToList(), "Id", "EventName");
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };
            ViewBag.districlist = new SelectList(districtlist.ToList(), "Id", "Name");

            var vollist = from mo in volantiarlist(0).ToList()
                          join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                          join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                          select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
            model = vollist.ToList();
            return View(model);

        }
        public ActionResult apliedvolantiar(int? cid, int? sdid, int? did)
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();

            if (cid == null && sdid == null && did == null)
            {
                var vollist = from mo in volantiarlist(1).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (cid != null && sdid == null && did == null)
            {
                var vollist = from mo in volantiarlist(1).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where pla.districtid.Equals(cid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (cid == null && sdid != null && did == null)
            {
                var vollist = from mo in volantiarlist(1).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where pla.subdistrictid.Equals(sdid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (cid != null && sdid == null && did != null)
            {
                var vollist = from mo in volantiarlist(1).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where dig.Id.Equals(did) && pla.districtid.Equals(cid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();

            }

            return View(model);
        }
        public ActionResult Volantiarlist(int? cid,int? sdid, int? did)
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();

            if (cid == null && sdid == null && did==null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (cid != null && sdid == null && did == null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where pla.districtid.Equals(cid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (cid == null && sdid != null && did == null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where pla.subdistrictid.Equals(sdid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (cid != null && sdid == null && did != null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where dig.Id.Equals(did) && pla.districtid.Equals(cid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();

            }
       
            return View(model);
        }
        public ActionResult volantiarDetails(Guid Id)
        {
            PersonalInfoMemberAddressBodyStructureReligion model = new PersonalInfoMemberAddressBodyStructureReligion();
            

            var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                join pla in platunlist(User.Identity.Name).ToList() on pi.platunId equals pla.PlatunId
                                join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                join i in mrbd.Image.ToList() on pi.Id equals i.MemberId
                                where pi.Id.Equals(Id)
                                select new PersonalInfoMemberAddressBodyStructure
                                {
                                    pi = pi,
                                    m = m,
                                    a = a,
                                    bs = bs,
                                    i = i,
                                    planame = pla.PlatuneName
                                };
            var s = volantierlist.Select(x => x.bs.BloodGroupId).SingleOrDefault();
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
                                        sandadno = vl.pi.dp,

                                        dob = vl.pi.DOB,
                                        maritalstatus = vl.pi.MaritalStatus,
                                        smname = vl.pi.WORHName,
                                        smpasa = vl.pi.WorHOccupation,
                                        nid = vl.pi.NID,
                                        dobno = vl.pi.DOBSNo,
                                        p = vl.pi.p,
                                        po = vl.pi.po,
                                        pos = vl.pi.pos,
                                        pot = vl.pi.pot,
                                        ps = vl.pi.ps,
                                        pst = vl.pi.pst,
                                        isactive = vl.m.IsActive,



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
           
            return View(model);
        }
        public ActionResult GetPrasikkanMemberList(int Id)
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            var prasikkanmemberlist = from v in volantiarlist(0).ToList()
                                      join pla in platunlist(User.Identity.Name).ToList() on v.pi.platunId equals pla.PlatunId
                                      join p in mrbd.Prasikkan on v.pi.Id equals p.MemberId
                                      where p.PraNameId.Equals(Id)
                                      select new PersonalInfoMemberAddressBodyStructure
                                      {
                                          pi = v.pi,
                                          m = v.m,
                                          a = v.a,
                                          bs = v.bs,
                                          platunid =pla.PlatunId,
                                          planame = pla.PlatuneName,
                                          Name=pla.disnane,
                                           disignation=pla.subname,
                                          p = p
                                      };
            model = prasikkanmemberlist.ToList();
          
            return View(model);
        }
        public ActionResult GetEventMemberList(Guid Id)
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            var eventmemberlist = from v in volantiarlist(0).ToList()
                                      join pla in platunlist(User.Identity.Name).ToList() on v.pi.platunId equals pla.PlatunId
                                      join me in mrbd.EventMember.ToList() on v.pi.Id equals me.memberId
                                      join e in mrbd.Event.ToList() on me.eventId equals e.Id
                                      where me.eventId.Equals(Id) && !String.IsNullOrEmpty(me.workfor) && me.IsPaid.Equals(false)
                                      select new PersonalInfoMemberAddressBodyStructure
                                      {
                                          pi = v.pi,
                                          m = v.m,
                                          a = v.a,
                                          bs = v.bs,
                                          platunid = pla.PlatunId,
                                          planame = pla.PlatuneName,
                                          Name = pla.disnane,
                                          disignation = pla.subname,
                                      
                                      };
            model = eventmemberlist.ToList();
            return View(model);
        }
        public ActionResult SendMessage()
        {
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var districtlist = from rd in db.RangDistrict.ToList()
                               join d in cdb.Distric.ToList() on rd.DistrictId equals d.Id
                               where rd.UserId.Equals(userid)
                               select new
                               {
                                   Id = d.Id,
                                   Name = d.Name
                               };
            ViewBag.districlist = new SelectList(districtlist.ToList(), "Id", "Name");
            var subdistrictlist = from rd in db.RangDistrict.ToList()
                                  join sd in cdb.SubDistric.ToList() on rd.DistrictId equals sd.countryid
                                  where rd.UserId.Equals(userid)
                                  select new
                                  {
                                      Id = sd.Id,
                                      Name = sd.Name
                                  };
            ViewBag.subdistriclist = new SelectList(subdistrictlist.ToList(), "Id", "Name");

            var degisnationlist = cdb.Designation.ToList();
            ViewBag.degisnationslist = new SelectList(degisnationlist.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult SendMessage(string ms,Guid[] ids)
        {
            if(ids==null)
            {
                TempData["error"] = "Please Select Member";
            }
            else
            {

                foreach (var i in ids)
                {
                   var pi = mrbd.PersonalInfo.Where(x =>x.Id.Equals(i)).FirstOrDefault();
                    message dba = new message();
                    dba.Id = Guid.NewGuid();
                    dba.MemberId = pi.Id;
                    dba.messages = ms;
                    dba.date = DateTime.Now;
                    //convertBanglaDigitToEnglish(pi.mobile);
                    //mrbd.message.Add(dba);
                    //mrbd.SaveChanges();
                    TempData["error"] = "Sent Successfully.";
                }
            }
            return RedirectToAction("SendMessage");
        }
        public JsonResult GetEvent(int idd)
        {
            List<Event> eventslist = new List<Event>();

            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var udis = db.RangDistrict.Where(x => x.UserId.Equals(userid)).ToList();
            var subdis = from ud in udis.ToList()
                         join sd in cdb.SubDistric.ToList() on ud.DistrictId equals sd.countryid
                         where ud.DistrictId==idd
                         select new SubDistric
                         {
                             Id = sd.Id,
                             Name = sd.Name
                         };

            var userlist = from u in db.User.ToList()
                           join ud in db.UserDistic.ToList() on u.Id equals ud.UserId
                           join sd in subdis.ToList() on ud.SubdisticId equals sd.Id
                           group u by new { u.Id, u.UserName } into g
                           select new User { Id = g.Key.Id, UserName = g.Key.UserName };

            var eventlist = from e in mrbd.Event.ToList()
                            join us in userlist.ToList() on e.UserId equals us.Id
                            select new Event
                            {
                                Id = e.Id,
                                EventName = e.EventName
                            };
            eventslist = eventlist.ToList();
            SelectList objdata = new SelectList(eventslist.ToList(), "Id", "EventName", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetPrasikkan(int idd)
        {
             List<PrasikkanName> plalist = new List<PrasikkanName>();
            var userid = db.User.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).FirstOrDefault();
            var udis = db.RangDistrict.Where(x => x.UserId.Equals(userid)).ToList();
            var subdis = from ud in udis.ToList()
                         join sd in cdb.SubDistric.ToList() on ud.DistrictId equals sd.countryid
                         where ud.DistrictId==idd
                         select new SubDistric
                         {
                             Id = sd.Id,
                             Name = sd.Name
                         };
            var userlist = from u in db.User.ToList()
                           join ud in db.UserDistic.ToList() on u.Id equals ud.UserId
                           join sd in subdis.ToList() on ud.SubdisticId equals sd.Id
                           group u by new { u.Id, u.UserName } into g
                           select new User { Id = g.Key.Id, UserName = g.Key.UserName };

            var prasikkanlist = from up in cdb.PrasikkanName.ToList()
                                join us in userlist.ToList() on up.UserId equals us.Id
                                select new PrasikkanName
                                {
                                    Id = up.Id,
                                    Name = up.Name
                                };
            plalist = prasikkanlist.ToList();
            SelectList objdata = new SelectList(plalist.ToList(), "Id", "Name", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetPlatuon(int idd)
        {

            var plalist = platunlist(User.Identity.Name).Where(x => x.subdistrictid.Equals(idd)).ToList();
            SelectList objdata = new SelectList(plalist.ToList(), "PlatunId", "PlatuneName", 0);

            return new JsonResult { Data = objdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult MSSVolantiarList(Guid? pid,int? cid,int? sdid)
        {
            List<PersonalInfoMemberAddressBodyStructure> model = new List<PersonalInfoMemberAddressBodyStructure>();
            if (pid != null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where mo.pi.platunId.Equals(pid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if(cid!=null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where pla.districtid.Equals(cid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            else if (sdid != null)
            {
                var vollist = from mo in volantiarlist(0).ToList()
                              join pla in platunlist(User.Identity.Name).ToList() on mo.pi.platunId equals pla.PlatunId
                              join dig in cdb.Designation.ToList() on mo.pi.DesignationId equals dig.Id
                              where pla.subdistrictid.Equals(sdid)
                              select new PersonalInfoMemberAddressBodyStructure { a = mo.a, bs = mo.bs, m = mo.m, pi = mo.pi, planame = pla.PlatuneName, Name = pla.disnane, subdis = pla.subname, disignation = dig.Name };
                model = vollist.ToList();
            }
            return View(model);
        }
        private List<PlaDistric>platunlist(string username)
        {
            List<PlaDistric> plalist = new List<PlaDistric>();

            var userid = db.User.Where(x => x.UserName.Equals(username)).Select(x => x.Id).FirstOrDefault();
            var subdistrictlist = from ud in db.RangDistrict.ToList()
                                  join d in cdb.Distric.ToList() on ud.DistrictId equals d.Id
                                  join sd in cdb.SubDistric.ToList() on ud.DistrictId equals sd.countryid
                                  join pla in db.Platun.ToList() on sd.Id equals pla.SubDistrcId
                                  where ud.UserId.Equals(userid) && pla.IsActive.Equals(true)
                                  select new PlaDistric
                                  {
                                      PlatuneName = pla.PlatuneName,
                                      creationdte = pla.CreationDate,
                                      disnane = d.Name,
                                      subname = sd.Name,
                                      PlatunId = pla.Id,
                                      Isactive = pla.IsActive,
                                      subdistrictid=sd.Id,
                                      districtid=d.Id

                                  };
            plalist = subdistrictlist.ToList();
            return plalist;
        }
        private List<PersonalInfoMemberAddressBodyStructure> volantiarlist(int? id)
        {
            if (id == 1)
            {
                var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                    join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                    join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                    join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                    orderby (pi.Date)
                                    where m.IDCardNo==null && m.Status==1
                                    select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi };
                return volantierlist.ToList();
            }
            else
            {
                var volantierlist = from pi in mrbd.PersonalInfo.ToList()
                                    join a in mrbd.Address.ToList() on pi.Id equals a.MemberId
                                    join bs in mrbd.BodyStructure.ToList() on pi.Id equals bs.MemberId
                                    join m in mrbd.Member.ToList() on pi.Id equals m.MemberId
                                    orderby (pi.Date)
                                    where m.IsActive.Equals(false)&& m.Status==2 && m.IDCardNo!=null
                                    select new PersonalInfoMemberAddressBodyStructure { a = a, bs = bs, m = m, pi = pi };

                return volantierlist.ToList();
            }

        }
        private List<PrasikkanName> prasikkanlist(string username)
        {
            List<PrasikkanName> plalist = new List<PrasikkanName>();
            var userid = db.User.Where(x => x.UserName.Equals(username)).Select(x => x.Id).FirstOrDefault();
            var udis = db.RangDistrict.Where(x => x.UserId.Equals(userid)).ToList();
            var subdis = from ud in udis.ToList()
                         join sd in cdb.SubDistric.ToList() on ud.DistrictId equals sd.countryid
                         select new SubDistric
                         {
                             Id = sd.Id,
                             Name = sd.Name
                         };
            var userlist = from u in db.User.ToList()
                           join ud in db.UserDistic.ToList() on u.Id equals ud.UserId
                           join sd in subdis.ToList() on ud.SubdisticId equals sd.Id
                           group u by new { u.Id, u.UserName } into g
                           select new User { Id = g.Key.Id, UserName = g.Key.UserName };

            var prasikkanlist = from up in cdb.PrasikkanName.ToList()
                                join us in userlist.ToList() on up.UserId equals us.Id
                                select new PrasikkanName
                                {
                                    Id = up.Id,
                                    Name = up.Name
                                };
            plalist = prasikkanlist.ToList();

            return plalist;
        }
        private List<Event> eventlist(string username)
        {
             List<Event> eventslist = new List<Event>();

             var userid = db.User.Where(x => x.UserName.Equals(username)).Select(x => x.Id).FirstOrDefault();
            var udis = db.RangDistrict.Where(x => x.UserId.Equals(userid)).ToList();
            var subdis = from ud in udis.ToList()
                         join sd in cdb.SubDistric.ToList() on ud.DistrictId equals sd.countryid
                         select new SubDistric
                         {
                             Id = sd.Id,
                             Name = sd.Name
                         };
         
            var userlist = from u in db.User.ToList()
                           join ud in db.UserDistic.ToList() on u.Id equals ud.UserId
                           join sd in subdis.ToList() on ud.SubdisticId equals sd.Id
                           group u by new { u.Id, u.UserName } into g
                           select new User { Id = g.Key.Id, UserName = g.Key.UserName };
        
            var eventlist = from e in mrbd.Event.ToList()
                                join us in userlist.ToList() on e.UserId equals us.Id
                                select new Event
                                {
                                    Id = e.Id,
                                    EventName = e.EventName
                                };
            eventslist = eventlist.ToList();
            return eventslist;
        }
        private int convertBanglaDigitToEnglish(string mobile)
        {
            //string number = "1234567890";
            int parsed_number = 0;
            bool match = Regex.IsMatch(mobile, "^[a-zA-Z0-9]*$");
            //string bengali_text = string.Concat(number.ToString().Select(c => (char)('\u09E6' + c - '0'))); // "??????????"
            if (match)
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
	}
}