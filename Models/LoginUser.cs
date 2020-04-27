using System.ComponentModel.DataAnnotations;
namespace DojoCenter.Models
{
    public class LoginUser
    {
        [Required (ErrorMessage="Email is required")]
        [EmailAddress (ErrorMessage="Please enter a valid email")]
        public string LoginEmail {get;set;}

        [DataType(DataType.Password)]
        [Required (ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage="Hint: Password must be 8 characters or longer & Must include atleast one number, letter, and special symbol!")]
        public string LoginPassword {get;set;}
    }
}