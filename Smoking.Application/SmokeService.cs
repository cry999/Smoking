using System;
using System.Threading.Tasks;
using Smoking.Domain;

namespace Smoking.Application
{
    public class SmokeCommand
    {
    }

    public class SmokeService
    {
        private SmokerRepository smokerRepository;
        private LoginRepository loginRepository;

        public SmokeService(SmokerRepository smokerRepository, LoginRepository loginRepository)
        {
            this.smokerRepository = smokerRepository;
            this.loginRepository = loginRepository;
        }

        public async Task Execute(SmokeCommand command)
        {
            var aggregateID = await loginRepository.GetLoggedinUser();
            var smoker = await this.smokerRepository.Get(aggregateID);
            smoker.Smoke(DateTimeOffset.Now);
            await this.smokerRepository.Put(smoker);
        }
    }
}
