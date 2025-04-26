using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.AdminViewModels
{
    public class ParentDetailsViewModel
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string ProfilePictureURL { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }  // Updated to use PhoneNumber
        public string Occupation { get; set; }
    }
}
