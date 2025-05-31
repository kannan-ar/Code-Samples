namespace DomainService.Contracts;

[Alias("DomainService.Contracts.IOrderProcessorGrain")]
public interface IOrderProcessorGrain : IGrainWithStringKey
{
    [Alias("Process")]
    Task Process(string orderJson);

}