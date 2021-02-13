using System;
using System.Threading.Tasks;
using Smoking.Domain;

namespace Smoking.Application
{
    public class SmokeCommand
    {
        public string AggregateID { get; set; }
    }

    public class SmokeService
    {
        private SmokerRepository repository;

        public SmokeService(SmokerRepository repository)
        {
            this.repository = repository;
        }

        public async Task Execute(SmokeCommand command)
        {
            var aggregateID = Guid.Parse(command.AggregateID);
            var smoker = await this.repository.Get(aggregateID);
            smoker.Smoke(DateTimeOffset.Now);
            await this.repository.Put(smoker);
        }
    }
}
