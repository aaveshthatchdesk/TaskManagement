using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
  public  class ChangePasswordDto
    {

        public int UserId {  get; set; }

        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
        public string NewPassword { get; set; }= string.Empty;


        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set;} = string.Empty;

    }
}
