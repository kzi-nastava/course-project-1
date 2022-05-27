using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.BuildingBlocks.CronJobs
{
    public class CronJobNotifications : CronJobService
    {
        public CronJobNotifications(string cronExpression, TimeZoneInfo timeZoneInfo) : base(cronExpression, timeZoneInfo)
        {

        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            
        }
    }
}
