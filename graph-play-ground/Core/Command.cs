namespace App.Core
{
    public class Command(IEnumerable<ScenarioBase> scenarios)
    {
        private ScenarioBase? scenario;

        public async Task Ask()
        {
            if(scenario == null)
            {
                Console.WriteLine("Available scenarios:");

                foreach(var s in scenarios)
                {
                    Console.WriteLine($" - {s.Name}");
                }

                Console.Write("Select a scenario: ");
                var scenariocommand = Console.ReadLine() ?? string.Empty;

                scenario = scenarios.FirstOrDefault(s => s.Name.Equals(scenariocommand, StringComparison.OrdinalIgnoreCase));

                return;
            }

            Console.Write($"[{scenario.Name}] Enter command (load, clear, run, exit, back): ");
            var command = Console.ReadLine() ?? string.Empty;

            switch (command)
            {
                case "exit":
                    Environment.Exit(0);
                    break;

                case "back":
                    scenario = null;
                    break;

                    case "clear":
                    await scenario.ClearDataAsync();
                    break;

                case "load":
                    await scenario.LoadDataAsync();
                    Console.WriteLine("Data loaded.");
                    break;

                default:
                    await scenario.RunQueriesAsync(command);
                    break;
            }
        }
    }
}
