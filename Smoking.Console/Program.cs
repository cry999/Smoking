using System.IO;
using Smoking.Infra;
using Smoking.Application;

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
            var repo = new SQLiteSmokingRepository();

            switch (command)
            {
                case "migrate":
                    repo.Migrate().Wait();
                    break;
                case "init":
                    var initService = new InitSmokerService(repo);
                    var initCommand = new InitSmokerCommand
                    {
                        LimitPerDay = 20,
                        IntervalHour = 1,
                    };
                    initService.Execute(initCommand);
                    break;
                case "info":
                    var infoService = new InfoSmokerService(repo);
                    var infoCommand = new InfoSmokerCommand
                    {
                        AggregateID = GetAggregateID(args),
                    };
                    infoService.Execute(infoCommand).Wait();
                    break;
                case "smoke":
                    var smokeService = new SmokeService(repo);
                    var smokeCommand = new SmokeCommand
                    {
                        AggregateID = GetAggregateID(args),
                    };
                    smokeService.Execute(smokeCommand).Wait();
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
