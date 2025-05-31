using DomainService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainService.Domain;

public class OrderProcessorGrain : Grain, IOrderProcessorGrain
{
    public Task Process(string orderJson)
    {
        Console.WriteLine($"[Grain:{this.GetPrimaryKeyString()}] Processing order: {orderJson}");
        return Task.CompletedTask;
    }
}