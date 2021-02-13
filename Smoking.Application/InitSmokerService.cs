using System;
using Smoking.Domain;

namespace Smoking.Application
{
    public class InitSmokerCommand
    {
        public int LimitPerDay { get; set; }
        public int IntervalHour { get; set; }
    }

    public class InitSmokerService
    {
        private SmokerRepository repository;

        public InitSmokerService(SmokerRepository repository)
        {
            this.repository = repository;
        }

        public void Execute(InitSmokerCommand command)
        {
            var smoker = new Smoker(command.LimitPerDay, TimeSpan.FromHours(command.IntervalHour));
            Console.WriteLine($"smoker id: {smoker.AggregateID}");
            this.repository.Put(smoker);
        }
    }
}
