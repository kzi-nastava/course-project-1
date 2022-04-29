using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces 
{
    public interface IService<T> where T : class 
    {
        Task<IEnumerable<T>> GetAll();
    }
}
