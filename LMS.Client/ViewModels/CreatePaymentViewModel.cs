using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LMS.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace LMS.Web.ViewModels
{
    public class CreatePaymentViewModel
    {
        [Required]
        [Range(0.01, 1000000000)]
        public decimal Amount { get; set; }

        [Required]
        public string StudentId { get; set; } = "";

        [Required]
        public string ParentId { get; set; } = "";

   
    }
}
