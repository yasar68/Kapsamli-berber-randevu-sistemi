using System;
using System.ComponentModel.DataAnnotations;

namespace BerberApp.API.ValidationAttributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.UtcNow;
            }
            return false;
        }
    }
}