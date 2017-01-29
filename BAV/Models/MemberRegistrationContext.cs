using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BAV.Models
{
    public class MemberRegistrationContext:DbContext
    {
        public MemberRegistrationContext()
            : base("BABCTGConnection")
              {
              }
                      public DbSet<PersonalInfo> PersonalInfo { get; set; }
                      public DbSet<Address> Address { get; set; }
                      public DbSet<AnotherInfo> AnotherInfo { get; set; }
                       public DbSet<BodyStructure> BodyStructure { get; set; }
                       public DbSet<EducationalQualification> EducationalQualification { get; set; } 
                       public DbSet<Image> Image { get; set; }
                       public DbSet<Member> Member { get; set; }
                       public DbSet<Prasikkan> Prasikkan { get; set; } 
                        public DbSet<UttaradikaryInfo> UttaradikaryInfo { get; set; }
                        public DbSet<Event> Event { get; set; }
                        public DbSet<EventMember> EventMember { get; set; }
                        public DbSet<ContactBasedMember> ContactBasedMember { get; set; }
                        public DbSet<HourlyBasedMember> HourlyBasedMember { get; set; }
                        public DbSet<MonthBasedMember> MonthBasedMember { get; set; }
                        public DbSet<message> message { get; set; }
                        public DbSet<ComandarSpecification> ComandarSpecification { get; set; }
                        public DbSet<MemberPlatunChange> MemberPlatunChange { get; set; }
                        public DbSet<AnsarInfo> AnsarInfo { get; set; }
                        public DbSet<Ansartranning> Ansartranning { get; set; }
                        public DbSet<AnsarAddress> AnsarAddress { get; set; }
        
        
        
    }

    [Table("PersonalInfo")]
    public class PersonalInfo
    {
     [Key]
     public Guid  Id{get;set;}
   
     [Required]
     public string BanglaName{get;set;}
     [Required]
     public string EnglishName{get;set;}
     [Required]
     public int DesignationId{get;set;}
     [Required]
     public string BanglaFatherName{get;set;}
     [Required]
     public string EnglishFatherName{get;set;}
     [Required]
     public string BanglaMotherName{get;set;}
     [Required]
     public string EnglishMotherName{get;set;}
     [Required]
     public string DOB{get;set;}
     [Required]
     public string MaritalStatus{get;set;}
  
     public string WORHName{get;set;}
 
     public string WorHOccupation{get;set;}
     public string NID{get;set;}
     public string DOBSNo{get;set;}
     public int dp { get; set; }
     [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
     public DateTime Date{get;set;}
     public string occupation { get; set; }
     public int ReligionId { get; set; }
     [Required]
     public string mobile { get; set; }
     public string faormomobile { get; set; }
     public int educationalQuId { get; set; }

     public int ps { get; set; }
     public int p { get; set; }
     public int pst { get; set; }

     public int po { get; set; }
     public int pos { get; set; }
     public int pot { get; set; }

        [NotMapped]
        public Address Addresss {get; set;}
        [NotMapped]
        public BodyStructure BodyStructures { get; set; }
        [NotMapped]
        public Image Images { get; set; }
        [NotMapped]
        public string platunername { get; set; }
        [Required]
        public Guid platunId { get; set; }
      


    }

     [Table("Address")]
    public class Address
     {
      [Key]
      public Guid Id{get;set;}
     [Required]
     public Guid MemberId{get;set;}
     [Required]
     public string PresAddress{get;set;}
     [Required]
     public int PerSubDistric { get; set; }
     [Required]
     public int PerDistric { get; set; }
     [Required]
     public int PresSubDistric { get; set; }
     [Required]
     public int PresDistric { get; set; }
     [Required]
     public string PerAddress { get; set; }
     public string PresPostCodeId { get; set; }
      [NotMapped]
      public string PresPostCodeIdM { get; set; }
      public string PresWard { get; set; }
      [NotMapped]
      public string PresWardM { get; set; }
      public string PresUnion { get; set; }
      public string PerPostCodeId { get; set; }
       [NotMapped]
       public string PerPostCodeIdM { get; set; }
       public string PerWard { get; set; }
       [NotMapped]
       public string PerWardM { get; set; }
       public string PerUnion { get; set; }
  
		
        }
     [Table("AnotherInfo")]
    public class AnotherInfo
      {
       [Key]
       public Guid  Id{get;set;}
       [Required]
       public Guid MemberId{get;set;}
       public string wphone{get;set;}
       [Required]
       public string wmobile{get;set;}
       public string wemail{get;set;}
       public string Rphone{get;set;}
       [Required]
       public string Rmobile{get;set;}
       public string Remail{get;set;}
		
      }
    [Table("BodyStructure")]
    public class BodyStructure
    {
        [Key]
     public Guid  Id{get;set;}
     [Required]
     public Guid MemberId{get;set;}
     public int footid { get; set; }
     public int inchid { get; set; }
     public Decimal HeightMC{get;set;}
     public string WeightKg{get;set;}
     public int GenderId{get;set;}
     public int EyeColorId{get;set;}
     public int BloodGroupId{get;set;}
     public string SanaktakaranSign{get;set;}
     public string bodycolor { get; set; }
		
    }
     [Table("EducationalQualification")]
    public class EducationalQualification
    { 
      [Key]
      public Guid Id{get;set;}
      [Required]
      public Guid MemberId{get;set;}
      [Required]
      public int ExamId{get;set;}
      public string SchoolName{get;set;}
      public string  PassingYear{get;set;}
      public string Grade{get;set;}
		
   }
     [Table("Image")]
     public class Image
     {
         [Key]
         public Guid Id{get;set;}
        
         public Guid MemberId{get;set;}
       
         public string ImageofBsign { get; set; }
         [Required]
         public string picture { get; set; }
         public string NIDImage{get;set;}
         public string DOBSImage { get; set; }
         [NotMapped]
         public HttpPostedFileBase NIDImages { get; set; }
         [NotMapped]
         public HttpPostedFileBase pictures { get; set; }
         [NotMapped]
         public HttpPostedFileBase DOBSImaged { get; set; }
         [NotMapped]
         public HttpPostedFileBase ImageofSigned { get; set; }
     
	

     }
     [Table("Member")]
     public class Member
     {
         [Required]
          public Guid? Id{get;set;}
         [Required]
          public Guid MemberId{get;set;}
          public int MemberType{get;set;}
          public int Status{get;set;}
          public DateTime CheckBeforeApproveddate{get;set;}
          public Guid  CheckBeforeApproved{get;set;}
          public DateTime ApporovedDate{get;set;}
          public Guid ApprovedBy{get;set;}
          public string PersonalNumber{get;set;}
          public string IDCardNo{get;set;}
          public string platunername { get; set; }
          public bool IsActive { get; set; }
          public string Remarks { get; set; }
          public DateTime platunchangedate { get; set; }
          public int mid { get; set; }

     }
       [Table("Prasikkan")]
     public class Prasikkan
     {
          
             public Guid Id{get;set;}
            
             public Guid MemberId{get;set;}
             public int PraNameId { get; set; }
             public string PraInstitudeName{get;set;}
             public string StartDate{get;set;}
             public string EndDate{get;set;}
             public string SNo{get;set;}
             public bool IsActive { get; set; }
             [NotMapped]
             public string PraName { get; set; }

     }
    
        [Table("UttaradikaryInfo")]
     public class UttaradikaryInfo
     {
      [Key]
      public Guid Id{get;set;}
      [Required]
      public Guid MemberId{get;set;}
      public string  UttaradikaryName{get;set;}
      public string Relation{get;set;}
      public string UttaradikaryPart{get;set;}
      public string  Mobile{get;set;}
      public string Phone{get;set;}
		
		
     }



    public class PersonalInfoMemberAddressBodyStructure
    {
        public PersonalInfo pi { get; set; }
        public Member m { get; set; }
        public Address a { get; set; }
        public BodyStructure bs { get; set; }
        public Image i { get; set; }
        public string Name { get; set; }
        public Prasikkan p { get; set; }
        public Guid platunid { get; set; }
        public string planame { get; set; }
        public Exam ex { get; set; }
        public string disignation { get; set; }
        public string subdis { get; set; }


    }
    public class Personevent
    {
        public PersonalInfo pi { get; set; }
        public Event e { get; set; }
        public Member m { get; set; }
        public EventMember em { get; set; }
        public string planame { get; set; }
     

    }

    public class prname
    {
        public Prasikkan p;
        public PrasikkanName pn;
      
    }
    public class praname
    {
       
        public Ansartranning at;
        public Tranning t;
    }
    public class PersonalInfoMemberAddressBodyStructureReligion
    {
        public Guid Id;
        public string platunname;
        public string bname;
        public string ename;
        public string bfname;
        public string efname;
        public string bmname;
        public string emname;
        public string designation;
        public string Imge;
        public string membersign;
        public int sandadno;
        public string maritalstatus;
       
        public string smname;
        public string smpasa;
        public string nid;
        public string dobno;
        public string dob;

        public string occupation;
        public string religion;

         public string presaddress;
         public string prespostcode;
         public string presunion;
         public string presthana;
         public string presdist;
         public string presunions;
      
         public string paddress;
         public string prpostcode;
         public string perunion;
         public string perthana;
         public string perdist;
         public string perunions;
        

         public string height;
         public string weight;
         public string zender;
         public string cyecolor;
         public string bodycolor;
         public string bloodgroup;
         public string sanaktakarn;

         public int statusid;
         public string membercode;

         public string mobile;
         public int ps { get; set; }
         public int p { get; set; }
         public int pst { get; set; }

         public int po { get; set; }
         public int pos { get; set; }
         public int pot { get; set; }
         public bool isactive { get; set; }


    }
    [Table("Event")]
    public class Event
    {
     public Guid Id{get;set;}
        [Required]
     public string EventName{get;set;}   
        [Required]
     public string Date{get;set;}
        [Required]
     public string Area{get;set;}
     public string enddate { get; set; }
     public bool IsActive{get;set;}
     public Guid UserId { get; set; }
     
     

    }
    [Table("EventMember")]
    public class EventMember 
    {
     public Guid   Id{get;set;}
     public Guid memberId{get;set;}
     public Guid eventId{get;set;}
     public DateTime starttime{get;set;}
     public DateTime endtime{get;set;}	
    public int totalhour{get;set;}
    public bool IsActive { get; set; }
    public string workfor { get; set; }
    public bool IsPaid { get; set; }
   
    }
    public class eve
    {
        public Event even { get; set; }
        public EventMember EventMember { get; set; }

        public int day { get; set; }
        public int month { get; set; }
    }

    [Table("ContactBasedMember")]
    public class ContactBasedMember
    {
     public Guid  Id{get;set;}
       public Guid EventMemberId{get;set;}
       public DateTime StartDate{get;set;}
       public string  dutitime{get;set;}
       public decimal amount { get; set; }
       public DateTime EndDate{get;set;}
		
}


    [Table("HourlyBasedMember")]
    public class HourlyBasedMember
    {
        public Guid Id { get; set; }
        public Guid EventMemberId { get; set; }
        public int perdayduty { get; set; }
        public DateTime Startingdate { get; set; }
        public string settime { get; set; }
        public decimal amount { get; set; }
        public DateTime Enddate { get; set; }
        public decimal hmhhw { get; set; }

    }
    [Table("MonthBasedMember")]
    public class MonthBasedMember
    {
        public Guid Id { get; set; }
        public Guid EventMemberId { get; set; }
        public DateTime dutystart { get; set; }
        public string settime { get; set; }
        public decimal salary { get; set; }
        public DateTime dutyend { get; set; }
        public decimal perdaycost { get; set; }
        public int hmgs { get; set; }

    }

    public class monthlyhourlycontact
    {
        public MonthBasedMember mbs{get;set;}
        public HourlyBasedMember hbm { get; set; }
        public ContactBasedMember cbm { get; set; }
    }
    [Table("message")]
    public class message
    {
      public Guid Id{get;set;}
      public Guid MemberId{get;set;}
      public string messages{get;set;}
      public DateTime date{get;set;}
        }

    [Table("ComandarSpecification")]
     public class ComandarSpecification
      {
         public int Id{get;set;}
          [Required]
         public string FirstName{get;set;}
             [Required]
         public string  LastName{get;set;}
             [Required]
         public string SignatureImage{get;set;}
             [Required]
         public int DistricId{get;set;}
             [Required]
         public int comandartype { get; set; }
         public bool IsActive{get;set;}	
         public DateTime CreationDate{get;set;}	
         public Guid UserId{get;set;}
         public string InactiveRemarks { get; set; }
         [NotMapped]
         public HttpPostedFileBase signimage { get; set; }
      }
    [Table("MemberPlatunChange")]
    public class MemberPlatunChange
    {
        [Key]
    public Guid Id{get;set;}
    public Guid PlatunId{get;set;}
    public Guid MemberId { get; set; }
    public DateTime StartDate{get;set;}
    public DateTime  EndDate{get;set;}
  
   }
    public class  MemberPlatunChangeList
    {
          public string PlatunName{get;set;}
          public DateTime   startdate{get;set;}
          public DateTime enddate { get; set; }
    }
     [Table("AnsarInfo")]
    public class AnsarInfo
     {
                   public Guid  Id{get;set;}
                   public int  Upid	{get;set;}
                   public int  Degisnation{get;set;}
         [Required]
                   public string  Name{get;set;}
           [Required]
                   public string    FatherName{get;set;}
                 public int education{get;set;}
           [Required]
                 public string occupation{get;set;}
           [Required]
                 public string mobile{get;set;}
                 public string age	{get;set;}
                 public DateTime creationdate{get;set;}
                 public Guid userId { get; set; }
                 public int personno { get; set; }
        }

     [Table("Ansartranning")]
     public class Ansartranning
     {

         public Guid Id { get; set; }

         public Guid MemberId { get; set; }
         public int PraNameId { get; set; }
         public string PraInstitudeName { get; set; }
         public string StartDate { get; set; }
         public string EndDate { get; set; }
         public string SNo { get; set; }
         public bool IsActive { get; set; }

     }

    public class aninfoantrad
    {
        public AnsarInfo ai { get; set; }
        public Ansartranning at { get; set; }
        public AnsarAddress a { get; set; }
        public string digname { get; set; }
        public string education { get; set; }
        public string disname{get;set;}
        public string sudisname { get; set; }
    }

   public class  ansermember
                            {
                              public Guid  Id;
                              public string  bname;
                              public string  faname;
                                public string  bmobile;
                             public int   personno ;
                                public string  designation ;
                                 public string education;
                                 public string occupation ;

                                public string  praname;
                                public string  prinstitude;
                                 public string pstart;
                                 public string pend;
                                

                            public string presaddress;
         public string prespostcode;
         public string presunion;
         public string presthana;
         public string presdist;
         public string presunions;
      
         public string paddress;
         public string prpostcode;
         public string perunion;
         public string perthana;
         public string perdist;
         public string perunions;
                            };

      [Table("AnsarAddress")]
    public class AnsarAddress
    {
      public Guid  Id{get;set;}
       public Guid MemberId{get;set;}
      public string PresAddress{get;set;}
      [Required]
      public int PerSubDistric { get; set; }
      [Required]
      public int PerDistric { get; set; }
      [Required]
      public int PresSubDistric { get; set; }
      [Required]
      public int PresDistric { get; set; }
      [Required]
      public string PerAddress { get; set; }

      public string PerPostCodeId { get; set; }
      [NotMapped]
      public string PerPostCodeIdM { get; set; }
      public string PerWard { get; set; }
      [NotMapped]
      public string PerWardM { get; set; }
      public string PerUnion { get; set; }
    }
    public class eventmemberlist
    {
                                   public string   membername ;
                                   public string   designation ;
                                    public string   membercode;
                                     public string  mobile;
                                     public string platun;
    }
}





  