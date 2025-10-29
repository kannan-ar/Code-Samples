using App.Core;
using App.Scenarios.Logistics;

var driver = Neo4jDriverFactory.Create(password: "Testpwd@123");

var scenarios = new List<ScenarioBase>
{
    new LogisticsScenario(driver)
};

Command command = new(scenarios);

try
{
    while (true)
    {
        await command.Ask();
    }
}
finally
{
    await Neo4jDriverFactory.CloseAsync();
}




