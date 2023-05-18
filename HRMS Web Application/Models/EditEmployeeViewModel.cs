//using HRMS_Web_Application.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Web_Application.Models
{
    public class EditEmployeeViewModel
    {
        public string Id { get; set; }
        [Required]
        [MinLength(2)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        [Required]
        public string MiddleName { get; set; }
        [DisplayName("Last Name")]
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
        public string? Email { get; set; }

        // Foreign Key
        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        [DisplayName("Position")]
        public int? PositionId { get; set; }
        public string? PositionName { get; set; }
        public string? EmployeeType { get; set; }


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
        public DateTime DateHired { get; set; }

        public bool ActiveStatus { get; set; }

    }
}
