using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BAV.Models
{
    public class CommonContext:DbContext
    {
        public CommonContext()
            : base("BABCTGConnection")
              {
              }
           public DbSet<BloodGroup> BloodGroup { get; set; }
           public DbSet<Designation> Designation { get; set; }
           public DbSet<Distric> Distric { get; set; }
           public DbSet<Exam> Exam { get; set; }
           public DbSet<EyeColor> EyeColor { get; set; }
           public DbSet<Gender> Gender { get; set; }
           public DbSet<MemberType> MemberType { get; set; }
           public DbSet<PostCode> PostCode { get; set; }
           public DbSet<Status> Status { get; set; }
           public DbSet<SubDistric> SubDistric { get; set; }
           public DbSet<Ward> Ward { get; set; }
           public DbSet<Union> Union { get; set; }
           public DbSet<Religion> Religion { get; set; }
           public DbSet<Foot> Foot { get; set; }
           public DbSet<Inch> Inch { get; set; }
           public DbSet<PrasikkanName> PrasikkanName { get; set; }
           public DbSet<Type> Type { get; set; }
           public DbSet<Tranning> Tranning { get; set; }
        
    }
    [Table("Type")]
    public class Type
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

    }

    [Table("BloodGroup")]
    public class BloodGroup
    {  
        [Key]
       public int Id{get;set;}
       public string Name{get;set;}
		
    }

    [Table("Union")]
    public class Union
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubDisId { get; set; }
        public int PostId { get; set; }
    }
    [Table("Designation")]
    public class Designation
    {
          [Key]
       public int Id{get;set;}
       public string Name{get;set;}
		
    }
       [Table("Distric")]
    public class Distric
    {
       [Key]
       public int Id{get;set;}
       public string Name{get;set;}
       public int code { get; set; }
		
    }
       [Table("Exam")]
       public class Exam
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }
          

       }
       [Table("EyeColor")]
       public class EyeColor
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
       [Table("Gender")]
       public class Gender
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
       [Table("MemberType")]
       public class MemberType
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
       [Table("PostCode")]
       public class PostCode
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }
       

       }
           [Table("Status")]
       public class Status
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
          [Table("SubDistric")]
           public class SubDistric
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }
           public int countryid { get; set; }
           public string text { get; set; }
       }
      [Table("Ward")]
          public class Ward
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }
           public int Postid { get; set; }
           public int SubDisticId { get; set; }
           public int DisticId { get; set; }
           public string text { get; set; }
           public int UnionId { get; set; }

       }
       [Table("Religion")]
      public class Religion
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
       [Table("Inch")]
       public class Inch
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
       [Table("Foot")]
       public class Foot
       {
           [Key]
           public int Id { get; set; }
           public string Name { get; set; }

       }
          [Table("PrasikkanName")]
       public class PrasikkanName
       {
           [Key]
           public int Id { get; set; }
           [Required]
           public string Name { get; set; }
           public Guid UserId { get; set; }
           public DateTime CreationDate { get;set; }

       }

      [Table("Tranning")]
       public class Tranning
      {
          public int  Id{get;set;}
          [Required]
         public string  Name{get;set;}
          public Guid UserId{get;set;}
          public DateTime CreationDate{get;set;}
        }
    
    
}