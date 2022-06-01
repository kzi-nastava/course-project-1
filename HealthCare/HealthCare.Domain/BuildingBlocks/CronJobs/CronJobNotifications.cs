using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.BuildingBlocks.CronJobs
{
    public class CronJobNotifications : CronJobService
    {
        public CronJobNotifications(IScheduleConfig<CronJobNotifications> config) : base(config.CronExpression, config.TimeZoneInfo)
        {

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            Console.WriteLine("radi");
        }
    }
}
