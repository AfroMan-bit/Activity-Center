using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace DojoCenter.Models
{
    public class Party
    {
        [Key]
        public int PartyId {get; set;}

        [Required (ErrorMessage = "Activity Title is required")]
        public string Title {get; set;}

        [DateInFuture]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date {get; set;}

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "{HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Time {get;set;}

        public int Duration {get; set;}
        public string DurationType {get; set;}

        [Required (ErrorMessage = "Description of Activity is required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
        public string Description {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public User Planner {get; set;}
        public int PlannerId {get; set;}
        public List<Association> Users {get;set;}

        public class DateInFuture : ValidationAttribute{
            public override string FormatErrorMessage(string name){
                return "Activity Date should be a future date";
            }
            protected override ValidationResult IsValid(object objValue, ValidationContext validationContext){
                var dateValue = objValue as DateTime? ?? new DateTime();
                if (dateValue.Date < DateTime.Now.Date){
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                return ValidationResult.Success;
            }
        }


    }
}