using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RepoContextNamespace;

namespace Repos.Repos
{
    public class Dept:IDept
    {
        private readonly RepoContext _context;
        public Dept(RepoContext context)
        {
            _context = context;
        }
        public string AddDept(Department dept)
        {
            try
            {
                _context.Departments.Add(dept);
                _context.SaveChanges();
                return "Successful addition of department";
            }
            catch(Exception ex)
            {
                return "Department Addition failed with error:" + ex.Message;
            }
        }

        public string EditDept(Department dept)
        {
            return "Yet to implement";
        }

        public List<Department> GetDepartments()
        {
            return _context.Departments.ToList();
            
        }

        public string RemoveDept(int id)
        {
            return "Yet to implement";
        }
    }
}
