using System.ComponentModel.DataAnnotations;

namespace HRISAPL
{
    public class Models
    {
        public class Department
        {
            [Key]
            public int DeptId { get; set; }       // Primary Key
            public string DeptName { get; set; }  // Department name
            //public string Name { get; internal set; }

            // Navigation property (One department → Many employees)
            //public ICollection<Employee> Employees { get; set; }
        }
    }

}

