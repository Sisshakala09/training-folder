using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Department
    {
        [Key]
        public int DeptId { get; set; }       // Primary Key
        public string DeptName { get; set; }  // Department name

        // Navigation property (One department → Many employees)
        //public ICollection<Employee> Employees { get; set; }
    }
}
