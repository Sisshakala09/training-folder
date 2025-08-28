using System.Transactions;
using HRISAPL.Context;
using Microsoft.AspNetCore.Mvc;
using static HRISAPL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HRISAPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        // GET: api/<DepartmentsController>
        private readonly HRISContext _context;
        private Models.Department dept;

        public DepartmentsController(HRISContext context)
        {
            _context = context;
        }


        [HttpGet]
        [HttpGet]
        public IEnumerable<Department> Get()
        {
            return _context.Departments.ToList();
        }

        // GET api/<DepartmentsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (_context.Departments.Find(id) == null)
                return NotFound();
            else
                return Ok(_context.Departments.Find(id));
        }

        // POST api/<DepartmentsController>
        [HttpPost]
        public string Post([FromBody] Department dept)
        {
            try
            {
                _context.Departments.Add(dept);
                _context.SaveChanges();
                return "Successful addition of department by name " + dept.DeptName;
            }
            catch (Exception ex)
            {
                return "Failed with error " + ex.StackTrace.ToString();
            }
        }

        // PUT api/<DepartmentsController>/5
        
        // PUT api/<DepartmentsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Department dept)
        {
            _context.Departments.Update(dept);
            _context.SaveChanges();
        }

        // DELETE api/<DepartmentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Department dept = _context.Departments.Find(id);
            _context.Departments.Remove(dept);
            _context.SaveChanges();
        }
    }
}

