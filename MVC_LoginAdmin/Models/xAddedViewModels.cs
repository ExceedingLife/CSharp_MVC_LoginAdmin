using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_LoginAdmin.Models
{
    public class UserWithRole
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public UserWithRole() { }

        public UserWithRole(string em, string us, string ro)
        {
            em = Email;
            us = Username;
            ro = Role;
        }
    }

    // MODEL used for UPDATING User.
    public class UpdateViewModel
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }
    }

    //public class UserRolesViewModel
    //{
    //    public List<int> SelectedRoleIds { get; set; }
    //    public IEnumerable<SelectListItem> RoleChoices { get; set; }
    //}

}