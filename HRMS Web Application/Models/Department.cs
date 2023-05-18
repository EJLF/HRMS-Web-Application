using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HRMS_Web_Application.Models
{
    public class Department
    {
        
        [Key]
        public int DeptId { get; set; }
        [Required]
        [MinLength(2)]
        [DisplayName("Department Name")]
        public string DeptName { get; set; }

        public Department() { }

        public Department(int deptId, string deptName)
        {
            DeptId = deptId;
            DeptName = deptName;
        }
    }
}
