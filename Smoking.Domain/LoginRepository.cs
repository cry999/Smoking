using System;
using System.Threading.Tasks;

namespace Smoking.Domain
{
    public interface LoginRepository
    {
        Task<Guid> GetLoggedinUser();
        Task Login(Guid userID);
        Task Logout();
    }
}
