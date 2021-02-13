using System;
using System.Threading.Tasks;
using Smoking;
using Smoking.Domain;

namespace Smoking.Application
{
    public class InfoSmokerCommand
    {
        public string AggregateID { get; set; }
    }

    public class InfoSmokerService
    {
        private SmokerRepository repository;

        public InfoSmokerService(SmokerRepository repository)
        {
            this.repository = repository;
        }

        public async Task Execute(InfoSmokerCommand command)
        {
            var aggregateID = Guid.Parse(command.AggregateID);
            var smoker = await this.repository.Get(aggregateID);
            Console.WriteLine($"aggregate id: {smoker.AggregateID}");
            Console.WriteLine($"=== stats ===");
            Console.WriteLine($"today smoked: {smoker.TodayConsumed}");
            Console.WriteLine($"last smoked: {smoker.LastSmoked}");
            Console.WriteLine($"=== setting ===");
            Console.WriteLine($"interval: {smoker.Interval}");
            Console.WriteLine($"limit per day: {smoker.LimitPerDay}");
        }
    }
}
