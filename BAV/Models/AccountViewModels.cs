using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace BAV.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("BABCTGConnection")
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<UserDistic> UserDistic { get; set; }
        public DbSet<Platun> Platun { get; set; }
        public DbSet<UserPlatun> UserPlatun { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RangDistrict> RangDistrict { get; set; }
        
    }

    [Table("User")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Createdby { get; set; }
      
        
    }
     [Table("Role")]
    public class Role
    {
       public int Id{get;set;}
       public string RoleName{get;set;}
    }
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int roleid { get; set; }
        public int DistricId{ get; set; }

    }
     [Table("UserDistic")]
    public class UserDistic
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
    
        public int SubdisticId { get; set; }

    }
     [Table("RangDistrict")]
    public class RangDistrict
    {
      public Guid  Id{get;set;} 
      public Guid  UserId{get;set;}
      public int DistrictId { get; set; }
    }
    [Table("Platun")]
    public class Platun
    {
     public Guid  Id{get;set;}
     public Guid UserId{get;set;}
     [Required]
     public string PlatuneName{get;set;}
     public DateTime CreationDate{get;set;}
     public bool IsActive { get; set; }
     public string  InactiveRemarks{get;set;}
     public DateTime InActiveDate{get;set;}
        [NotMapped]
     public int distric { get; set; }
     public int SubDistrcId { get; set; }
		
    }
    [Table("UserPlatun")]
    public class UserPlatun
    {
        public int Id { get; set; }
    public Guid UserId{get;set;}
    public Guid PlatunId { get; set; }	
    }
    public class PlaDistric
    {
        public string PlatuneName { get; set; }
        public DateTime creationdte { get; set; }
        public string subname { get; set; }
        public string disnane { get; set; }
        public Guid PlatunId { get; set; }
        public bool Isactive { get; set; }
        public int subdistrictid { get; set; }
        public int districtid { get; set; }
    }
    public class PlatunList
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PlatunName { get; set; }
        public int total { get; set; }
    }
    public class uurd
    {
        public User u { get; set; }
        public UserRole ur { get; set; }
        public Distric d { get; set; }
        public Role r { get; set; }
        public string password { get; set; }
    }
  
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int role { get; set; }

        public int distric { get; set; }
        public string Createdby { get; set; }
        //public int subdistric { get; set; }
    }
}
