using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Web_Application.Models
{
    public class DepartmentPosition
    {
        [Key]
        public int No { get; set; }
        [DisplayName("Department")]
        public int DepartmentId { get; set; }
        [DisplayName("Department Name")]
        public string departmentName { get; set; }
        [DisplayName("Position")]
        public int PositionId { get; set; }
        [DisplayName("Position Name")]
        public string positionName { get; set; }
    }
}
