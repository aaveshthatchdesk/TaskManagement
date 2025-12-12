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
        //public bool AllowPast { get; set; } = false;
        public override bool IsValid(object? value)
        {
            
            if (value == null) return true; 

            DateTime date = (DateTime)value;

        
        //      if (AllowPast)
        //return true;
            return date.Date >= DateTime.Today;
        }
    }
}
