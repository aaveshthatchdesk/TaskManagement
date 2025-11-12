using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task.Application
{
    public class StrictEmailAttribute:ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return true;
            string email=value.ToString()??"";

            var emailPattern= @"^[^@\s]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,}$";
            if (!Regex.IsMatch(email,emailPattern))
            {
                return false;
            }
            if (Regex.IsMatch(email, @"(\.[A-Za-z]{2,})\1$", RegexOptions.IgnoreCase))
                return false;

            return true;
        }
    }
}
