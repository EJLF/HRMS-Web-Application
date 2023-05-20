using HRMS_Web_Application.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace HRMS_Web_Application.Models
{
    public class RegisterEmployeeViewModel
    {
        public RegisterEmployeeViewModel()
        {
            DateHired = DateTime.Today; // set the date to today
        }
        [Required]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-ZñÑ ]+$", ErrorMessage = "This is not a valid Name. Special characters are not allowed.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        [RegularExpression(@"^[a-zA-ZñÑ ]+$", ErrorMessage = "This is not a valid Name. Special characters are not allowed.")]
        [Required]
        public string MiddleName { get; set; }
        [DisplayName("Last Name")]
        [RegularExpression(@"^[a-zA-ZñÑ ]+$", ErrorMessage = "This is not a valid Name. Special characters are not allowed.")]
        [MinLength(2)]
        [Required]
        public string LastName { get; set; }
        [DisplayName("Full Name")]
        public string FullName => string.Join(" ", FirstName, MiddleName, LastName);
        [Required]
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression("(09)[0-9]{9}", ErrorMessage = "This is not a valid phone number")]
        [DisplayName("Phone Number")]
        [MaxLength(11)]
        public string Phone { get; set; }

        // Foreign Key
        [Required]
        [DisplayName("Department")]
        public int DepartmentId { get; set; }
        
        [ForeignKey("DepartmentId")]
        [DisplayName("Department")]
        public Department? Department { get; set; }
        [Required]
        [DisplayName("Position")]
        public int PositionId { get; set; }
        [ForeignKey("PositionId")]
        [DisplayName("Position")]
        public Position? Position { get; set; }
        public string EmployeeType { get; set; }

        /*//Benefits
        [Required]
        [Display(Name = "SSS Number")]
        [MinLength(12)]
        [MaxLength(12)]
        public string SSSNumber { get; set; }
        [Required]
        [Display(Name = "PagIbig Number")]
        [MinLength(14)]
        [MaxLength(14)]
        public string PagIbigId { get; set; }
        [Required]
        [Display(Name = "Philhealth Number")]
        [MinLength(14)]
        [MaxLength(14)]
        public string PhilHealthId { get; set; }*/


        //Address
        [Required]
        public string Street { get; set; }
        [Required]
        public string Barangay { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        [RegularExpression("[0-9]{4}", ErrorMessage = "This is not a valid Postal Code")]
        public string PostalCode { get; set; }

        //Account Status
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateHired { get; set; }

        public bool ActiveStatus { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [PasswordPropertyText]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirm password doesnt match")]
        public string ConfirmPassword { get; set; }


    }
}
