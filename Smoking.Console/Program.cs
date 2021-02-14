using System.IO;
using Smoking.Infra;
using Smoking.Application;
using System;

namespace Smoking.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine($"CWD: {Directory.GetCurrentDirectory()}");
            if (args.Length < 1)
            {
                System.Console.WriteLine($"command required: ${args}");
                return;
            }
            var command = args[0];
            System.Console.WriteLine($"do {command}");
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var setting = Path.Join(home, ".smoking");
            if (!Directory.Exists(setting))
            {
                Directory.CreateDirectory(setting);
            }
            var smokingRepository = new SQLiteSmokingRepository(setting);
            var loginRepository = new FileLoginRepository(setting);

            switch (command)
            {
                case "migrate":
                    smokingRepository.Migrate().Wait();
                    break;
                case "init":
                    var initService = new InitSmokerService(smokingRepository);
                    var initCommand = new InitSmokerCommand
                    {
                        LimitPerDay = 20,
                        IntervalHour = 1,
                    };
                    initService.Execute(initCommand);
                    break;
                case "info":
                    var infoService = new InfoSmokerService(smokingRepository, loginRepository);
                    var infoCommand = new InfoSmokerCommand { };
                    infoService.Execute(infoCommand).Wait();
                    break;
                case "smoke":
                    var smokeService = new SmokeService(smokingRepository, loginRepository);
                    var smokeCommand = new SmokeCommand { };
                    smokeService.Execute(smokeCommand).Wait();
                    break;
                case "login":
                    var loginService = new LoginService(loginRepository);
                    var loginCommand = new LoginCommand { UserID = GetAggregateID(args) };
                    loginService.Execute(loginCommand).Wait();
                    break;
            }
        }

        private static string GetAggregateID(string[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("aggregate id required");
            }
            return args[1];
        }
    }
}
