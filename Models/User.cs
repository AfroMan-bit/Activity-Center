using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DojoCenter.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required (ErrorMessage = "Name is required!")]
        [MinLength(2, ErrorMessage = "Name must be 2 characters or more!")]
        public string Name {get;set;}

        [Required (ErrorMessage="Email is required")]
        [EmailAddress (ErrorMessage="Please enter a valid email")]
        public string Email {get;set;}

        [DataType(DataType.Password)]
        [ValidPassword]
        [Required (ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}

        [NotMapped]
        [Required (ErrorMessage="Confirm Password is required")]
        [Compare("Password", ErrorMessage="Confirm Password must match Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Party> CreatedParties{get; set;}
        // public List<Association> Parties {get;set;}

        public class ValidPassword : ValidationAttribute{
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {   
                var pass = Convert.ToString(value);
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSpecial = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
                //var hasChar = new Regex(@"[A-Z] *|[a-z]+");
                //var hasSpecial = new Regex(@"[#?!@$%^&*-]+");

                var isValidated = hasNumber.IsMatch(pass) && hasLowerChar.IsMatch(pass) && hasUpperChar.IsMatch(pass) && hasSpecial.IsMatch(pass);
                    if(isValidated){
                        return ValidationResult.Success;
                    }else{
                        return new ValidationResult("Password must include at least one uppercase letter, one lowercase letter, one number, & one special character");
                    }
            }
        }

    }
}