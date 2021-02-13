using System;
using System.Threading.Tasks;

namespace Smoking.Domain
{
    public interface SmokerRepository
    {
        Task<Smoker> Get(Guid aggregateID);
        Task Put(Smoker smoker);
    }
}
