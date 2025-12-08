using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application
{
   public class FutureDateAttribute:ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
            if (value == null) return true; 

            DateTime date = (DateTime)value;

            return date.Date >= DateTime.Today;
        }
    }
}
