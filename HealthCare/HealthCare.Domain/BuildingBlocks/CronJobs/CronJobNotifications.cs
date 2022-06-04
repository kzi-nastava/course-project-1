using HealthCare.Domain.BuildingBlocks.Mail;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.BuildingBlocks.CronJobs
{
    public class CronJobNotifications : CronJobService
    {

        public CronJobNotifications(IScheduleConfig<CronJobNotifications> config) : base(config.CronExpression, config.TimeZoneInfo, config.PrescriptionService)
        {
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            Console.WriteLine("radi");
            List<string> emails = await _prescriptionService.GetAllReminders();
            Console.WriteLine(emails.Count);
            foreach (string item in emails)
            {
                MailSender sender = new MailSender("usi2022hospital@gmailcom", item);
                sender.SetBody("Podsetnik za lek.");
                sender.SetSubject("Uskoro morate popiti lek!");
                sender.Send();
            }
        }
    }
}
