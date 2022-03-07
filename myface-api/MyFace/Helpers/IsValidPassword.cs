using System;
using System.ComponentModel.DataAnnotations;

namespace MyFace.Helpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsValidPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as string;
            var isValid = true;
            char[] charArray = inputValue.ToCharArray();
            int countLowerCase=0;
            int countUpperCase=0;
            int countDigit=0;
            foreach (char c in charArray)
            {
                if (Char.IsNumber(c))
                    countDigit+=1;
                else if (Char.IsUpper(c))
                    countUpperCase+=1;
                else if (Char.IsLower(c))
                    countLowerCase+=1;
            }
            if (!(countDigit>0 && countLowerCase>0 && countUpperCase>0))
                isValid=false;

            return isValid;
        }
    }
}