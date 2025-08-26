using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repos.Repos;

namespace Services
{
    public class ServiceClass<T> where T : class
    {
        private readonly IRepogeneric<T> _repo;
        public ServiceClass(IRepogeneric<T> repo)
        {
            _repo = repo;
        }
        public IEnumerable<T> GetList()
        {
            return _repo.GetAll();
        }
    }
    
    }

