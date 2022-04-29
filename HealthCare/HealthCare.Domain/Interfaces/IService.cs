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
    
    public class DataIsNullException : Exception
    {
        public DataIsNullException() : base("Data doesn't exist")
        {
        }
    }
    public class AlreadyHandledException : Exception
    {
        public AlreadyHandledException() : base("Cannot approve/reject the approval because it has already been handled")
        {
        }
    }
    public class AntiTrollException : Exception
    {
        public AntiTrollException() : base("Patient is blocked via AntiTroll mechanism and cannot make requests")
        {
        }
    }
    public class DoctorNotAvailableException : Exception
    {
        public DoctorNotAvailableException() : base("Doctor is not available for given start time")
        {
        }
    }
    public class PatientNotAvailableException : Exception
    {
        public PatientNotAvailableException() : base("Patient is not available for given start time")
        {
        }
    }
    public class NoFreeRoomsException : Exception
    {
        public NoFreeRoomsException() : base("There are no available rooms for current procedure")
        {
        }
    }
    
    
}
