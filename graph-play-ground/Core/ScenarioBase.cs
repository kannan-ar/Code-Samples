namespace App.Core;

public abstract class ScenarioBase
{
    public abstract string Name { get; }
    public abstract Task ClearDataAsync();
    public abstract Task LoadDataAsync();
    public abstract Task RunQueriesAsync(string command);
}