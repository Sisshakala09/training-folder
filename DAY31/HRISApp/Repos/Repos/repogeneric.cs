using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepoContextNamespace;

namespace Repos.Repos
{
    public interface IRepogeneric<T> where T:class
    {
        IEnumerable<T> GetAll();
        T GetByID(int? id);
        string Create(T Obj);
        string Delete(T Obj);
        string Update(T Obj);
    }
    public class Repogeneric<T> : IRepogeneric<T> where T : class
    {

        private DbSet<T> db = null;
        private readonly RepoContext _context;
        public Repogeneric (RepoContext context)
        {
            _context = context;
            db = _context.Set<T>();
        }
        public string Create(T Obj)
        {
            try
            {
                _context.Add(Obj);
                _context.SaveChanges();
                return "Successful addition of record";
            }
            catch(Exception ex)
            {
                return "Failed with error" + ex.StackTrace;
            }
        }

        public string Delete(T Obj)
        {
            try
            {
                _context.Remove(Obj);
                _context.SaveChanges();
                return "Successful deletion of recoed";
            }
            catch(Exception ex)
            {
                return "Deletion failed with error" + ex.StackTrace;
            }
        }

        public IEnumerable<T> GetAll()
        {
            return db.ToList();
        }

        public T GetByID(int? id)
        {
            return db.Find(id);
        }

        public string Update(T Obj)
        {
            throw new NotImplementedException();
        }
    }
}
