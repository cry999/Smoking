using System;
using System.Threading.Tasks;
using Smoking.Domain;

namespace Smoking.Application
{
    public class LoginCommand
    {
        public string UserID { get; set; }
    }

    public class LoginService
    {
        private LoginRepository loginRepository;

        public LoginService(LoginRepository loginRepository)
        {
            this.loginRepository = loginRepository;
        }

        public async Task Execute(LoginCommand command)
        {
            var userID = Guid.Parse(command.UserID);
            await this.loginRepository.Login(userID);
        }
    }
}
