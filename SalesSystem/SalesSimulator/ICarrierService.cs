namespace SalesSimulator
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using SalesSimulator.Models;
    public interface ICarrierService
    {
        Task<IList<Carrier>> GetCarriers();
        Carrier GetCarrier(ReadOnlyCollection<Carrier> allCarriers);
    }
}