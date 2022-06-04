using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class UtilityService
    {
        public static DateTime RemoveSeconds(DateTime dateTime)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;
            int hour = dateTime.Hour;
            int minute = dateTime.Minute;
            int second = 0;
            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
